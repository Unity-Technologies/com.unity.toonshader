#if (SHADER_LIBRARY_VERSION_MAJOR >= 8)


# ifdef _ADDITIONAL_LIGHTS
#  ifndef  REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
#   define REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
#  endif
# endif
#else
# ifdef _MAIN_LIGHT_SHADOWS
//#  if !defined(_MAIN_LIGHT_SHADOWS_CASCADE)
#   ifndef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
#    define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
#   endif
//#  endif
# endif
# ifdef _ADDITIONAL_LIGHTS
#  ifndef  REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
#   define REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
#  endif
# endif
#endif

//function to rotate the UV: RotateUV()
//float2 rotatedUV = RotateUV(i.uv0, (_angular_Verocity*3.141592654), float2(0.5, 0.5), _Time.g);
float2 RotateUV(float2 _uv, float _radian, float2 _piv, float _time) {
    float RotateUV_ang = _radian;
    float RotateUV_cos = cos(_time * RotateUV_ang);
    float RotateUV_sin = sin(_time * RotateUV_ang);
    return (mul(_uv - _piv, float2x2(RotateUV_cos, -RotateUV_sin, RotateUV_sin, RotateUV_cos)) + _piv);
}

//
fixed3 DecodeLightProbe(fixed3 N) {
    return ShadeSH9(float4(N, 1));
}


inline void InitializeStandardLitSurfaceDataUTS(float2 uv, out SurfaceData outSurfaceData) {
    outSurfaceData = (SurfaceData)0;
    // half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    half4 albedoAlpha = half4(1.0, 1.0, 1.0, 1.0);

    outSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);

    half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a);
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

#if _SPECULAR_SETUP
    outSurfaceData.metallic = 1.0h;
    outSurfaceData.specular = specGloss.rgb;
#else
    outSurfaceData.metallic = specGloss.r;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
#endif

    outSurfaceData.smoothness = specGloss.a;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    outSurfaceData.occlusion = SampleOcclusion(uv);
    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
}

half3 GlobalIlluminationUTS_Deprecated_Deprecated(BRDFData brdfData, half3 bakedGI, half occlusion, half3 normalWS,
                                                  half3 viewDirectionWS) {
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half fresnelTerm = Pow4(1.0 - saturate(dot(normalWS, viewDirectionWS)));

    half3 indirectDiffuse = bakedGI * occlusion;
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, occlusion);

    return EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);
}

half3 GlobalIlluminationUTS(BRDFData brdfData, half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS,
                            float3 positionWS, float2 normalizedScreenSpaceUV) {
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half fresnelTerm = Pow4(1.0 - saturate(dot(normalWS, viewDirectionWS)));

    half3 indirectDiffuse = bakedGI * occlusion;
#if USE_FORWARD_PLUS || USE_CLUSTER_LIGHT_LOOP
    half3 irradiance = CalculateIrradianceFromReflectionProbes(reflectVector, positionWS, brdfData.perceptualRoughness,
        normalizedScreenSpaceUV);
    half3 indirectSpecular = irradiance * occlusion;
#else
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, occlusion);
#endif
    return EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);
}

void ApplyDecalToSurfaceDataUTS(float4 positionCS, inout float3 albedo, inout SurfaceData surfaceData,
                                inout float3 normalWS) {

#if defined(_DBUFFER)

#ifdef _SPECULAR_SETUP
half metallic = 0;
ApplyDecal(positionCS,
           albedo,
           surfaceData.specular,
           normalWS,
           metallic,
           surfaceData.occlusion,
           surfaceData.smoothness);
#else
half3 specular = 0;
ApplyDecal(positionCS,
           albedo,
           specular,
           normalWS,
           surfaceData.metallic,
           surfaceData.occlusion,
           surfaceData.smoothness);
#endif
#endif
}

struct VertexInput {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 texcoord0 : TEXCOORD0;

#if defined(_IS_ANGELRING_ON)
    float2 texcoord1 : TEXCOORD1;
#endif

    float2 lightmapUV : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput {
    float4 pos : SV_POSITION;
    float2 uv0 : TEXCOORD0;

#if defined(_IS_ANGELRING_ON)
    float2 uv1 : TEXCOORD1;
#endif
    
    float4 posWorld : TEXCOORD2;
    float3 normalDir : TEXCOORD3;
    float3 tangentDir : TEXCOORD4;
    float3 bitangentDir : TEXCOORD5;
    float mirrorFlag : TEXCOORD6;

    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 7);

#  ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    float4 shadowCoord : TEXCOORD9;
#  endif

