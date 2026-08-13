URP Toon Shader — Metallic + Dissolve

This branch adds a non-invasive URP shader variant that augments the existing Universal Toon Shader (UTS) with:
- Dissolve effect (noise map, threshold, softness, edge color/width, pulse speed).
- Metallic + Roughness maps with scalar fallbacks and an option to invert roughness (for smoothness maps).
- _ToonBlend float to blend between PBR and Toon shading (0 = PBR, 1 = Toon).

Notes and compatibility
- Implemented as a new shader: Runtime/Shaders/URP/ToonURP_MetallicDissolve.shader so all original UTS files remain unchanged.
- Metaverse-specific code was intentionally not copied into the new shader.
- Targets URP ~6.3 compatibility; it relies on existing UTS include files (UniversalToonInput.hlsl, UniversalToonLighting.hlsl). If you use a significantly different URP version, you may need to adjust includes or some helper function signatures.
- No sample textures or scenes were added per your request.

Files added in this commit:
- Runtime/Shaders/URP/ToonURP_MetallicDissolve.shader
- Editor/ToonURP_MetallicDissolveEditor.cs
- Documentation/URP_Dissolve_Metallic_README.md

If you want this integrated into the existing generated shader templates (instead of a separate variant), I can perform a follow-up to update the generator and canonical shader files — that is more invasive.
