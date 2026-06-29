# Rendering Layer Bug Investigation

## Bug Report

**Issue:** Directional Lights impact Sprites/GameObjects they should not impact when the GameObjects are on different Rendering Layers

### How to reproduce:
1. Open the "RenderLayerForSprite.scene"
2. Select the "Directional Light 2" 
3. Change the Intensity value between 0.5 and 1.5 in the Light Component in the Inspector
4. Observe the Scene view

### Expected results: 
The "GO Layer 1" GameObject lighting level remains the same, while the "GO Layer 2" GameObject lighting level changes

### Actual results: 
The "GO Layer 1" GameObject lighting level switches with the "GO Layer 2" GameObject whenever the light intensity value is higher than the intensity on the "Directional Light 1"

### Clarified Behavior:
When Light 2's intensity becomes higher than Light 1:
* Object 2, which is assigned to Light 2, gets the correct lighting (blue)
* **BUT**, Object 1, which is assigned to Light 1, goes **BLACK**

And vice versa when Light 1's intensity becomes higher than Light 2

---

## Investigation Summary

### Related PR
PR #485 (https://github.com/Unity-Technologies/com.unity.toonshader/pull/485/changes) previously fixed this but the code has changed significantly since then.

Key changes from PR #485:
- Changed `#pragma multi_compile_fragment _ _LIGHT_LAYERS` to `#pragma multi_compile _ _LIGHT_LAYERS`
- Added `asuint()` conversion for layer mask access
- Enhanced light selection logic to prioritize lights matching rendering layers
- Made `meshRenderingLayers` const

---

## Attempted Fixes

### Fix Attempt #1: Pragma Directive
**Problem Identified:** Shader was using `#pragma multi_compile_fragment _ _LIGHT_LAYERS` which only defines `_LIGHT_LAYERS` in fragment shader.

