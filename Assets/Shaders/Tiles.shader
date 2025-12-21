Shader "Map/Tiles"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _StepDelay ("Step Delay", float) = .2
        _FadeInDuration ("Fade In Duration", float) = .5
        _MapSize ("Map Size", float) = 10
        _StartTime ("Start time", float) = 10
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
            float _StepDelay;
            float _FadeInDuration;

            float _MapSize;
            StructuredBuffer<float> _VisitOrder;
            float _StartTime;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 p = i.positionWS.xz;
                float time = _Time.y - _StartTime;

                float x = round(p.x);
                float y = round(p.y);
                
                if (x < 0.0 || x >= _MapSize || y < 0.0 || y >= _MapSize)
                    return float4(0.0, 0.0, 0.0, 0.0);

                int index = y * _MapSize + x;
                float order = _VisitOrder[index];

                if (order < 0.0)
                    return float4(0.0, 0.0, 0.0, 0.0);

                float4 color = _Color;
                color.a *= smoothstep(0.0, _FadeInDuration, time - order * _StepDelay);

                return color;
            }
            ENDHLSL
        }
    }
}
