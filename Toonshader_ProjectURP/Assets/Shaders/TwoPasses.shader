Shader "Test/TwoPasses"{
    Properties{
        _Color1("Color 1", Color) = (1, 0, 0, 1)
        _Color2("Color 2", Color) = (0, 1, 0, 1)
        _NormalExtrusion("Normal Extrusion", float) = 0.02
    }
    SubShader{
        Pass{
            Name "FirstPass"
            Tags{
                "RenderType" = "Opaque"
                "RenderPipeline" = "UniversalPipeline"
                "LightMode" = "UniversalForward"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
            };

            struct Varyings{
                float4 positionHCS : SV_POSITION;
            };

            uniform half4 _Color1;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                return _Color1;
            }
            ENDHLSL
        }

        Pass{
            Name "SecondPass"

            Tags{
                "RenderType" = "Opaque"
                "RenderPipeline" = "UniversalPipeline"
                "LightMode" = "SRPDefaultUnlit"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            uniform half4 _Color2;
            float _NormalExtrusion;

            Varyings vert(Attributes IN){
                Varyings OUT;

                float3 newPos = IN.positionOS;
                float3 normal = normalize(IN.normal);
                newPos += float3(normal) * _NormalExtrusion;

                OUT.positionHCS = TransformObjectToHClip(newPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                return _Color2;
            }
            ENDHLSL
        }
    }

    CustomEditor "TwoPassesShaderGUI"

}