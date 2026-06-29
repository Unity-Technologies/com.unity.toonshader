Thank you for the feedback and feature suggestion! We appreciate you taking the time to bring this to our attention.

Path tracing support is an interesting request, though there are some important technical considerations to be aware of. Unity Toon Shader is fundamentally designed for **stylized, non-photorealistic rendering** with features like cel-shaded steps, rim lights, and MatCap effects. Path tracing, on the other hand, is built for **physically-based photorealistic rendering** with smooth gradients and accurate light transport.

## Why Toon Shading and Path Tracing Are Fundamentally Incompatible

### Different Rendering Paradigms

**Forward/Deferred Rendering (Current UTS):**
```hlsl
float4 frag(VertexOutput input) : SV_Target
{
    // YOU calculate final color directly in the fragment shader
    float3 baseColor = tex2D(_MainTex, uv);
    float toonStep = step(0.5, NdotL);  // Hard-edged cel shading
    float3 finalColor = baseColor * lightColor * toonStep + rimLight + matCap;
    
    return float4(finalColor, 1.0);  // Return RGB color to screen
}
```

**Path Tracing:**
```hlsl
bool CreateMaterialData(...)
{
    // You DON'T calculate color - you describe how light bounces
    mtlData.baseColor = tex2D(_MainTex, uv);
    mtlData.bsdfWeight[0] = 0.7;  // 70% diffuse bounce
    mtlData.bsdfWeight[2] = 0.3;  // 30% specular bounce
    
    // Path tracer traces recursive rays and accumulates color through:
    // - Direct light sampling (shadow rays to lights)
    // - Material sampling (bounced rays for GI)
    // - Multiple bounces with Russian roulette termination
    return true;
}
```

The fundamental difference:
- **Forward rendering**: You explicitly calculate and return final lit color
- **Path tracing**: You return material properties (BSDF); the integrator calculates color through recursive ray tracing

### The Path Tracing Call Chain

When a ray hits a surface in path tracing:

```
DXR Ray Hit
    ↓
[shader("closesthit")] ClosestHit  (PathTracingIntegrator.hlsl)
    ↓ Calls GetSurfaceInfo
    ↓
GetSurfaceInfo (PathTracingSurface.hlsl)
    ├─→ GetSurfaceAndBuiltinData()  ⭐ YOU IMPLEMENT
    │   - Sample textures (_MainTex, _NormalMap)
    │   - Return albedo, normal, roughness, alpha
    │   - Return emissive color
    │
    └─→ ConvertSurfaceDataToBSDFData()  (Unity provides)
        - Converts SurfaceData → BSDFData
        - Calculates fresnel, roughness values
    ↓
ComputeSurfaceScattering (PathTracingSurface.hlsl)
    ├─→ CreateMaterialData()  ⭐ YOU IMPLEMENT
    │   └─→ ProcessBSDFData()  ⭐ YOU IMPLEMENT
    │       - Clamp roughness to avoid fireflies
    │       - Energy compensation
    │   - Returns BSDF weights (diffuse/specular/transmission/coat)
    │
    ├─→ EvaluateMaterial()  ⭐ YOU IMPLEMENT
    │   - Evaluates BSDF for a given direction (toward light)
    │   - Returns MaterialResult:
    │       • diffValue: diffuse reflection color
    │       • specValue: specular reflection color
    │       • diffPdf, specPdf: probability densities
    │
    └─→ SampleMaterial()  ⭐ YOU IMPLEMENT
        - Samples random bounce direction based on BSDF
        - Returns MaterialResult for that direction
        - Path tracer traces new ray → recursive
```

### Where Color Is Actually Calculated

Color accumulates **recursively** through the path tracing integrator:

```hlsl
// Start with emissive
payload.value = builtinData.emissiveColor;

// Add direct lighting (explicit light sampling)
if (SampleLights(..., lightDir, lightColor, ...))
{
    EvaluateMaterial(YOUR_material, lightDir, result);
    payload.value += lightColor × (result.diffValue + result.specValue);
}

// Continue path with indirect lighting (material sampling)
if (SampleMaterial(YOUR_material, random, bounceDir, result))
{
    payload.throughput *= (result.diffValue + result.specValue);
    TraceRay(..., bounceDir, ...);  // Recursive call → more bounces
}

// After N bounces: payload.value = final accumulated color
```

You **never** directly calculate the final color - the integrator does it through:
1. Sampling lights and tracing shadow rays
2. Evaluating YOUR BSDF toward those lights
3. Sampling YOUR BSDF for bounce directions
4. Recursively tracing rays until max depth or Russian roulette termination
5. Accumulating contributions at each bounce

### Why Toon Features Don't Work

**Cel-shaded steps:**
```hlsl
// Forward: YOU control this
float toonStep = step(0.5, NdotL);  // Hard edge at 0.5
```
Path tracing: Integrator samples light from ALL directions and smoothly accumulates → cannot produce hard edges

