Shader "Map/Path"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Speed ("Speed", float) = .2
        _Size ("Size", float) = .1
        _Blur ("Blur", float) = .1

        _MapSize ("Map Size", float) = 10
        _PathSize ("Path Size", float) = 10
        _StartTime ("Start Time", float) = 10
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            float4 _Color;
            float _Speed;
            float _Size;
            float _Blur;

            float _MapSize;
            StructuredBuffer<float2> _Path;
            float _PathSize;
            float _StartTime;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            float distanceToSegment(float2 p, float2 a, float2 b) {
                float2 ab = b - a;
                float2 ap = p - a;

                float t = dot(ap, ab) / dot(ab, ab);
                t = saturate(t);

                float2 closest = a + ab * t;
                return length(p - closest);
            }
            float distanceToClampedSegment(float2 p, float2 a, float2 b, float size) {
                float2 ab = b - a;
                float abLen = length(ab);

                float2 bClamped = a + ab * min(size / abLen, 1.0);

                return distanceToSegment(p, a, bClamped);
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 p = i.positionWS.xz;
                float time = _Time.y - _StartTime;

                float l = _Speed * time;

                float d = 10.0;
                for (int i = 1; i < _PathSize; i++) {
                    if (l <= 0.0)
                        break;
                    d = min(d, distanceToClampedSegment(p, _Path[i - 1], _Path[i], l));
                    l -= length(_Path[i] - _Path[i - 1]);
                }

                float4 color = _Color;
                color.a *= 1.0 - smoothstep(_Size, _Size + _Blur, d);
                return color;
            }
            ENDHLSL
        }
    }
}
