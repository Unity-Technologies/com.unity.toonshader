Shader "Toon/Toon 3D as 2D"{
    Properties{
        
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}

        //Three Colors
        _1st_ShadeColor ("1st Shade Color", Color) = (0.5,0.5,0.5,1)
        _1st_ShadeMap ("1st Shade Map", 2D) = "white" {}
        [Toggle(_)] _Use_BaseAs1st ("Use BaseMap as 1st_ShadeMap", Integer ) = 0
        _2nd_ShadeColor ("2nd Shade Color", Color) = (0.1,0.1,0.1,1)
        _2nd_ShadeMap ("2nd Shade Map", 2D) = "white" {}
        [Toggle(_)] _Use_1stAs2nd ("Use 1st ShadeMap as 2nd ShadeMap", Integer ) = 0
        
        //Start and Feather
        _BaseTo1st_ShadeStart ("Base to 1st Shade Start", Range(0, 1)) = 0.5
        _BaseTo1st_ShadeFeather ("Base to 1st Shade Feather", Range(0, 1)) = 0.1
        _1stTo2nd_ShadeStart ("1st to 2nd Shade Start", Range(0, 1)) = 0.25
        _1stTo2nd_ShadeFeather ("1st to 2nd Shade Feather", Range(0, 1)) = 0.1
        
        _2DLightStrength ("2D Light Strength", Range(0,1)) = 1

        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 1)) = 1
        
        [HideInInspector] _White("Tint", Color) = (1,1,1,1) // Added to break SRP batching. Work around for issue with SRP Batching
        
        
        //Outline
        _OutlineMode("Outline Mode", Integer) = 0
        _OutlineWidth ("Outline Width", Float ) = 5
        _OutlineWidthMap ("Outline Width Map", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0.1,0.1,0.1,1)
        _OutlineTex ("Outline Tex", 2D) = "white" {}
        _Outline_BaseColorBlend ("Blend Base Color to Outline", Range(0,1) ) = 0.5
        _Outline_LightColorBlend ("Blend Light Color to Outline", Range(0,1) ) = 0.5
        _OutlineOffsetZ ("Outline Z Offset", Float) = 0
        _OutlineNear ("Outline Near", Float ) = 0.5
        _OutlineFar ("Outline Far", Float ) = 100
        _Outline_UseCustomNormalMap ("Use Custom Normal Map", Integer ) = 0
        _Outline_CustomNormalMap ("Custom Normal Map", 2D) = "white" {}
        
        //Directional Light
        _DirectionalLight_Use ("Use Directional Light", Integer) = 0
        _DirectionalLight_Direction ("Specular Light Direction", Vector) = (0,-1,0,0)
        _DirectionalLight_Color("Directional Light Color", Color) = (1,1,1,1)
        _DirectionalLight_Intensity ("Directional Light Intensity", float) = 0.5
        _DirectionalLight_DiffuseFactor ("Directional Light: Diffuse Factor", Range(0,1)) = 0.5
        _DirectionalLight_SpecularFactor ("Directional Light: Specular Factor", Range(0,1)) = 0.5
        
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

            #pragma vertex ToonVertex
            #pragma fragment ToonFragment

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes {
                float3 positionOS   : POSITION; 
                float2 uv           : TEXCOORD0;
                float3 normal       : NORMAL;  
                float4 tangent      : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;  
                half2 lightingUV    : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float4 tangentWS   : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _White;

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Lit2DCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _BumpScale;

                //Three colors
                float4 _1st_ShadeColor;
                int _Use_BaseAs1st;
                float4 _2nd_ShadeColor;
                int _Use_1stAs2nd;

                //Start and Feather
                float _BaseTo1st_ShadeStart;
                float _BaseTo1st_ShadeFeather;
                float _1stTo2nd_ShadeStart;
                float _1stTo2nd_ShadeFeather;
            
                float _2DLightStrength;
            
                int _DirectionalLight_Use;
                float3 _DirectionalLight_Direction;
                float4 _DirectionalLight_Color;
                float _DirectionalLight_Intensity;
                float _DirectionalLight_DiffuseFactor;
                float _DirectionalLight_SpecularFactor;
            
            CBUFFER_END

//----------------------------------------------------------------------------------------------------------------------            
            float4 _MainTex_ST;

            TEXTURE2D(_1st_ShadeMap);
            TEXTURE2D(_2nd_ShadeMap);

            #include "ObjectTransform.hlsl"
            
            Varyings ToonVertex(Attributes input) {

                Varyings o = (Varyings) 0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(input.positionOS);
                const float3 normalWS = TransformObjectToWorldDir(input.normal);
    
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(input.positionOS);
                o.normalWS = normalWS;
                #endif
                o.uv = input.uv;
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);
                o.normalWS = normalWS;

                const float3 tangentWS = normalize( mul( GetObjectToWorldMatrix(), float4( input.tangent.xyz, 0 ) ).xyz); 
                o.tangentWS = float4(tangentWS, input.tangent.w);
                
                return o;
            }


            float3 ThreeColorsLinearShading(
                float3 baseColor,
                float3 firstColor,
                float3 secondColor,
                float3  baseTo1stStart,     // t=0: use base, t=1: transition
                float3  baseTo1stFeather,
                float3  firstToSecondStart, //t=0: use base, t=1: transition
                float3  firstToSecondFeather,
                float  dotNL) // dot(N.L)
            {
                const float t = saturate(1 - dotNL); //t = 0: light, t=1: dark shaded

                const float invBaseTo1stStart = 1 - baseTo1stStart;
                const float invBaseTo2ndStart = 1 - firstToSecondStart;
                
                const float s1 = smoothstep(invBaseTo1stStart, invBaseTo1stStart + baseTo1stFeather,t); 
                const float s2 = smoothstep(invBaseTo2ndStart, invBaseTo2ndStart + firstToSecondFeather,t); 
                
                float3 c01 = lerp(baseColor,firstColor,  s1);
                float3 c12 = lerp(c01, secondColor, s2);
                return c12;
            }


            half4 CombinedShapeLightShared2(in SurfaceData2D surfaceData, in InputData2D inputData, in float2 uv,
                in float3 tangentWS, in float3 bitangentWS, in float3 normalWS)
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

                float3x3 tangentTransform = float3x3( tangentWS, bitangentWS, normalWS);
                const float3 normalTS = surfaceData.normalTS;
                float3 perturbedNormalWS = normalize(mul( normalTS, tangentTransform )); // Perturbed normals

                #if USE_SHAPE_LIGHT_TYPE_0
                half4 shapeLight0 = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, lightingUV);

                if (any(_ShapeLightMaskFilter0))
                {
                    half4 processedMask = (1 - _ShapeLightInvertedFilter0) * mask + _ShapeLightInvertedFilter0 * (1 -
                        mask);
                    shapeLight0 *= dot(processedMask, _ShapeLightMaskFilter0);
                }

                const float3 diffuseLightFactor = (shapeLight0.rgb * _2DLightStrength)
                    + (_DirectionalLight_Color * _DirectionalLight_DiffuseFactor * _DirectionalLight_Use);
                
                const float4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                const float3 baseColor = _BaseColor.rgb * surfaceData.albedo.rgb * diffuseLightFactor;

                //1st and 2nd Shade
                float4 _1st_ShadeMap_var = lerp(
                    SAMPLE_TEXTURE2D(_1st_ShadeMap, sampler_MainTex, TRANSFORM_TEX(uv, _MainTex)), _MainTex_var,
                    _Use_BaseAs1st);
                const float3 firstShadeAlbedo = _1st_ShadeColor.rgb * _1st_ShadeMap_var.rgb; 
                const float3 firstShadeColor = firstShadeAlbedo * diffuseLightFactor;

                float4 _2nd_ShadeMap_var = lerp(
                    SAMPLE_TEXTURE2D(_2nd_ShadeMap, sampler_MainTex, TRANSFORM_TEX(uv, _MainTex)), _1st_ShadeMap_var,
                    _Use_1stAs2nd);
                const float3 secondShadeAlbedo = _2nd_ShadeColor.rgb * _2nd_ShadeMap_var.rgb;
                const float3 secondShadeColor = secondShadeAlbedo * diffuseLightFactor;

                const float light2dDiffuse = max(shapeLight0.r, max(shapeLight0.g, shapeLight0.b)); 
                const float directionalDiffuse = 0.5 * dot( perturbedNormalWS, _DirectionalLight_Direction) + 0.5;

                float lightFactor = (light2dDiffuse * _2DLightStrength)
                    + (directionalDiffuse * _DirectionalLight_DiffuseFactor);

                float3 Set_FinalBaseColor = ThreeColorsLinearShading(baseColor,firstShadeColor, secondShadeColor,
                    _BaseTo1st_ShadeStart, _BaseTo1st_ShadeFeather,
                    _1stTo2nd_ShadeStart, _1stTo2nd_ShadeFeather, lightFactor);
                
                // float4 _Set_HighColorMask_var = tex2D(_Set_HighColorMask, TRANSFORM_TEX(Set_UV0, _Set_HighColorMask));
                //
                // float _Specular_var = 0.5*dot(halfDirection,lerp( i.normalDir, normalDirection, _Is_NormalMapToHighColor ))+0.5; // Specular
                // float _TweakHighColorMask_var = (
                //   saturate((_Set_HighColorMask_var.g+_Tweak_HighColorMaskLevel))*lerp( (1.0 - step(_Specular_var,(1.0 - pow(abs(_HighColor_Power),5)))), pow(abs(_Specular_var),exp2(lerp(11,1,_HighColor_Power))), _Is_SpecularToHighColor ));
                //
                // float4 _HighColor_Tex_var = tex2D(_HighColor_Tex, TRANSFORM_TEX(Set_UV0, _HighColor_Tex));
                //
                // float3 _HighColor_var = (lerp( (_HighColor_Tex_var.rgb*_HighColor.rgb), ((_HighColor_Tex_var.rgb*_HighColor.rgb)*Set_LightColor), _Is_LightColor_HighColor )*_TweakHighColorMask_var);
                // //Composition: 3 Basic Colors and HighColor as Set_HighColor
                // float3 Set_HighColor = (lerp(SATURATE_IF_SDR((Set_FinalBaseColor-_TweakHighColorMask_var)), Set_FinalBaseColor, lerp(_Is_BlendAddToHiColor,1.0,_Is_SpecularToHighColor) )+lerp( _HighColor_var, (_HighColor_var*((1.0 - Set_FinalShadowMask)+(Set_FinalShadowMask*_TweakHighColorOnShadow))), _Is_UseTweakHighColorOnShadow ));
    

                Set_FinalBaseColor = Set_FinalBaseColor * _ShapeLightBlendFactors0.x;

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

            half4 ToonFragment(Varyings input) : SV_Target {
                const half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);
                const half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _BumpScale);

    
                SurfaceData2D surfaceData;
                InputData2D inputData;

                const float3 normalWS = normalize(input.normalWS);
                const float3 tangentWS = normalize(input.tangentWS);
                const float3 bitangentWS = normalize(cross(normalWS, tangentWS) * input.tangentWS.w);
                
                InitializeSurfaceData(main.rgb, main.a, mask, normalTS, surfaceData);
                InitializeInputData(input.uv, input.lightingUV, inputData);

                #if defined(DEBUG_DISPLAY)
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, input.positionWS, input.positionCS, _MainTex);
                surfaceData.normalWS = input.normalWS;
                #endif

                return CombinedShapeLightShared2(surfaceData, inputData, input.uv,
                    tangentWS, bitangentWS, normalWS);
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

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"

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
                half2 lightingUV  : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };            

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            
            TEXTURE2D(_OutlineTex);
            SAMPLER(sampler_OutlineTex);
            float4 _OutlineTex_ST;

            TEXTURE2D(_Outline_CustomNormalMap);
            SAMPLER(sampler_Outline_CustomNormalMap);
            float4 _Outline_CustomNormalMap_ST;
            int    _Outline_UseCustomNormalMap;

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _OutlineOffsetZ;
                float _OutlineWidth; 
                float _OutlineNear; 
                float _OutlineFar;
                float4 _OutlineColor;
                float _Outline_BaseColorBlend;
                float _Outline_LightColorBlend;
                
            CBUFFER_END

            #include "ObjectTransform.hlsl"
            
            OutlineVertexOutput OutlineVertex(OutlineVertexInput v) {
                OutlineVertexOutput o = (OutlineVertexOutput) 0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                const float2 uv = v.texcoord0;
                o.uv0 = v.texcoord0;
                
                float4 objPos = mul (GetObjectToWorldMatrix(), float4(0,0,0,1) );
                float4 _Outline_Sampler_var = tex2Dlod(sampler_OutlineTex,float4(TRANSFORM_TEX(uv, _OutlineTex),0.0,0));
                const float3 normalDir = UnityObjectToWorldNormal(v.normal);

                const float3 tangentDir = normalize( mul( GetObjectToWorldMatrix(), float4( v.tangent.xyz, 0.0 ) ).xyz );
                const float3 bitangentDir = normalize(cross(normalDir, tangentDir) * v.tangent.w);
                float3x3 tangentTransform = float3x3(tangentDir, bitangentDir, normalDir);

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

                o.lightingUV = half2(ComputeScreenPos(o.pos / o.pos.w).xy);
                
    
                return o;
            }

            //SHAPE_LIGHT macros
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            
            half4 OutlineFragment(OutlineVertexOutput i) : SV_Target {

                InputData2D inputData;

                InitializeInputData(i.uv0, i.lightingUV, inputData);

                half4 shapeLight0 = half4(0,0,0,0);

                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv0);
                const half2 lightingUV = inputData.lightingUV;

                #if USE_SHAPE_LIGHT_TYPE_0
                shapeLight0 = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, lightingUV);
                if (any(_ShapeLightMaskFilter0))
                {
                    half4 processedMask = (1 - _ShapeLightInvertedFilter0) * mask + _ShapeLightInvertedFilter0 * (1 -
                        mask);
                    shapeLight0 *= dot(processedMask, _ShapeLightMaskFilter0);
                }
                #endif

                float3 lightColor = shapeLight0.rgb;
                
                const float2 Set_UV0 = i.uv0;
                float4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex));
                float3 Set_BaseColor = _BaseColor.rgb * _MainTex_var.rgb;
                
                const float3 outlineTex = tex2D(sampler_OutlineTex,TRANSFORM_TEX(Set_UV0, _OutlineTex)).rgb;
                const float3 outlineAlbedo = outlineTex * _OutlineColor.rgb;

                //Blend
                const float3 outlineBaseBlend = lerp(outlineAlbedo, outlineAlbedo * Set_BaseColor, _Outline_BaseColorBlend);
                const float3 outlineBaseAndLightBlend = lerp(outlineBaseBlend, outlineBaseBlend * lightColor, _Outline_LightColorBlend);
                
