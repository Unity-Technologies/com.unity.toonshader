Shader "Toon2D" {
    Properties {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        [HideInInspector] _White("Tint", Color) = (1,1,1,1)  // Added to break SRP batching. Work around for issue with SRP Batching
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Back
        ZWrite On

        Stencil {
            Ref 128 // Put this in the last bit of our stencil value for maximum compatibility with sprite mask
            Comp always
            Pass replace
        }

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

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

            Varyings LitVertex(Attributes input)
            {
                return CommonLitVertex(input);
            }

            half4 LitFragment(Varyings input) : SV_Target
            {

                // float3 Set_LightColor = lightColor.rgb;
                // float3 Set_BaseColor = lerp( (_BaseColor.rgb*_MainTex_var.rgb), ((_BaseColor.rgb*_MainTex_var.rgb)*Set_LightColor), _Is_LightColor_Base );
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


                return float4(1,0,0,1); 

                
                return CommonLitFragment(input, _White);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}

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

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

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
