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
        
        //Directional Light
        _DirectionalLight_Use ("Use Directional Light", Integer) = 0
        _DirectionalLight_Direction ("Directional Light Direction", Vector) = (0,-1,0,0)
        _DirectionalLight_Color("Directional Light Color", Color) = (1,1,1,1)
        _DirectionalLight_Intensity ("Directional Light Intensity", float) = 0.5
        _DirectionalLight_DiffuseStrength ("Directional Light: Diffuse Strength", Range(0,1)) = 0.5

        _DirectionalLight_ViewPosition ("Directional Light: View Position", Vector) = (0,0,1,0)
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _HighlightTex ("HighColor Map", 2D) = "white" {}
        _DirectionalLight_HighlightMode ("Directional Light: Highlight Mode", Integer) = 0 //0: Hard, 1: Soft
        _DirectionalLight_HighlightStrength ("Directional Light: Highlight Strength", Range(0,1)) = 0.5
        _DirectionalLight_HighlightSize ("Directional Light: Highlight Size", Range(0,1)) = 0.3
        
        //Outline
        _OutlineMode("Outline Mode", Integer) = 0
        _OutlineWidth ("Outline Width", Float ) = 5
        _OutlineWidthMap ("Outline Width Map", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0.1,0.1,0.1,1)
        _OutlineTex ("Outline Tex", 2D) = "white" {}
        _Outline_BaseColorBlend ("Blend Base Color to Outline", Range(0,1) ) = 0.5
        _Outline_LightColorBlend ("Blend Light Color to Outline", Range(0,1) ) = 0.5
        _OutlineOffsetZ ("Outline Z Offset", Float) = 0.75
        _OutlineNear ("Outline Near", Float ) = 0.5
        _OutlineFar ("Outline Far", Float ) = 100
        _Outline_UseNormalMap ("Outline: Use Outline Normal Map", Integer ) = 0
        _Outline_NormalMap ("Outline Normal Map", 2D) = "bump" {}
        
        
    }

    SubShader{
        PackageRequirements {
             "com.unity.render-pipelines.universal": "17.3.0" //Unity 6.3+
        }
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

            //USE_SHAPE_LIGHT keywords
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
                float3 positionWS  : TEXCOORD4;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _White;
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_MainTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            

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
                float _DirectionalLight_DiffuseStrength;

                float3 _DirectionalLight_ViewPosition;
                float4 _HighlightColor;
                int _DirectionalLight_HighlightMode;
                float _DirectionalLight_HighlightStrength;
                float _DirectionalLight_HighlightSize;

            CBUFFER_END

//----------------------------------------------------------------------------------------------------------------------            
            float4 _MainTex_ST;

            TEXTURE2D(_1st_ShadeMap);
            TEXTURE2D(_2nd_ShadeMap);
            
            TEXTURE2D(_HighlightTex);
            SAMPLER(sampler_HighlightTex);
            float4 _HighlightTex_ST;

            #include "ObjectTransform.hlsl"
            #include "ShapeLight2D.hlsl"

            //_HDREmulationScale declaration
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightVariables.hlsl" 
            
            Varyings ToonVertex(Attributes input) {

                Varyings o = (Varyings) 0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(input.positionOS);
                const float3 normalWS = TransformObjectToWorldDir(input.normal);
    
                o.uv = input.uv;
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);
                o.normalWS = normalWS;

                const float3 tangentWS = normalize( mul( GetObjectToWorldMatrix(), float4( input.tangent.xyz, 0 ) ).xyz); 
                o.tangentWS = float4(tangentWS, input.tangent.w);
                o.positionWS = TransformObjectToWorld(input.positionOS);
                
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


            half4 CombinedShapeLightAndToon(ShapeLightResult shapeLightResult, SurfaceData2D surfaceData,
                in float2 uv,
                in float3 tangentWS, in float3 bitangentWS, in float3 normalWS, in float3 positionWS)
            {
                const half alpha = surfaceData.alpha;
                
                float3x3 tangentTransform = float3x3( tangentWS, bitangentWS, normalWS);
                const float3 normalTS = surfaceData.normalTS;
                float3 perturbedNormalWS = normalize(mul( normalTS, tangentTransform )); // Perturbed normals

                half4 light2dMod = shapeLightResult.mod; 
                half4 light2dAdd = shapeLightResult.add; 

                const float light2dIntensity = max(
                    light2dMod.r * light2dAdd.r, max(
                        light2dMod.g + light2dAdd.g,
                        light2dMod.b + light2dAdd.b));
                
                const half4 mainTex = half4(surfaceData.albedo, alpha);
                const float3 baseAlbedo = _BaseColor.rgb * mainTex.rgb;

                //1st and 2nd Shade
                const float4 firstShadeTex = lerp(
                    SAMPLE_TEXTURE2D(_1st_ShadeMap, sampler_MainTex, TRANSFORM_TEX(uv, _MainTex)), mainTex,
                    _Use_BaseAs1st);
                const float3 firstShadeAlbedo = _1st_ShadeColor.rgb * firstShadeTex.rgb; 

                const float4 secondShadeTex = lerp(
                    SAMPLE_TEXTURE2D(_2nd_ShadeMap, sampler_MainTex, TRANSFORM_TEX(uv, _MainTex)), firstShadeTex,
                    _Use_1stAs2nd);
                const float3 secondShadeAlbedo = _2nd_ShadeColor.rgb * secondShadeTex.rgb;
                
                //perform 3 color linear shading with 2D colors and lights  
                const float3 color2D = ThreeColorsLinearShading(
                    (baseAlbedo * light2dMod + light2dAdd).rgb,
                    (firstShadeAlbedo * light2dMod + light2dAdd).rgb,
                    (secondShadeAlbedo * light2dMod + light2dAdd).rgb,
                    _BaseTo1st_ShadeStart, _BaseTo1st_ShadeFeather,
                    _1stTo2nd_ShadeStart, _1stTo2nd_ShadeFeather, light2dIntensity);
                


                //Toon Directional Light
                const float3 directionalLightColorAndUse = _DirectionalLight_Color.rgb * _DirectionalLight_Use; 
                const float3 directionalLightDirection = normalize(-_DirectionalLight_Direction);
                const float dotNL = 0.5 * dot( perturbedNormalWS, directionalLightDirection) + 0.5;

                const float3 toonDiffuseColor = ThreeColorsLinearShading(
                    baseAlbedo * directionalLightColorAndUse,
                    firstShadeAlbedo * directionalLightColorAndUse,
                    secondShadeAlbedo * directionalLightColorAndUse,
                    _BaseTo1st_ShadeStart, _BaseTo1st_ShadeFeather,
                    _1stTo2nd_ShadeStart, _1stTo2nd_ShadeFeather,
                    dotNL);

                const float3 finalDiffuseColor = color2D * _2DLightStrength +
                    toonDiffuseColor * _DirectionalLight_DiffuseStrength;
                
                //Highlight
                const float3 viewDirection = normalize(_DirectionalLight_ViewPosition - positionWS);
                const float3 halfDirection = normalize(viewDirection + directionalLightDirection);
                float dotHN_01 = 0.5 * dot(halfDirection,perturbedNormalWS) + 0.5;

                const float highlight =
                    lerp( (1.0 - step(dotHN_01,(1.0 - pow(abs(_DirectionalLight_HighlightSize),5)))),
                        pow(abs(dotHN_01),exp2(lerp(11,1,_DirectionalLight_HighlightSize))),
                        _DirectionalLight_HighlightMode );
                
                
                const float4 highlightTex = SAMPLE_TEXTURE2D(_HighlightTex, sampler_HighlightTex, TRANSFORM_TEX(uv, _HighlightTex));
                const float3 highlightAlbedo = highlightTex.rgb * _HighlightColor.rgb; 
                const float3 highlightFactor = directionalLightColorAndUse * _DirectionalLight_HighlightStrength; 
                
                const float3 finalHighlightColor = highlightAlbedo * highlightFactor * highlight;
                
                const float3 finalColor = _HDREmulationScale * (finalDiffuseColor + finalHighlightColor);

                return float4(finalColor,alpha);
            }

            
            half4 ToonFragment(Varyings input) : SV_Target {
                const half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);
                const half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _BumpScale);

                SurfaceData2D surfaceData;
  
                const float3 normalWS = normalize(input.normalWS);
                const float3 tangentWS = normalize(input.tangentWS);
                const float3 bitangentWS = normalize(cross(normalWS, tangentWS) * input.tangentWS.w);

                const float alpha = main.a;
                
                InitializeSurfaceData(main.rgb, alpha, mask, normalTS, surfaceData);

                #if defined(DEBUG_DISPLAY)
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, input.positionWS, input.positionCS, _MainTex);
                surfaceData.normalWS = input.normalWS;
                #endif

                if (alpha == 0.0)
                    discard;
                
                ShapeLightResult shapeLightResult = CombinedShapeLight(mask, input.lightingUV);

                
                return CombinedShapeLightAndToon(shapeLightResult, surfaceData, input.uv,
                    tangentWS, bitangentWS, normalWS, input.positionWS);
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
            Blend SrcAlpha OneMinusSrcAlpha