#ifdef _IS_OUTLINE_CLIPPING_NO
                return float4(outlineBaseAndLightBlend,1.0);
#elif _IS_OUTLINE_CLIPPING_YES
                float4 _ClippingMask_var = SAMPLE_TEXTURE2D(_ClippingMask, sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex));
                float Set_MainTexAlpha = _MainTex_var.a;
                float _IsBaseMapAlphaAsClippingMask_var = lerp( _ClippingMask_var.r, Set_MainTexAlpha, _IsBaseMapAlphaAsClippingMask );
                float _Inverse_Clipping_var = lerp( _IsBaseMapAlphaAsClippingMask_var, (1.0 - _IsBaseMapAlphaAsClippingMask_var), _Inverse_Clipping );
                float Set_Clipping = saturate((_Inverse_Clipping_var+_Clipping_Level));
                clip(Set_Clipping - 0.5);
                float4 Set_Outline_Color = lerp( float4(_Is_BlendBaseColor_var,Set_Clipping), float4((_OutlineTex_var.rgb*_Outline_Color.rgb*lightColor),Set_Clipping), _Is_OutlineTex );
                return Set_Outline_Color;
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

            struct Attributes {
                COMMON_2D_NORMALS_INPUTS
            };

            struct Varyings {
                COMMON_2D_NORMALS_OUTPUTS
            };

            float4 _White;

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Normals2DCommon.hlsl"

            Varyings NormalsRenderingVertex(Attributes input) {
                return CommonNormalsVertex(input);
            }

            half4 NormalsRenderingFragment(Varyings input) : SV_Target {
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

            Varyings UnlitVertex(Attributes input) {
                return CommonUnlitVertex(input);
            }

            half4 UnlitFragment(Varyings input) : SV_Target {
                return CommonUnlitFragment(input, _White);
            }
            ENDHLSL
        }
        
    }

    CustomEditor "UnityToon3Das2DGUI"

}