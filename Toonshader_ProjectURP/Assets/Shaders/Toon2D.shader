Shader "Toon2D"{
    Properties{
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
        _MainTex ("BaseMap", 2D) = "white" {}
        _BaseColor_Step ("BaseColor_Step", Range(0, 1)) = 0.5
        _BaseShade_Feather ("Base/Shade_Feather", Range(0.0001, 1)) = 0.0001

        _1st_ShadeMap ("1st_ShadeMap", 2D) = "white" {}
        [Toggle(_)] _Use_BaseAs1st ("Use BaseMap as 1st_ShadeMap", Integer ) = 0
        _1st_ShadeColor ("1st_ShadeColor", Color) = (1,1,1,1)
        
        _2nd_ShadeMap ("2nd_ShadeMap", 2D) = "white" {}
        [Toggle(_)] _Use_1stAs2nd ("Use 1st_ShadeMap as 2nd_ShadeMap", Integer ) = 0
        _2nd_ShadeColor ("2nd_ShadeColor", Color) = (1,1,1,1)

        _ShadeColor_Step ("ShadeColor_Step", Range(0, 1)) = 0
        _1st2nd_Shades_Feather ("1st/2nd_Shades_Feather", Range(0.0001, 1)) = 0.0001


        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 1)) = 1
        
        
        [HideInInspector] _White("Tint", Color) = (1,1,1,1) // Added to break SRP batching. Work around for issue with SRP Batching
        
        
        //Outline
        _OutlineMode("Outline Mode", Integer) = 0
        _OutlineWidth ("Outline Width", Float ) = 0
        _OutlineWidthMap ("Outline Width Map", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineTex ("Outline Tex", 2D) = "black" {}
        _Outline_BlendBaseColor ("Blend Base Color to Outline", Integer ) = 0
        _OutlineOffsetZ ("Outline Z Offset", Float) = 0
        _OutlineNear ("Outline Near", Float ) = 0.5
        _OutlineFar ("Outline Far", Float ) = 100
        _Outline_UseCustomNormalMap ("Use Custom Normal Map", Integer ) = 0
        _Outline_CustomNormalMap ("Custom Normal Map", 2D) = "white" {}

        //Specular
        _Specular_UseDirectionalLight ("Specular Intensity", Integer) = 0
        _Specular_Color ("Specular Color", Color) = (1,1,1,1)
        _Specular_Intensity ("Specular Intensity", float) = 0
        _Specular_LightDirection ("Specular Light Direction", Vector) = (0,-1,0,0)
        
        //Test
		_OutlineExtrusion("Outline Extrusion", float) = 0.02
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

            struct Attributes {
                COMMON_2D_INPUTS
            };

            struct Varyings {
                COMMON_2D_LIT_OUTPUTS
            };

            float4 _White;

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Lit2DCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _Unlit_Intensity;

                int _Use_BaseAs1st;
                int _Use_1stAs2nd;

                float4 _1st_ShadeColor;
                float4 _2nd_ShadeColor;

                float _BaseColor_Step;
                float _BaseShade_Feather;
            
                float _ShadeColor_Step;
                float _1st2nd_Shades_Feather;
            CBUFFER_END

//----------------------------------------------------------------------------------------------------------------------            
            float4 _MainTex_ST;

            TEXTURE2D(_1st_ShadeMap);
            TEXTURE2D(_2nd_ShadeMap);

            Varyings LitVertex(Attributes input) {
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

                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor)) {
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
                float3 baseColor = surfaceData.albedo.rgb;

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

                float4 _Set_1st_ShadePosition_var = float4(1, 1, 1, 1);
                float4 _Set_2nd_ShadePosition_var = float4(1, 1, 1, 1);


                float _SystemShadowsLevel_var = 0.5f;
                float _Set_SystemShadowsToBase = 1.0f;
                float Set_FinalShadowMask = saturate((1.0 + ( (lerp( _HalfLambert_var, _HalfLambert_var*saturate(_SystemShadowsLevel_var), _Set_SystemShadowsToBase ) - (_BaseColor_Step-_BaseShade_Feather)) * ((1.0 - _Set_1st_ShadePosition_var.rgb).r - 1.0) ) / (_BaseColor_Step - (_BaseColor_Step-_BaseShade_Feather))));
                

                float innerLerpOp = saturate((1.0 + ((_HalfLambert_var - (_ShadeColor_Step - _1st2nd_Shades_Feather)) * ((1.0 - _Set_2nd_ShadePosition_var.rgb).r - 1.0)) / ( _ShadeColor_Step - ( _ShadeColor_Step - _1st2nd_Shades_Feather))));
                
                float3 Set_FinalBaseColor = lerp(baseColor, lerp(firstShadeColor, secondShadeColor,
                                                                 innerLerpOp),
                                                 Set_FinalShadowMask);

                //test
                //Set_FinalBaseColor = firstShadeColor;

                Set_FinalBaseColor = Set_FinalBaseColor * shapeLight0 * _ShapeLightBlendFactors0.x;
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
                float3 lightColor = max(defaultLightColor, saturate(originalLightColor));
                float3 Set_LightColor = lightColor.rgb;

                float4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex));
                float3 Set_BaseColor = ((_BaseColor.rgb * _MainTex_var.rgb) * Set_LightColor);


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

