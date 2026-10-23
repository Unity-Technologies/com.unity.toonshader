#ifndef UNIVERSAL_TOON_META_INCLUDED
#define UNIVERSAL_TOON_META_INCLUDED

// Meta pass for URP. Lightmap baking and Surface Cache Global Illumination render this pass to
// read a surface's albedo and emission; Surface Cache looks it up by name ("Meta") and logs an
// error and substitutes a grey fallback material when it is missing.
// Toon's base map is _MainTex (not _BaseMap), so this uses its own vertex function rather than
// UniversalVertexMeta, which applies _BaseMap_ST.

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UniversalMetaPass.hlsl"

Varyings ToonVertexMeta(Attributes input)
{
    Varyings output = (Varyings)0;
    output.positionCS = UnityMetaVertexPosition(input.positionOS.xyz, input.uv1, input.uv2);
    output.uv = input.uv0;
#ifdef EDITOR_VISUALIZATION
    UnityEditorVizData(input.positionOS.xyz, input.uv0, input.uv1, input.uv2, output.VizUV, output.LightCoord);
#endif
    return output;
}

half4 ToonFragmentMeta(Varyings input) : SV_Target
{
    MetaInput metaInput = (MetaInput)0;
    metaInput.Albedo = _BaseColor.rgb * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(input.uv, _MainTex)).rgb;
    metaInput.Emission = _Emissive_Color.rgb * tex2D(_Emissive_Tex, TRANSFORM_TEX(input.uv, _Emissive_Tex)).rgb;
    return UniversalFragmentMeta(input, metaInput);
}

#endif // UNIVERSAL_TOON_META_INCLUDED
