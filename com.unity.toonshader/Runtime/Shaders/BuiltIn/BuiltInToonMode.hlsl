// DoubleShadeWithFeather and ShadingGradeMap use different fragment shader.
#pragma shader_feature_local _ _SHADINGGRADEMAP
// used in ShadingGradeMap
#pragma shader_feature _IS_TRANSCLIPPING_OFF _IS_TRANSCLIPPING_ON
#pragma shader_feature _IS_ANGELRING_OFF _IS_ANGELRING_ON
// used in DoubleShadeWithFeather
#pragma shader_feature _IS_CLIPPING_OFF _IS_CLIPPING_MODE _IS_CLIPPING_TRANSMODE
#pragma shader_feature _EMISSIVE_SIMPLE _EMISSIVE_ANIMATION

#if defined(_SHADINGGRADEMAP)

#include "../../Legacy/Shaders/UCTS_ShadingGradeMap.cginc"


#else //#if defined(_SHADINGGRADEMAP)

#include "../../Legacy/Shaders/UCTS_DoubleShadeWithFeather.cginc"


#endif //#if defined(_SHADINGGRADEMAP)
