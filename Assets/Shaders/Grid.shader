Shader "Pathfinding/Grid"
{
    Properties
    {
        _Color ("Color", Color) = (0.25, 0.8, 1, 1)
        _LineWidth ("Line Width", Range(0.0, 0.5)) = 0.02
        _MapSize ("Map Size", float) = 10
        _EdgeFade ("Edge Fade", float) = 1
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
                float3 positionOS  : TEXCOORD0;
            };

            float4 _Color;
            float _LineWidth;
            float _MapSize;
            float _EdgeFade;

            float sdBox(float2 p, float2 size)
            {
                float2 d = abs(p) - size;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionOS  = v.positionOS.xyz;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 p = i.positionOS.xz;
                float2 f = frac(p);

                float2 d = min(f, 1.0 - f);
                float aa = fwidth(d.x) + fwidth(d.y);

                float gridSize = _MapSize * .5;
                float gridD = sdBox(p, float2(gridSize, gridSize));
                float gridAA = fwidth(gridD);

                float4 color = _Color;
                if (gridD < 0)
                    color.a *= 1.0 - smoothstep(_LineWidth - aa, _LineWidth + aa, min(d.x, d.y));
                else {
                    color.a *= 1.0 - smoothstep(_LineWidth - gridAA, _LineWidth + gridAA, gridD);
                }

                return color;
            }
            ENDHLSL
        }
    }
}
