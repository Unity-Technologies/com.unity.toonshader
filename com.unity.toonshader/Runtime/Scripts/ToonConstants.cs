
using UnityEngine;

namespace Unity.Rendering.Toon {

internal static class ToonConstants {

    internal const string PACKAGE_NAME = "com.unity.toonshader";
    internal const string PACKAGE_VERSION_MAJOR_MINOR = "0.12";
    
    internal const string SHADER_KEYWORD_IS_CLIPPING_MATTE = "_IS_CLIPPING_MATTE";

    internal static readonly int SHADER_PROPERTY_CLIPPING_MATTE_MODE = Shader.PropertyToID("_ClippingMatteMode");

    internal static readonly int SHADER_PROPERTY_BASE_COLOR_VISIBLE   = Shader.PropertyToID("_BaseColorVisible");
    internal static readonly int SHADER_PROPERTY_FIRST_SHADE_VISIBLE  = Shader.PropertyToID("_FirstShadeVisible");
    internal static readonly int SHADER_PROPERTY_SECOND_SHADE_VISIBLE = Shader.PropertyToID("_SecondShadeVisible");
    internal static readonly int SHADER_PROPERTY_HIGHLIGHT_VISIBLE    = Shader.PropertyToID("_HighlightVisible");
    internal static readonly int SHADER_PROPERTY_ANGEL_RING_VISIBLE   = Shader.PropertyToID("_AngelRingVisible");
    internal static readonly int SHADER_PROPERTY_RIM_LIGHT_VISIBLE    = Shader.PropertyToID("_RimLightVisible");
    internal static readonly int SHADER_PROPERTY_OUTLINE_VISIBLE      = Shader.PropertyToID("_OutlineVisible");

    internal static readonly int SHADER_PROPERTY_COMPOSER_MASK_MODE = Shader.PropertyToID("_ComposerMaskMode");

    internal static readonly int SHADER_PROPERTY_BASE_COLOR_MASK_COLOR   = Shader.PropertyToID("_BaseColorMaskColor");
    internal static readonly int SHADER_PROPERTY_FIRST_SHADE_MASK_COLOR  = Shader.PropertyToID("_FirstShadeMaskColor");
    internal static readonly int SHADER_PROPERTY_SECOND_SHADE_MASK_COLOR = Shader.PropertyToID("_SecondShadeMaskColor");
    internal static readonly int SHADER_PROPERTY_HIGHLIGHT_MASK_COLOR    = Shader.PropertyToID("_HighlightMaskColor");
    internal static readonly int SHADER_PROPERTY_ANGEL_RING_MASK_COLOR   = Shader.PropertyToID("_AngelRingMaskColor");
    internal static readonly int SHADER_PROPERTY_RIM_LIGHT_MASK_COLOR    = Shader.PropertyToID("_RimLightMaskColor");
    internal static readonly int SHADER_PROPERTY_OUTLINE_MASK_COLOR      = Shader.PropertyToID("_OutlineMaskColor");

    //Common constants
    //Colors
    internal const string SHADER_PROP_BASE_COLOR = "_BaseColor";
    internal const string SHADER_PROP_MAIN_TEX = "_MainTex";

    internal const string SHADER_PROP_NORMAL_MAP = "_NormalMap";
    internal const string SHADER_PROP_BUMP_SCALE = "_BumpScale";
    
    internal const string SHADER_PROP_1_ST_SHADE_COLOR = "_1st_ShadeColor";
    internal const string SHADER_PROP_1_ST_SHADE_MAP = "_1st_ShadeMap";
    internal const string SHADER_PROP_USE_BASE_AS_1ST = "_Use_BaseAs1st";

    internal const string SHADER_PROP_2ND_SHADE_COLOR = "_2nd_ShadeColor";
    internal const string SHADER_PROP_2ND_SHADE_MAP = "_2nd_ShadeMap";
    internal const string SHADER_PROP_USE_1ST_AS_2ND = "_Use_1stAs2nd";

    //Shading
    internal const string SHADER_PROP_BASE_TO_1ST_SHADE_START = "_BaseTo1st_ShadeStart";
    internal const string SHADER_PROP_BASE_TO_1ST_SHADE_FEATHER = "_BaseTo1st_ShadeFeather";
    internal const string SHADER_PROP_1ST_TO_2ND_SHADE_START = "_1stTo2nd_ShadeStart";
    internal const string SHADER_PROP_1ST_TO_2ND_SHADE_FEATHER = "_1stTo2nd_ShadeFeather";


    //Lighting
    internal const string SHADER_PROP_2D_LIGHT_STRENGTH = "_2DLightStrength";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_USE = "_DirectionalLight_Use";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_DIFFUSE_STRENGTH = "_DirectionalLight_DiffuseStrength";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_MODE = "_DirectionalLight_HighlightMode";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_STRENGTH = "_DirectionalLight_HighlightStrength";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_HIGHLIGHT_SIZE = "_DirectionalLight_HighlightSize";

    //Highlight
    internal const string SHADER_PROP_HIGHLIGHT_COLOR = "_HighlightColor";
    internal const string SHADER_PROP_HIGHLIGHT_TEX = "_HighlightTex";

    //Outline
    internal const string SHADER_PROP_OUTLINE_MODE = "_OutlineMode";
    internal static readonly int SHADER_PROP_OUTLINE_MODE_ID = Shader.PropertyToID(SHADER_PROP_OUTLINE_MODE);
    internal const string SHADER_PROP_OUTLINE_WIDTH = "_OutlineWidth";
    internal const string SHADER_PROP_OUTLINE_WIDTH_MAP = "_OutlineWidthMap";
    internal const string SHADER_PROP_OUTLINE_TEX = "_OutlineTex";
    internal const string SHADER_PROP_OUTLINE_COLOR = "_OutlineColor";
    internal const string SHADER_PROP_OUTLINE_BASE_COLOR_BLEND = "_Outline_BaseColorBlend";
    internal const string SHADER_PROP_OUTLINE_LIGHT_COLOR_BLEND = "_Outline_LightColorBlend";
    internal const string SHADER_PROP_OUTLINE_OFFSET_Z = "_OutlineOffsetZ";
    internal const string SHADER_PROP_OUTLINE_NEAR = "_OutlineNear";
    internal const string SHADER_PROP_OUTLINE_FAR = "_OutlineFar";
    internal const string SHADER_PROP_OUTLINE_USE_NORMAL_MAP = "_Outline_UseNormalMap";
    internal const string SHADER_PROP_OUTLINE_NORMAL_MAP = "_Outline_NormalMap";
    
    internal static readonly int SHADER_PROPERTY_MATERIAL_VERSION = Shader.PropertyToID("_ToonMaterialVersion");
    
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_DIRECTION = "_DirectionalLight_Direction";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_COLOR  = "_DirectionalLight_Color";
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_INTENSITY  = "_DirectionalLight_Intensity";
    
    internal const string SHADER_PROP_DIRECTIONAL_LIGHT_VIEW_POSITION = "_DirectionalLight_ViewPosition";
    
    //Doc: Use this LightMode tag value to draw an extra Pass when rendering objects.
    internal const string SHADER_LIGHT_MODE_NAME_FOR_OUTLINE = "SRPDefaultUnlit";

    
    internal const string GBUFFER_PASS_NAME = "GBuffer";

    internal const string SHADER_KEYWORD_RP_BUILTIN = "UTS_RP_BUILTIN";
    
}



} //end namespace