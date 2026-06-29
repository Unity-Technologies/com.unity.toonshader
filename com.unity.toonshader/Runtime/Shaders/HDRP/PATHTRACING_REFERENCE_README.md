# Minimal Path Tracing Reference Shader

This is a **reference implementation** showing the minimum structure needed for a shader to work with HDRP Path Tracing.

## ⚠️ Important Notes

- This shader **ONLY** works with path tracing enabled
- It will appear **black** in normal rendering (forward/deferred)
- This is intentionally minimal to show the core structure
- For production, you'd need forward/GBuffer passes too

## File Structure

```
MinimalPathTracingReference.shader   <- Main shader file with PathTracingDXR pass
MinimalPathTracingImpl.hlsl          <- Implementation with required functions
```

## How to Test

1. Open Unity with this project
2. Create a new material using shader "Reference/MinimalPathTracing"
3. Assign to a GameObject
4. Enable Path Tracing in HDRP settings:
   - Edit > Project Settings > HDRP Global Settings > Frame Settings
   - Enable "Ray Tracing"
   - Camera component > Enable "Path Tracing"
5. Material should now render with path-traced lighting

## Required Components

### 1. Shader Pass Structure

```hlsl
Pass
{
    Name "PathTracingDXR"
    Tags { "LightMode" = "PathTracingDXR" }
    
    #pragma raytracing surface_shader  // ← Key: tells Unity this is raytracing
    #define SHADERPASS SHADERPASS_PATH_TRACING
    
    // Include path tracing integrator
    #include "ShaderPassPathTracing.hlsl"
}
```

### 2. Three Required Functions

#### `GetSurfaceAndBuiltinData()`
Called when ray hits surface. Returns material properties:
- Albedo color
- Roughness/smoothness
- Normal
- Alpha/opacity

```hlsl
void GetSurfaceAndBuiltinData(..., out SurfaceData surfaceData, out BuiltinData builtinData)
{
    surfaceData.baseColor = SAMPLE_TEXTURE2D(...);
    surfaceData.perceptualSmoothness = 1.0 - _Roughness;
    // etc...
}
```

#### `ProcessBSDFData()`
Adjusts properties for path tracing (optional modifications):
- Clamp roughness to avoid fireflies
- Calculate energy compensation
- Fresnel calculations

```hlsl
void ProcessBSDFData(..., inout BSDFData bsdfData)
{
    bsdfData.roughnessT = max(payload.maxRoughness, bsdfData.roughnessT);
}
```

#### `CreateMaterialData()` ⭐ **Most Important**
Sets up BSDF (Bidirectional Scattering Distribution Function):
- Defines how light bounces off the surface
- Sets weights for diffuse/specular/transmission
- Returns `false` if material absorbs all light (black)

```hlsl
bool CreateMaterialData(..., out MaterialData mtlData)
{
    // Set up BSDF weights
    mtlData.bsdfWeight[0] = diffuseWeight;   // Lambertian diffuse
    mtlData.bsdfWeight[1] = coatWeight;      // Clear coat
    mtlData.bsdfWeight[2] = specularWeight;  // Specular reflection (GGX)
    mtlData.bsdfWeight[3] = transmitWeight;  // Transmission/refraction
    
    // Must normalize weights to sum to 1.0
    mtlData.bsdfWeight /= totalWeight;
    
    return true;
}
```

## Understanding BSDF Weights

The `bsdfWeight` array controls material appearance:

| Weight | Component | Effect |
|--------|-----------|--------|
| `[0]` | Diffuse | Matte, clay-like scattering (Lambertian) |
| `[1]` | Coat | Clear coat layer (car paint) |
| `[2]` | Specular Reflection | Glossy reflections (GGX microfacet) |
| `[3]` | Transmission | See-through glass/water (refraction) |

**Example configurations:**

```hlsl
// Pure diffuse (clay/matte)
bsdfWeight = float4(1.0, 0.0, 0.0, 0.0);

// Plastic (diffuse + specular)
bsdfWeight = float4(0.7, 0.0, 0.3, 0.0);

// Metallic (only specular)
bsdfWeight = float4(0.0, 0.0, 1.0, 0.0);

// Glass (specular reflection + transmission)
bsdfWeight = float4(0.0, 0.0, 0.3, 0.7);
```