**Fix Applied:** Changed to `#pragma multi_compile _ _LIGHT_LAYERS` (matching URP's standard and PR #485)

**Files Modified:**
- `UnityToon.shader:1115`
- `UnityToonTessellation.shader:1253`
- `UnityToon.shadertemplate:675`
- `UnityToonTessellation.shadertemplate:797`

**Result:** Still not working

---

### Fix Attempt #2: Fallback Lighting Bypass
**Problem Identified:** Even when `GetLightColor()` returned 0 for non-matching layers, the shader used fallback lighting via:
```hlsl
float3 lightColor = max(defaultLightColor, originalLightColor);
```

**Fix Applied:** Added explicit layer matching check before using main light:
```hlsl
bool useMainLight = true;
#ifdef _LIGHT_LAYERS
    useMainLight = IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers);
#endif

float3 lightColor = useMainLight ?
    lerp(max(defaultLightColor, originalLightColor), ...) :
    defaultLightColor;
```

**Files Modified:**
- `UniversalToonBodyDoubleShadeWithFeather.hlsl:172-203`
- `UniversalToonBodyShadingGradeMap.hlsl:187-218`

**Result:** Still not working

---

### Fix Attempt #3: Forward+ notDirectional Flag
**Problem Identified:** In Forward+ rendering mode, directional lights in the additional lights loop were incorrectly treated as point lights (`notDirectional = 1.0f` instead of `0.0f`).

**Fix Applied:** Changed `notDirectional` from `1.0f` to `0.0f` in the Forward+ directional lights loop

**Files Modified:**
- `UniversalToonBodyDoubleShadeWithFeather.hlsl:359`
- `UniversalToonBodyShadingGradeMap.hlsl:436`

**Result:** Still not working (user not using Forward+)

---

### Fix Attempt #4: Additional Light Type Detection
**Problem Identified:** Regular additional lights loop hardcoded `notDirectional = 1.0f`, treating all additional lights as point lights including additional directional lights.

**Fix Applied:** Dynamic detection based on `distanceAttenuation`:
```hlsl
// Directional lights have distanceAttenuation = 1.0, point/spot lights have < 1.0
float notDirectional = additionalLight.distanceAttenuation < 0.99 ? 1.0f : 0.0f;
```

**Files Modified:**
- `UniversalToonBodyDoubleShadeWithFeather.hlsl:409-414`
- `UniversalToonBodyShadingGradeMap.hlsl:490-495`

**Result:** Still not working

---

### Fix Attempt #5: Main Light Shadow Filtering
**Problem Identified:** Shader was using main light's shadow even when it didn't match the rendering layer.

**Fix Applied:** Only apply main light shadows when layer matches:
```hlsl
bool useMainLight = true;
#ifdef _LIGHT_LAYERS
    useMainLight = IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers);
#endif

float shadowAttenuation = 1.0;
#if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)
    shadowAttenuation = useMainLight ? mainLight.shadowAttenuation : 1.0;
#endif
```

**Files Modified:**
- `UniversalToonBodyDoubleShadeWithFeather.hlsl:162-185`
- `UniversalToonBodyShadingGradeMap.hlsl:173-200`

**Result:** Still not working

---

### Fix Attempt #6: Additional Directional Light Intensity
**Problem Identified:** When Light 2 becomes brighter and becomes the "main light", Light 1 moves to additional lights. Object 1 goes BLACK because:
- Object 1 correctly rejects Light 2 (wrong layer)
- Light 1 is in additional lights array
- But code was setting `lightIntensity = 0` for ALL additional directional lights
- Legacy comment: "If Added lights is directional, set 0 as _LightIntensity"

This was based on old Unity behavior where directional lights were NEVER in the additional lights array.

**Fix Applied:** Changed intensity calculation for additional lights:
```hlsl
// Old code:
float _LightIntensity = lerp(0, Intensity(additionalLightColor), notDirectional);

// New code:
float lightIntensity = notDirectional ? Intensity(additionalLightColor) : 1.0;
```

Now additional directional lights get `lightIntensity = 1.0` (like main directional light), while point/spot lights get their actual intensity for distance falloff.

**Files Modified:**
- `UniversalToonBodyDoubleShadeWithFeather.hlsl` (both Forward+ and regular paths)
- `UniversalToonBodyShadingGradeMap.hlsl` (both Forward+ and regular paths)

**Result:** Still not working

---

## Current Status

Despite all fixes, the bug persists. Objects still go BLACK when they should be lit by their matching directional light in the additional lights array.

## Key Technical Findings

1. **URP's GetMainLight()** selects the "main light" based on intensity, NOT based on rendering layer matching
2. **Additional directional lights** can exist in the additional lights array (confirmed by URP code comments)
3. **Light detection:** Directional lights have `lightPositionWS.w = 0.0` and `distanceAttenuation = 1.0`
4. **GetAdditionalLightsCount()** returns `min(_AdditionalLightsCount.x, unity_LightData.y)` which should include additional directional lights

## Potential Issues to Investigate

1. **Does the second directional light actually make it into the additional lights buffer?**
   - Need to verify with Frame Debugger or RenderDoc
   - Check URP Asset settings for "Additional Lights"
   
2. **Is there a limitation in URP that only ONE directional light is supported?**
   - Standard URP may not support multiple directional lights properly
   
3. **Is the additional lights loop actually processing the second directional light?**
   - Need debug output to verify loop execution
   - Check if `GetAdditionalLightsCount()` returns > 0
   
4. **Are we accidentally skipping the second directional light in the loop?**
   - May need to check for and skip the main light index in additional lights

## Files Modified (Summary)

### Shader Files
- `com.unity.toonshader/Runtime/Shaders/UnityToon.shader`
- `com.unity.toonshader/Runtime/Shaders/UnityToonTessellation.shader`
- `com.unity.toonshader/Runtime/Shaders/Common/Parts/UnityToon.shadertemplate`
- `com.unity.toonshader/Runtime/Shaders/Common/Parts/UnityToonTessellation.shadertemplate`

### HLSL Files
- `com.unity.toonshader/Runtime/Shaders/URP/UniversalToonBodyDoubleShadeWithFeather.hlsl`
- `com.unity.toonshader/Runtime/Shaders/URP/UniversalToonBodyShadingGradeMap.hlsl`

## Next Steps

1. Verify that both directional lights are actually in the lighting data (Frame Debugger)
2. Check if `GetAdditionalLightsCount()` returns the expected count
3. Add debug visualization to see which lights are affecting which objects
4. Test with standard URP Lit shader to see if it has the same issue
5. Review URP source code for how multiple directional lights should be handled
6. Consider if this is a URP limitation rather than a Toon shader issue