    float4 positionCS : TEXCOORD10;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

///////////////////////////////////////////////////////////////////////////////
//                      Light Abstraction                                    //
/////////////////////////////////////////////////////////////////////////////

half AdditionalLightRealtimeShadowUTS(int lightIndex, float3 positionWS) {
#if defined(ADDITIONAL_LIGHT_CALCULATE_SHADOWS)


    ShadowSamplingData shadowSamplingData = GetAdditionalLightShadowSamplingData(lightIndex);

#if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    lightIndex = _AdditionalShadowsIndices[lightIndex];

    // We have to branch here as otherwise we would sample buffer with lightIndex == -1.
    // However this should be ok for platforms that store light in SSBO.
    UNITY_BRANCH if (lightIndex < 0)
        return 1.0;

    float4 shadowCoord = mul(_AdditionalShadowsBuffer[lightIndex].worldToShadowMatrix, float4(positionWS, 1.0));
#else
    float4 shadowCoord = mul(_AdditionalLightsWorldToShadow[lightIndex], float4(positionWS, 1.0));
#endif

    half4 shadowParams = GetAdditionalLightShadowParams(lightIndex);
    return SampleShadowmap(TEXTURE2D_ARGS(_AdditionalLightsShadowmapTexture, sampler_AdditionalLightsShadowmapTexture),
        shadowCoord, shadowSamplingData, shadowParams, true);
#else
    return 1.0h;
#endif
}

half3 GetLightColor(Light light
#ifdef _LIGHT_LAYERS
    , uint meshRenderingLayers
#endif
) {
    half3 lightColor = 0;
#ifdef _LIGHT_LAYERS
    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif
    {
        lightColor = light.color * light.distanceAttenuation;
    }
    return lightColor;
}


float4 GetShadowCoordUTS(VertexOutput v)
{
#if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
    return ComputeScreenPos(v.positionCS);
#else
    return TransformWorldToShadowCoord(v.posWorld);
#endif
}

inline float TweakShadow(float shadow) {
    return saturate((shadow * 0.5) + 0.5 + _Tweak_SystemShadowsLevel);
}

//----------------------------------------------------------------------------------------------------------------------

VertexOutput vert(VertexInput v) {
    VertexOutput o = (VertexOutput)0;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.uv0 = v.texcoord0;
#if defined(_IS_ANGELRING_ON)
    o.uv1 = v.texcoord1;
#endif
    o.normalDir = UnityObjectToWorldNormal(v.normal);
    o.tangentDir = normalize(mul(GetObjectToWorldMatrix(), float4(v.tangent.xyz, 0.0)).xyz);
    o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
    o.posWorld = mul(GetObjectToWorldMatrix(), v.vertex);

    o.pos = UnityObjectToClipPos(v.vertex);
    //Detection of the inside the mirror (right or left-handed) o.mirrorFlag = -1 then "inside the mirror".

    float3 crossFwd = cross(UNITY_MATRIX_V[0].xyz, UNITY_MATRIX_V[1].xyz);
    o.mirrorFlag = dot(crossFwd, UNITY_MATRIX_V[2].xyz) < 0 ? 1 : -1;

    //
    float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
    float4 positionCS = TransformWorldToHClip(positionWS);

    OUTPUT_LIGHTMAP_UV(v.lightmapUV, unity_LightmapST, o.lightmapUV);
    
    // https://github.com/Unity-Technologies/Graphics/commit/74b1fdc26cee492e8af7358116076806bdf5b4cc
    float4 probeOcclusionUnused;
    OUTPUT_SH4(positionWS, o.normalDir.xyz, GetWorldSpaceNormalizeViewDir(positionWS), o.vertexSH,
        probeOcclusionUnused);
    o.positionCS = positionCS;

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    o.shadowCoord = GetShadowCoordUTS(o);
#endif
    
    return o;
}

#if UNITY_VERSION >= 60030000

#define ENCODE_MESH_RENDERING_LAYER_UTS EncodeMeshRenderingLayer()

#else

#define ENCODE_MESH_RENDERING_LAYER_UTS EncodeMeshRenderingLayer(GetMeshRenderingLayer())

#ifndef CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK
#define CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
#endif //CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK

#endif

//----------------------------------------------------------------------------------------------------------------------

//the actual fragment shaders
#if defined(_SHADINGGRADEMAP)
#include "UniversalToonBodyShadingGradeMap.hlsl"
#else //#if defined(_SHADINGGRADEMAP)
#include "UniversalToonBodyDoubleShadeWithFeather.hlsl"
#endif //#if defined(_SHADINGGRADEMAP)