//----------------------------------------------------------------------------------------------------------------------
        Pass {
            Name "Outline"
            Tags {
                "LightMode" = "SRPDefaultUnlit"
            }
//            Cull [_SRPDefaultUnlitColMode]
//            ColorMask [_SPRDefaultUnlitColorMask]
//            Blend SrcAlpha OneMinusSrcAlpha
//            Stencil
//            {
//                Ref[_StencilNo]
//                Comp[_StencilComp]
//                Pass[_StencilOpPass]
//                Fail[_StencilOpFail]
//
//            }

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment


            #pragma multi_compile _IS_OUTLINE_CLIPPING_NO _IS_OUTLINE_CLIPPING_YES
            #pragma multi_compile _OUTLINE_NML _OUTLINE_POS
            // Outline is implemented in UniversalToonOutline.hlsl.
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"


           struct OutlineVertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
           };
            
            struct OutlineVertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };            
            
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			float _OutlineExtrusion;

            TEXTURE2D(_OutlineTex);
            SAMPLER(sampler_OutlineTex);
            float4 _OutlineTex_ST;

            TEXTURE2D(_Outline_CustomNormalMap);
            SAMPLER(sampler_Outline_CustomNormalMap);
            float4 _Outline_CustomNormalMap_ST;
            int    _Outline_UseCustomNormalMap;

            float _OutlineOffsetZ;
            float _OutlineWidth; 
            float _OutlineNear; 
            float _OutlineFar;

// #ifdef UNIVERSAL_PIPELINE_CORE_INCLUDED
//             #include "../../UniversalRP/Shaders/UniversalToonInput.hlsl"
//             #include "../../UniversalRP/Shaders/UniversalToonHead.hlsl"
//             #include "../../UniversalRP/Shaders/UniversalToonOutline.hlsl"
// #endif


inline float4 UnityObjectToClipPosInstanced(in float3 pos) {
    return mul(UNITY_MATRIX_VP, mul(GetObjectToWorldMatrix(), float4(pos, 1.0)));
}
#define UnityObjectToClipPos UnityObjectToClipPosInstanced
            

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
            
            
inline float3 UnityObjectToWorldNormal(in float3 norm)
{
#ifdef UNITY_ASSUME_UNIFORM_SCALING
    return UnityObjectToWorldDir(norm);
#else
    // mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
    return normalize(mul(norm, (float3x3)GetWorldToObjectMatrix()));
#endif
}
            
            OutlineVertexOutput OutlineVertex(OutlineVertexInput v) {
                OutlineVertexOutput o = (OutlineVertexOutput) 0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                const float2 uv = v.texcoord0;
                o.uv0 = v.texcoord0;
                
                float4 objPos = mul (GetObjectToWorldMatrix(), float4(0,0,0,1) );
                float4 _Outline_Sampler_var = tex2Dlod(sampler_OutlineTex,float4(TRANSFORM_TEX(uv, _OutlineTex),0.0,0));
                o.normalDir = UnityObjectToWorldNormal(v.normal);

                const float3 tangentDir = normalize( mul( GetObjectToWorldMatrix(), float4( v.tangent.xyz, 0.0 ) ).xyz );
                const float3 bitangentDir = normalize(cross(o.normalDir, tangentDir) * v.tangent.w);
                float3x3 tangentTransform = float3x3(tangentDir, bitangentDir, o.normalDir);

                //UnpackNormal() can't be used, and so as follows. Do not specify a bump for the texture to be used.
                float4 _BakedNormal_var = (tex2Dlod(sampler_Outline_CustomNormalMap,float4(TRANSFORM_TEX(uv, _Outline_CustomNormalMap),0.0,0)) * 2 - 1);
                float3 _BakedNormalDir = normalize(mul(_BakedNormal_var.rgb, tangentTransform));
    
                float Set_Outline_Width = (_OutlineWidth*0.001*smoothstep( _OutlineFar, _OutlineNear, distance(objPos.rgb,_WorldSpaceCameraPos) )*_Outline_Sampler_var.rgb).r;
                float4 _ClipCameraPos = mul(UNITY_MATRIX_VP, float4(_WorldSpaceCameraPos.xyz, 1));
                _OutlineOffsetZ = _OutlineOffsetZ * -0.01;

				float3 newPos;
#ifdef _OUTLINE_NML
                newPos = lerp(float4(v.vertex.xyz + v.normal*Set_Outline_Width,1), float4(v.vertex.xyz + _BakedNormalDir*Set_Outline_Width,1),_Outline_UseCustomNormalMap);
                o.pos = TransformObjectToHClip(newPos);
#elif _OUTLINE_POS
                Set_Outline_Width = Set_Outline_Width*2;
                float signVar = dot(normalize(v.vertex.xyz),normalize(v.normal))<0 ? -1 : 1;
                o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + signVar*normalize(v.vertex)*Set_Outline_Width, 1));
#endif
                o.pos.z = o.pos.z + _OutlineOffsetZ * _ClipCameraPos.z;
    
                return o;
            }

            

            half4 OutlineFragment(OutlineVertexOutput input) : SV_Target {


#if (UNITY_VERSION >= 202230)
                return float4(0,1,1,1);
#else
                return float4(1,0,0,1);
#endif                
            }
            
            
            ENDHLSL
        }


//----------------------------------------------------------------------------------------------------------------------
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

            struct Attributes {
                COMMON_2D_INPUTS
            };

            struct Varyings {
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

    CustomEditor "UnityToon3Das2DGUI"

}