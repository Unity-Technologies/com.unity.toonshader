Shader "Universal Render Pipeline/Toon/ToonURP_MetallicDissolve"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}

        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _RoughnessMap ("Roughness Map", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Roughness ("Roughness", Range(0,1)) = 0.5
        _InvertSmoothness ("Invert Smoothness Map", Float) = 0

        _ToonBlend ("Toon Blend (0=PBR,1=Toon)", Range(0,1)) = 1.0

        _DissolveMap ("Dissolve Noise", 2D) = "white" {}
        _DissolveThreshold ("Dissolve Threshold", Range(0,1)) = 1.0
        _DissolveSoftness ("Dissolve Softness", Range(0,1)) = 0.1
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1,0.7,0.2,1)
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0,1)) = 0.02
        _DissolvePulseSpeed ("Dissolve Pulse Speed", Float) = 0.0

        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _ALPHAPREMULTIPLY_ON
            #pragma multi_compile _ _SPECULAR_SETUP
            #pragma multi_compile _METALLICSPECGLOSSMAP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Runtime/Shaders/URP/URPIncludeGuards.hlsl"
            #include "Runtime/Shaders/URP/UniversalToonInput.hlsl"
            #include "Runtime/Shaders/URP/UniversalToonLighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SETUP_INSTANCE_ID(v);

                float4 posWS = mul(GetObjectToWorldMatrix(), v.positionOS);
                o.positionCS = GetVertexPositionInputs(v.positionOS.xyz).positionCS;
                o.uv = v.uv;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                float3 worldPos = posWS.xyz;
                o.viewDirWS = _WorldSpaceCameraPos - worldPos;
                return o;
            }

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST;

            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);

            TEXTURE2D(_RoughnessMap);
            SAMPLER(sampler_RoughnessMap);

            TEXTURE2D(_DissolveMap);
            SAMPLER(sampler_DissolveMap);

            float _Metallic;
            float _Roughness;
            float _InvertSmoothness;
            float _ToonBlend;

            float _DissolveThreshold;
            float _DissolveSoftness;
            float4 _DissolveEdgeColor;
            float _DissolveEdgeWidth;
            float _DissolvePulseSpeed;

            half4 _BaseColor;
            half _Cutoff;
            half4 _EmissionColor;

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                // Albedo
                half4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half3 albedo = baseSample.rgb * _BaseColor.rgb;
                float alpha = baseSample.a * _BaseColor.a;

                // Dissolve
                float dissolveVal = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, uv).r;
                // pulse affects dissolveVal if requested
                if (_DissolvePulseSpeed > 0.0001)
                {
                    dissolveVal += sin(_Time.y * _DissolvePulseSpeed) * 0.5;
                }
                float edgeMask = smoothstep(_DissolveThreshold - _DissolveSoftness, _DissolveThreshold + _DissolveSoftness, dissolveVal);
                // edge detection for colored rim
                float edge = smoothstep(_DissolveThreshold - _DissolveEdgeWidth, _DissolveThreshold + _DissolveEdgeWidth, dissolveVal) - edgeMask;

                // Apply cutoff/discard based on dissolve
                float finalAlpha = alpha * edgeMask;
                if (finalAlpha <= _Cutoff)
                    discard;

                // Normal
                half3 normalWS = UnpackNormalFromMap(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv).rgb);
                // if no bump map, use vertex normal
                // Note: UnpackNormalFromMap returns in tangent space in many libs; keep reasonable fallback
                if (length(normalWS) < 0.001) normalWS = normalize(IN.normalWS);

                // Metallic / Roughness sampling
                float metallic = _Metallic;
                float smoothness = _Roughness;
                #ifdef _METALLICSPECGLOSSMAP
                // existing flow in UTS uses metallic/specular map, but we support separate maps as additive feature
                #endif

                // Try sample maps if present (graceful fallback handled by shader keywords in URP includes)
                float3 metalSample = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, uv).rgb;
                float3 roughSample = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, uv).rgb;
                // Use red channel as metallic, roughness uses r channel
                // If a map is white/absent, these samples will be 1 and not harm
                metallic = lerp(metallic, metalSample.r, step(0.001, metalSample.r + metalSample.g + metalSample.b));
                smoothness = lerp(smoothness, roughSample.r, step(0.001, roughSample.r + roughSample.g + roughSample.b));
                if (_InvertSmoothness > 0.5) smoothness = 1 - smoothness;

                // Lighting calculations — obtain URP lighting via toon lighting include
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(uv, surfaceData);
                // Override metallic/smoothness from our sampled values for the new feature
                surfaceData.metallic = metallic;
                surfaceData.smoothness = smoothness;
                surfaceData.albedo = albedo;
                surfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), 1.0);

                // Calculate PBR lighting
                UnityGI gi = UnityGlobalIllumination(IN.positionCS.xyz);

                half3 pbrColor = SURFACE_PBR_LIGHTING(surfaceData, IN.viewDirWS, IN.normalWS);

                // Calculate Toon lighting via existing toon lighting utilities
                half3 toonColor = ToonLighting(surfaceData, IN.viewDirWS, IN.normalWS);

                // Blend between PBR and Toon according to _ToonBlend
                half3 finalColor = lerp(pbrColor, toonColor, _ToonBlend);

                // Add dissolve edge color on top
                finalColor = lerp(finalColor, _DissolveEdgeColor.rgb, saturate(edge * 10));

                // Emission
                finalColor += _EmissionColor.rgb;

                return half4(finalColor, finalAlpha);
            }

            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