//            Stencil
//            {
//                Ref[_StencilNo]
//                Comp[_StencilComp]
//                Pass[_StencilOpPass]
//                Fail[_StencilOpFail]
//
//            }

            
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment

            #pragma multi_compile TOON_OUTLINE_NORMAL TOON_OUTLINE_POS
            // Outline is implemented in UniversalToonOutline.hlsl.
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            //USE_SHAPE_LIGHT keywords
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"

            //_HDREmulationScale declaration
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightVariables.hlsl" 
            
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
            
            TEXTURE2D(_OutlineWidthMap);
            SAMPLER(sampler_OutlineWidthMap);
            float4 _OutlineWidthMap_ST;

            TEXTURE2D(_OutlineTex);
            SAMPLER(sampler_OutlineTex);
            float4 _OutlineTex_ST;

            TEXTURE2D(_Outline_NormalMap);
            SAMPLER(sampler_Outline_NormalMap);
            float4 _Outline_NormalMap_ST;
            int    _Outline_UseNormalMap;

            CBUFFER_START(UnityPerMaterial)

                float _2DLightStrength;
                int _DirectionalLight_Use;
                float4 _DirectionalLight_Color;
                float _DirectionalLight_DiffuseStrength;
            
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
            #include "ShapeLight2D.hlsl"
            
            OutlineVertexOutput OutlineVertex(OutlineVertexInput v) {
                OutlineVertexOutput o = (OutlineVertexOutput) 0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                const float2 uv = v.texcoord0;
                o.uv0 = v.texcoord0;
                
                const float4 objPos = mul (GetObjectToWorldMatrix(), float4(0,0,0,1) );

                const float outlineWidthAlbedo = SAMPLE_TEXTURE2D_LOD(_OutlineWidthMap, sampler_OutlineWidthMap,
                    TRANSFORM_TEX(uv, _OutlineWidthMap),0).r;
                const float outlineWidth = _OutlineWidth * 0.001 * outlineWidthAlbedo;
    
                float finalOutlineWidth = outlineWidth * smoothstep( _OutlineFar, _OutlineNear, distance(objPos.rgb,_WorldSpaceCameraPos) );

				float3 newPos;
#ifdef TOON_OUTLINE_NORMAL

                //TBN
                const float3 normalDir = UnityObjectToWorldNormal(v.normal);
                const float3 tangentDir = normalize( mul( GetObjectToWorldMatrix(), float4( v.tangent.xyz, 0.0 ) ).xyz );
                const float3 bitangentDir = normalize(cross(normalDir, tangentDir) * v.tangent.w);
                float3x3 tangentTransform = float3x3(tangentDir, bitangentDir, normalDir);

                //Outline normal map
                const float4 customNormalMap = SAMPLE_TEXTURE2D_LOD(
                    _Outline_NormalMap, sampler_Outline_NormalMap, TRANSFORM_TEX(uv, _Outline_NormalMap),0);
                const float3 normalTS = UnpackNormal(customNormalMap);
                const float3 outlineNormalMapWS = normalize(mul(normalTS.xyz, tangentTransform));
                
                const float3 outlineDir = lerp(v.normal, outlineNormalMapWS, _Outline_UseNormalMap); 
                
                newPos = float4(v.vertex.xyz + outlineDir * finalOutlineWidth,1);
                o.pos = TransformObjectToHClip(newPos);
#elif TOON_OUTLINE_POS
                const float3 normalizedPos = normalize(v.vertex.xyz);
                const float signPN = sign(dot(normalizedPos,normalize(v.normal)));
                o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + signPN * normalizedPos * finalOutlineWidth, 1));
