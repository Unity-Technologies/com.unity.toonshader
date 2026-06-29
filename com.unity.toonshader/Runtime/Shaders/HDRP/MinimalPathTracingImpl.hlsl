// Minimal Path Tracing Implementation
// This file contains the minimum required functions for path tracing support
// All necessary includes are done in the main shader file before this

#ifndef MINIMAL_PATH_TRACING_IMPL_INCLUDED
#define MINIMAL_PATH_TRACING_IMPL_INCLUDED

// Include path tracing utilities (payload, material, BSDF)
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/PathTracing/Shaders/PathTracingPayload.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/PathTracing/Shaders/PathTracingMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/PathTracing/Shaders/PathTracingBSDF.hlsl"

// ============================================================================
// Material Properties
// ============================================================================

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    float4 _BaseColor;
    float _Roughness;
CBUFFER_END

// ============================================================================
// Required Function 1: GetSurfaceAndBuiltinData
// Called by path tracer to get material properties at intersection point
// ============================================================================

void GetSurfaceAndBuiltinData(
    FragInputs input,
    float3 V,
    inout PositionInputs posInput,
    out SurfaceData surfaceData,
    out BuiltinData builtinData)
{
    // Get UV coordinates
    float2 uv = input.texCoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;

    // Sample textures
    float4 baseMapSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    float3 albedo = baseMapSample.rgb * _BaseColor.rgb;
    float alpha = baseMapSample.a * _BaseColor.a;

    // Initialize surface data (standard HDRP structure)
    surfaceData = (SurfaceData)0;
    surfaceData.baseColor = albedo;
    surfaceData.perceptualSmoothness = 1.0 - _Roughness;
    surfaceData.ambientOcclusion = 1.0;
    surfaceData.metallic = 0.0;
    surfaceData.specularOcclusion = 1.0;

    // Normal (use geometric normal for simplicity)
    surfaceData.normalWS = input.tangentToWorld[2];
    surfaceData.geomNormalWS = input.tangentToWorld[2];

    // Alpha
    #ifdef _ALPHATEST_ON
        surfaceData.alpha = alpha;
    #else
        surfaceData.alpha = 1.0;
    #endif

    // Initialize builtin data
    builtinData = (BuiltinData)0;
    builtinData.opacity = alpha;
    builtinData.emissiveColor = float3(0, 0, 0);
}

// ============================================================================
// Required Function 2: ProcessBSDFData
// Adjusts material properties for path tracing (roughness clamping, etc.)
// ============================================================================

void ProcessBSDFData(PathPayload payload, BuiltinData builtinData, MaterialData mtlData, inout BSDFData bsdfData)
{
    // Clamp roughness to avoid fireflies (bright pixel artifacts)
    bsdfData.roughnessT = max(payload.maxRoughness, bsdfData.roughnessT);
    bsdfData.roughnessB = max(payload.maxRoughness, bsdfData.roughnessB);

    // Calculate Fresnel for specular reflections
    float NdotV = abs(dot(bsdfData.normalWS, mtlData.V));

    // Simple energy compensation for GGX specular
    bsdfData.specularOcclusion = 0.0;
}

// ============================================================================
// Required Function 3: CreateMaterialData
// Main function: sets up BSDF weights and material behavior for path tracing
// ============================================================================

bool CreateMaterialData(
    PathPayload payload,
    BuiltinData builtinData,
    BSDFData bsdfData,
    inout float3 shadingPosition,
    inout float theSample,
    out MaterialData mtlData)
{
    // Initialize material data
    mtlData.V = -WorldRayDirection();  // View direction (ray direction)
    mtlData.Nv = bsdfData.normalWS;    // Shading normal
    mtlData.bsdfData = bsdfData;

    // Process BSDF data (roughness clamping, etc.)
    ProcessBSDFData(payload, builtinData, mtlData, mtlData.bsdfData);

    // ========================================================================
    // BSDF Weight Setup - This is the key part!
    // ========================================================================
    // We have 4 possible BSDF lobes (components):
    // bsdfWeight[0] = Diffuse (Lambertian)
    // bsdfWeight[1] = Coat (clear coat layer)
    // bsdfWeight[2] = Specular reflection (GGX)
    // bsdfWeight[3] = Specular transmission (GGX refraction)

    mtlData.bsdfWeight = 0.0;

    // For minimal diffuse material, we only use diffuse lobe
    float NdotV = dot(bsdfData.normalWS, mtlData.V);
    float fresnel = F_Schlick(0.04, NdotV); // Assume F0 = 0.04 (dielectric)

    // Diffuse: most of the light scatters diffusely
    mtlData.bsdfWeight[0] = (1.0 - fresnel) * Luminance(bsdfData.diffuseColor);

    // Specular: small amount of specular reflection
    mtlData.bsdfWeight[2] = fresnel;

    // Normalize weights
    float totalWeight = mtlData.bsdfWeight[0] + mtlData.bsdfWeight[2];

    if (totalWeight < BSDF_WEIGHT_EPSILON)
        return false;  // Material absorbs all light

    mtlData.bsdfWeight /= totalWeight;

    // No subsurface scattering for this minimal shader
    mtlData.isSubsurface = false;

    return true;
}


// ============================================================================
// Optional: GetMaterialAbsorption
// Controls volumetric absorption (for transparent materials)
// ============================================================================

float3 GetMaterialAbsorption(MaterialData mtlData, float dist, out bool interacted)
{
    interacted = false;
    return 1.0;  // No absorption for this minimal shader
}

#endif // MINIMAL_PATH_TRACING_IMPL_INCLUDED