## How Path Tracing Works

1. **Ray hits surface** → Unity calls `GetSurfaceAndBuiltinData()`
2. **Material setup** → Calls `ProcessBSDFData()` and `CreateMaterialData()`
3. **BSDF sampling** → Path tracer uses weights to decide:
   - Should light bounce diffusely? (weight[0])
   - Should it reflect glossily? (weight[2])
   - Should it transmit through? (weight[3])
4. **Recursive tracing** → Traces new rays based on BSDF choice
5. **Accumulation** → Repeats many times, accumulates final color

## Adapting for UTS Toon Shader

To add path tracing to UTS, you would:

### 1. Reuse existing material evaluation
```hlsl
// ✅ Can reuse from UTS forward pass:
float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, ...);
float3 baseColor = mainTex.rgb * _BaseColor.rgb;
float3 normalWS = GetNormalFromMap(...);
```

### 2. Map toon properties to BSDF
```hlsl
// ❌ Cannot reuse: Toon lighting calculations
// Instead, approximate as physically-based:

// Option A: Simple diffuse
bsdfWeight[0] = 1.0;

// Option B: Approximate with roughness
bsdfWeight[0] = 0.7; // Diffuse from base/shade colors
bsdfWeight[2] = 0.3; // Specular from highlight settings
roughness = lerp(0.5, 0.9, _ShadeSmoothness);
```

### 3. Handle non-PBR features
```hlsl
// Toon features that don't translate:
// - Stepped shading → Use gradient as roughness variation
// - Rim lights → Ignore (view-dependent effects don't work)
// - MatCap → Ignore (screen-space effect)
// - Outlines → Separate geometry pass (not in material BSDF)
```

## Key Differences: Forward vs PathTracing

| Aspect | Forward Pass | PathTracing Pass |
|--------|-------------|------------------|
| **Entry point** | `Frag()` fragment shader | `ClosestHit()` ray hit shader |
| **Output** | Final RGB color | Material properties (BSDF) |
| **Lighting** | Explicit light loop | Automatic (recursive rays) |
| **Shadows** | Shadow maps | Ray-traced |
| **GI** | Probes/lightmaps | Automatic (bounced rays) |
| **Reflection** | Reflection probes | Automatic (reflected rays) |
| **Transparency** | Alpha blending | Physical refraction |

## Limitations for Toon Shading

Path tracing fundamentally conflicts with toon aesthetics:

❌ **Hard-edged shadows** → Path tracing produces soft, physically accurate shadows  
❌ **Stepped shading** → Path tracing produces smooth gradients  
❌ **Rim lights** → View-dependent effects don't work in path tracing  
❌ **MatCap** → Screen-space effects incompatible  
❌ **Artistic control** → Path tracing follows physics, not artistic intent  

## Recommended Approach

For UTS path tracing support:

1. **Phase 1**: Implement basic diffuse (like this reference)
   - Materials won't be black ✅
   - Will look flat/diffuse ⚠️

2. **Phase 2**: Add specular approximation
   - Map highlight settings to specular weight
   - Use fixed roughness or derive from shade settings

3. **Phase 3**: Advanced (optional)
   - Experiment with custom BSDF
   - May produce artifacts ⚠️
   - Requires deep path tracing knowledge

## References

- Unity HDRP Lit shader: `Lit.shader` + `LitPathTracing.hlsl`
- Unity HDRP Unlit shader: `Unlit.shader` (simplest example)
- Path tracing integrator: `PathTracingIntegrator.hlsl`
- BSDF utilities: `PathTracingBSDF.hlsl`

## Testing Checklist

- [ ] Material not black under path tracing
- [ ] Responds to lighting (point/directional lights)
- [ ] Shadows appear correctly
- [ ] Reflections on smooth surfaces
- [ ] GI bounces affect nearby objects
- [ ] No black artifacts (fireflies are expected with low sample count)

---

**Created as reference for adding path tracing support to Unity Toon Shader (UTS)**