#endif
                const float scaledOutlineOffsetZ = _OutlineOffsetZ * -0.01;
                o.pos.z = o.pos.z + scaledOutlineOffsetZ;
                
                o.lightingUV = half2(ComputeScreenPos(o.pos / o.pos.w).xy);
                return o;
            }

            
            half4 OutlineFragment(OutlineVertexOutput i) : SV_Target {

                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv0);
                const half2 lightingUV = i.lightingUV;
                
                const float2 Set_UV0 = i.uv0;
                float4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex));
                float3 Set_BaseColor = _BaseColor.rgb * _MainTex_var.rgb;
                
                const float3 outlineTex = tex2D(sampler_OutlineTex,TRANSFORM_TEX(Set_UV0, _OutlineTex)).rgb;
                const float3 outlineAlbedo = outlineTex * _OutlineColor.rgb;

                //Blend with baseColor
                const float3 outlineBaseBlend = lerp(outlineAlbedo, outlineAlbedo * Set_BaseColor, _Outline_BaseColorBlend);

                //Blend with light
                ShapeLightResult shapeLightResult = CombinedShapeLight(mask, lightingUV);
                const float3 color2D = (outlineBaseBlend.rgb * shapeLightResult.mod.rgb) + shapeLightResult.add.rgb;
                const float3 colorToon = outlineBaseBlend.rgb * _DirectionalLight_Color.rgb * _DirectionalLight_Use;
                const float3 outlineLightColor = (color2D * _2DLightStrength) +
                    (colorToon * _DirectionalLight_DiffuseStrength);
                
                const float3 outlineBaseAndLightBlend = lerp(outlineBaseBlend, outlineLightColor, _Outline_LightColorBlend);
                
                return float4(_HDREmulationScale * outlineBaseAndLightBlend,1.0);
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


//[Note-sin: 2025-11-24] Texture that only needs one channel at the moment
//1. _OutlineWidthMap
