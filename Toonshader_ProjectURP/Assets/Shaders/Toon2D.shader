Shader "Toon2D"{
    Properties{
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
        _MainTex ("BaseMap", 2D) = "white" {}

        _1st_ShadeMap ("1st_ShadeMap", 2D) = "white" {}
        [Toggle(_)] _Use_BaseAs1st ("Use BaseMap as 1st_ShadeMap", Float ) = 0
        _1st_ShadeColor ("1st_ShadeColor", Color) = (1,1,1,1)
        [Toggle(_)] _Is_LightColor_1st_Shade ("Is_LightColor_1st_Shade", Float ) = 1
        _2nd_ShadeMap ("2nd_ShadeMap", 2D) = "white" {}
        [Toggle(_)] _Use_1stAs2nd ("Use 1st_ShadeMap as 2nd_ShadeMap", Float ) = 0
        _2nd_ShadeColor ("2nd_ShadeColor", Color) = (1,1,1,1)



        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Unlit_Intensity ("Unlit_Intensity", Range(0, 4)) = 0

        [Toggle(_)] _Is_Filter_LightColor ("VRChat : SceneLights HiCut_Filter", Float ) = 1
        [Toggle(_)] _Is_LightColor_Base ("Is_LightColor_Base", Float ) = 1

        [HideInInspector] _White("Tint", Color) = (1,1,1,1) // Added to break SRP batching. Work around for issue with SRP Batching
    }

    SubShader{
        Tags{
            "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Back
        ZWrite On



        Stencil{
            Ref 128 // Put this in the last bit of our stencil value for maximum compatibility with sprite mask
            Comp always
            Pass replace
        }

        Pass{
            Tags{
                "LightMode" = "Universal2D"
            }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex LitVertex
            #pragma fragment LitFragment

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                COMMON_2D_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_LIT_OUTPUTS
            };

            float4 _White;

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Lit2DCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _Unlit_Intensity;
                float _Is_Filter_LightColor;
                float _Is_LightColor_Base;

                float _Use_BaseAs1st;
                float _Use_1stAs2nd;

                float4 _1st_ShadeColor;
                float4 _2nd_ShadeColor;

                float _ShadeColor_Step;
                float _1st2nd_Shades_Feather;


            CBUFFER_END

            //----------------------------------------------------------------------------------------------------------------------            
            float4 _MainTex_ST;

            //TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_1st_ShadeMap);
            TEXTURE2D(_2nd_ShadeMap);


            Varyings LitVertex(Attributes input)
            {
                return CommonLitVertex(input);
            }

            //----------------------------------------------------------------------------------------------------------------------

            // normal should be normalized, w=1.0
            half3 SHEvalLinearL0L1(half4 normal)
            {
                half3 x;

                // Linear (L1) + constant (L0) polynomial terms
                x.r = dot(unity_SHAr, normal);
                x.g = dot(unity_SHAg, normal);
                x.b = dot(unity_SHAb, normal);

                return x;
            }

            // normal should be normalized, w=1.0
            half3 SHEvalLinearL2(half4 normal)
            {
                half3 x1, x2;
                // 4 of the quadratic (L2) polynomials
                half4 vB = normal.xyzz * normal.yzzx;
                x1.r = dot(unity_SHBr, vB);
                x1.g = dot(unity_SHBg, vB);
                x1.b = dot(unity_SHBb, vB);

                // Final (5th) quadratic (L2) polynomial
                half vC = normal.x * normal.x - normal.y * normal.y;
                x2 = unity_SHC.rgb * vC;

                return x1 + x2;
            }


            // normal should be normalized, w=1.0
            // output in active color space
            half3 ShadeSH9(half4 normal)
            {
                // Linear + constant polynomial terms
                half3 res = SHEvalLinearL0L1(normal);

                // Quadratic polynomials
                res += SHEvalLinearL2(normal);

                #   ifdef UNITY_COLORSPACE_GAMMA
                res = LinearToGammaSpace(res);
                #   endif

                return res;
            }


            //----------------------------------------------------------------------------------------------------------------------

            half4 CombinedShapeLightShared2(in SurfaceData2D surfaceData, in InputData2D inputData, in float2 uv)
            {
                #if defined(DEBUG_DISPLAY)
                half4 debugColor = 0;

                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                half alpha = surfaceData.alpha;
                half4 color = half4(surfaceData.albedo, alpha);
                const half4 mask = surfaceData.mask;
                const half2 lightingUV = inputData.lightingUV;

                if (alpha == 0.0)
                    discard;


                //return float4(Set_BaseColor,1); 


                #if USE_SHAPE_LIGHT_TYPE_0
                half4 shapeLight0 = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, lightingUV);

                if (any(_ShapeLightMaskFilter0))
                {
                    half4 processedMask = (1 - _ShapeLightInvertedFilter0) * mask + _ShapeLightInvertedFilter0 * (1 -
                        mask);
                    shapeLight0 *= dot(processedMask, _ShapeLightMaskFilter0);
                }


                float4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                float3 baseColor = surfaceData.albedo.rgb * shapeLight0 * _ShapeLightBlendFactors0.x;

                // //v.2.0.5
                float4 _1st_ShadeMap_var = lerp(
                    SAMPLE_TEXTURE2D(_1st_ShadeMap, sampler_MainTex, TRANSFORM_TEX(uv, _MainTex)), _MainTex_var,
                    _Use_BaseAs1st);
                float3 firstShadeColor = _1st_ShadeColor.rgb * _1st_ShadeMap_var.rgb;

                float4 _2nd_ShadeMap_var = lerp(
                    SAMPLE_TEXTURE2D(_2nd_ShadeMap, sampler_MainTex, TRANSFORM_TEX(uv, _MainTex)), _1st_ShadeMap_var,
                    _Use_1stAs2nd);
                float3 secondShadeColor = _2nd_ShadeColor.rgb * _2nd_ShadeMap_var.rgb;

                float _HalfLambert_var = max(shapeLight0.r, max(shapeLight0.g, shapeLight0.b));

                //float _HalfLambert_var = 0.5*dot(lerp( i.normalDir, normalDirection, _Is_NormalMapToBase ),lightDirection)+0.5;
                //
                // float4 _Set_2nd_ShadePosition_var = tex2D(_Set_2nd_ShadePosition, TRANSFORM_TEX(uv, _Set_2nd_ShadePosition));
                // float4 _Set_1st_ShadePosition_var = tex2D(_Set_1st_ShadePosition, TRANSFORM_TEX(uv, _Set_1st_ShadePosition));
                // //v.2.0.6
                // //Minmimum value is same as the Minimum Feather's value with the Minimum Step's value as threshold.
                // float _SystemShadowsLevel_var = (shadowAttenuation*0.5)+0.5+_Tweak_SystemShadowsLevel > 0.001 ? (shadowAttenuation*0.5)+0.5+_Tweak_SystemShadowsLevel : 0.0001;
                // float Set_FinalShadowMask = saturate((1.0 + ( (lerp( _HalfLambert_var, _HalfLambert_var*saturate(_SystemShadowsLevel_var), _Set_SystemShadowsToBase ) - (_BaseColor_Step-_BaseShade_Feather)) * ((1.0 - _Set_1st_ShadePosition_var.rgb).r - 1.0) ) / (_BaseColor_Step - (_BaseColor_Step-_BaseShade_Feather))));
                // //
                // //Composition: 3 Basic Colors as Set_FinalBaseColor

                float4 _Set_2nd_ShadePosition_var = float4(1, 1, 1, 1);
                float Set_FinalShadowMask = 1;

                float3 Set_FinalBaseColor = lerp(baseColor, lerp(firstShadeColor, secondShadeColor,
                                                                 saturate(
                                                                     (1.0 + ((_HalfLambert_var - (_ShadeColor_Step -
                                                                         _1st2nd_Shades_Feather)) * ((1.0
                                                                         - _Set_2nd_ShadePosition_var.rgb).r - 1.0)) / (
                                                                         _ShadeColor_Step - (
                                                                             _ShadeColor_Step -
                                                                             _1st2nd_Shades_Feather))))),
                                                 Set_FinalShadowMask);


                half4 shapeLight0Modulate = half4(Set_FinalBaseColor, alpha);
                half4 shapeLight0Additive = shapeLight0 * _ShapeLightBlendFactors0.y;

                #else
                half4 shapeLight0Modulate = 0;
                half4 shapeLight0Additive = 0;
                #endif

                half4 finalOutput;
                #if !USE_SHAPE_LIGHT_TYPE_0 && !USE_SHAPE_LIGHT_TYPE_1 && !USE_SHAPE_LIGHT_TYPE_2 && ! USE_SHAPE_LIGHT_TYPE_3
                finalOutput = color;
                #else
                half4 finalModulate = shapeLight0Modulate;
                half4 finalAdditve = shapeLight0Additive;
                finalOutput = _HDREmulationScale * (finalModulate + finalAdditve);
                #endif

                finalOutput.a = alpha;

                return max(0, finalOutput);
            }

            half4 CommonLitFragment2(Varyings input, half4 color)
            {
                const half4 main = color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);
                const half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));

                SurfaceData2D surfaceData;
                InputData2D inputData;

                InitializeSurfaceData(main.rgb, main.a, mask, normalTS, surfaceData);
                InitializeInputData(input.uv, input.lightingUV, inputData);

                #if defined(DEBUG_DISPLAY)
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, input.positionWS, input.positionCS, _MainTex);
                surfaceData.normalWS = input.normalWS;
                #endif

                return CombinedShapeLightShared2(surfaceData, inputData, input.uv);
            }


            //----------------------------------------------------------------------------------------------------------------------
            half4 LitFragment(Varyings input) : SV_Target
            {
                float3 defaultLightDirection = normalize(UNITY_MATRIX_V[2].xyz + UNITY_MATRIX_V[1].xyz);
                float2 Set_UV0 = input.uv;
                float3 mainLightColor = float3(1, 1, 1);


                // //v.2.0.5
                float3 defaultLightColor = saturate(max(half3(0.05, 0.05, 0.05) * _Unlit_Intensity,
                    max(ShadeSH9(half4(0.0, 0.0, 0.0, 1.0)),
                        ShadeSH9(half4(0.0, -1.0, 0.0, 1.0)).rgb) *
                    _Unlit_Intensity));
                // float3 customLightDirection = normalize(mul( GetObjectToWorldMatrix(), float4(((float3(1.0,0.0,0.0)*_Offset_X_Axis_BLD*10)+(float3(0.0,1.0,0.0)*_Offset_Y_Axis_BLD*10)+(float3(0.0,0.0,-1.0)*lerp(-1.0,1.0,_Inverse_Z_Axis_BLD))),0)).xyz);
                // float3 lightDirection = normalize(lerp(defaultLightDirection, mainLight.direction.xyz,any(mainLight.direction.xyz)));
                // lightDirection = lerp(lightDirection, customLightDirection, _Is_BLD);
                // //v.2.0.5: 
                //
                half3 originalLightColor = mainLightColor.rgb;
                float3 lightColor = lerp(max(defaultLightColor, originalLightColor),
                              max(defaultLightColor, saturate(originalLightColor)),
                              _Is_Filter_LightColor);
                float3 Set_LightColor = lightColor.rgb;

                float4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex));
                float3 Set_BaseColor = lerp((_BaseColor.rgb * _MainTex_var.rgb),
                                                               ((_BaseColor.rgb * _MainTex_var.rgb) * Set_LightColor),
                                                               _Is_LightColor_Base);


                // //v.2.0.5
                // float4 _1st_ShadeMap_var = lerp(SAMPLE_TEXTURE2D(_1st_ShadeMap,sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex)),_MainTex_var,_Use_BaseAs1st);
                // float3 Set_1st_ShadeColor = lerp( (_1st_ShadeColor.rgb*_1st_ShadeMap_var.rgb), ((_1st_ShadeColor.rgb*_1st_ShadeMap_var.rgb)*Set_LightColor), _Is_LightColor_1st_Shade );
                // //v.2.0.5
                // float4 _2nd_ShadeMap_var = lerp(SAMPLE_TEXTURE2D(_2nd_ShadeMap, sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex)),_1st_ShadeMap_var,_Use_1stAs2nd);
                // float3 Set_2nd_ShadeColor = lerp( (_2nd_ShadeColor.rgb*_2nd_ShadeMap_var.rgb), ((_2nd_ShadeColor.rgb*_2nd_ShadeMap_var.rgb)*Set_LightColor), _Is_LightColor_2nd_Shade );
                // float _HalfLambert_var = 0.5*dot(lerp( i.normalDir, normalDirection, _Is_NormalMapToBase ),lightDirection)+0.5;
                //
                // float4 _Set_2nd_ShadePosition_var = tex2D(_Set_2nd_ShadePosition, TRANSFORM_TEX(Set_UV0, _Set_2nd_ShadePosition));
                // float4 _Set_1st_ShadePosition_var = tex2D(_Set_1st_ShadePosition, TRANSFORM_TEX(Set_UV0, _Set_1st_ShadePosition));
                // //v.2.0.6
                // //Minmimum value is same as the Minimum Feather's value with the Minimum Step's value as threshold.
                // float _SystemShadowsLevel_var = (shadowAttenuation*0.5)+0.5+_Tweak_SystemShadowsLevel > 0.001 ? (shadowAttenuation*0.5)+0.5+_Tweak_SystemShadowsLevel : 0.0001;
                // float Set_FinalShadowMask = saturate((1.0 + ( (lerp( _HalfLambert_var, _HalfLambert_var*saturate(_SystemShadowsLevel_var), _Set_SystemShadowsToBase ) - (_BaseColor_Step-_BaseShade_Feather)) * ((1.0 - _Set_1st_ShadePosition_var.rgb).r - 1.0) ) / (_BaseColor_Step - (_BaseColor_Step-_BaseShade_Feather))));
                // //
                // //Composition: 3 Basic Colors as Set_FinalBaseColor
                // float3 Set_FinalBaseColor = lerp(Set_BaseColor,lerp(Set_1st_ShadeColor,Set_2nd_ShadeColor,saturate((1.0 + ( (_HalfLambert_var - (_ShadeColor_Step-_1st2nd_Shades_Feather)) * ((1.0 - _Set_2nd_ShadePosition_var.rgb).r - 1.0) ) / (_ShadeColor_Step - (_ShadeColor_Step-_1st2nd_Shades_Feather))))),Set_FinalShadowMask); // Final Color


                //return float4(Set_BaseColor,1); 


                return CommonLitFragment2(input, _White);
            }
            ENDHLSL
        }

        Pass{
            Tags{
                "LightMode" = "NormalsRendering"
            }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            // GPU Instancing
            #pragma multi_compile_instancing

            struct Attributes
            {
                COMMON_2D_NORMALS_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_NORMALS_OUTPUTS
            };

            float4 _White;

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Normals2DCommon.hlsl"

            Varyings NormalsRenderingVertex(Attributes input)
            {
                return CommonNormalsVertex(input);
            }

            half4 NormalsRenderingFragment(Varyings input) : SV_Target
            {
                return CommonNormalsFragment(input, _White);
            }
            ENDHLSL
        }

        Pass{
            Tags{
                "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"
            }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            // GPU Instancing
            #pragma multi_compile_instancing

            struct Attributes
            {
                COMMON_2D_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
            };

            float4 _White;

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            Varyings UnlitVertex(Attributes input)
            {
                return CommonUnlitVertex(input);
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                return CommonUnlitFragment(input, _White);
            }
            ENDHLSL
        }
    }
}