**Rim lights:**
```hlsl
// Forward: View-dependent screen-space effect
float rim = pow(1.0 - dot(normal, viewDir), _RimPower);
```
Path tracing: Rays come from arbitrary directions, not just camera → view-dependent effects don't make sense

**MatCap:**
```hlsl
// Forward: Screen-space UV mapping
float2 matcapUV = viewNormal.xy * 0.5 + 0.5;
```
Path tracing: Screen space doesn't exist in recursive ray tracing → incompatible

### What Implementation Would Look Like

To add basic path tracing support, we'd need to implement:

**1. Add PathTracingDXR pass to shader:**
```hlsl
Pass
{
    Name "PathTracingDXR"
    Tags { "LightMode" = "PathTracingDXR" }
    
    #pragma raytracing surface_shader
    
    // Include HDRP path tracing headers
    #include "ToonPathTracing.hlsl"
    #include "ShaderPassPathTracing.hlsl"
}
```

**2. Implement required functions in `ToonPathTracing.hlsl`:**

```hlsl
// Sample textures and return material properties
void GetSurfaceAndBuiltinData(FragInputs input, float3 V, ...)
{
    float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    surfaceData.baseColor = mainTex.rgb × _BaseColor.rgb;
    surfaceData.normalWS = GetNormalFromMap(...);
    builtinData.emissiveColor = _EmissiveColor;  // Can use toon emissive
}

// Set up BSDF weights (how light bounces)
bool CreateMaterialData(PathPayload payload, BuiltinData builtinData, 
                       BSDFData bsdfData, out MaterialData mtlData)
{
    // For toon → just treat as diffuse
    mtlData.bsdfWeight[0] = 1.0;  // 100% Lambertian diffuse
    mtlData.baseColor = bsdfData.diffuseColor;  // From _MainTex × _BaseColor
    
    // CANNOT use: toon steps, rim lights, MatCap, highlights
    // These don't map to BSDF model
    
    return true;
}

// Evaluate BSDF toward a specific direction (e.g., light)
void EvaluateMaterial(MaterialData mtlData, float3 sampleDir, 
                     out MaterialResult result)
{
    // Use Unity's Lambert BRDF evaluation
    BRDF::EvaluateLambert(mtlData, normal, sampleDir, 
                          result.diffValue, result.diffPdf);
    result.diffValue *= mtlData.baseColor;  // Scale by toon texture color
}

// Sample random bounce direction based on BSDF
bool SampleMaterial(MaterialData mtlData, float3 inputSample, 
                   out float3 sampleDir, out MaterialResult result)
{
    // Use Unity's Lambert BRDF sampling (cosine-weighted hemisphere)
    BRDF::SampleLambert(mtlData, normal, inputSample, sampleDir,
                        result.diffValue, result.diffPdf);
    result.diffValue *= mtlData.baseColor;
    return true;
}
```

**Result:**
- Materials render as flat diffuse under path tracing
- Base texture color is preserved
- Benefits from path-traced GI, reflections, shadows
- **All toon features disabled** (cel shading, rim, MatCap, etc.)

## What We Can Offer

We understand that preventing materials from appearing **black** when path tracing is enabled would be valuable. We could potentially add a basic **fallback PathTracingDXR pass** that treats toon materials as simple diffuse surfaces under path tracing. This would:

- ✅ Prevent black materials when path tracing is enabled
- ✅ Allow materials to receive path-traced lighting (GI, reflections, shadows)  
- ✅ Preserve base texture colors and emissive
- ⚠️ Lose all toon aesthetic features (cel shading, rim lights, MatCap, highlights)
- ⚠️ Materials render as flat diffuse (Lambertian) under path tracing

The implementation effort would be approximately **1-2 weeks** for a basic diffuse fallback.

## Questions for Clarification

1. **Is your use case specifically about preventing black materials (basic fallback)?** Or are you hoping to preserve toon shading features under path tracing (not technically feasible)?

2. **Are you looking for path-traced indirect lighting (GI/reflections) combined with stylized direct lighting?** This would require custom renderer modifications beyond shader support - essentially a hybrid renderer.

3. **Could you share more details about the anime games you mentioned using path tracing?** Understanding their approach might help us better address your needs. Our suspicion is they're either:
   - Using stylized texture maps with realistic lighting (not actual toon shading)
   - Using path tracing only for specific effects (reflections/GI) with custom integration

4. **What's your actual workflow/use case?** Are you:
   - Accidentally enabling path tracing and want materials to not break?
   - Intentionally using path tracing for specific effects?
   - Trying to achieve a specific visual style (if so, example images would help)?

We're open to implementing a basic fallback if there's sufficient interest from the community, but want to set clear expectations about what's technically feasible given the fundamental differences between toon shading and path tracing.
