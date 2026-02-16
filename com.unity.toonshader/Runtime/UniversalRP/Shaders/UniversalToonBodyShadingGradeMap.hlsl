#include "../../Shaders/UTSLighting.hlsl"

void ToonShadingSG(
    const float3 highlightAlbedo, const float3 highlightMaskTex,
    const float3 lightColor, const float lightIntensity, const float tweakShadows, 
    const float3 baseAlbedo, const float3 firstShadeAlbedo, const float3 secondShadeAlbedo,
    const float firstShadeColorStep, const float secondShadeColorStep,
    const float3 vertexNormalWS, const float3 perturbedNormalWS, const float3 lightDirection, const float3 viewDirection,
    const float specularBlendModeLerp, const float filterHighlightInForwardAdd, const float sgMapLevel, 
    out float3 outShadeResult, out float outShadowMask     
) {
    const float3 halfDirection = normalize(viewDirection + lightDirection);
    
    //v.2.0.5: Filtering the high intensity zone of PointLights
    float3 Set_BaseColor = lerp((baseAlbedo * lightIntensity), (baseAlbedo * lightColor), _Is_LightColor_Base);
    
    //v.2.0.5
    float3 Set_1st_ShadeColor = lerp((firstShadeAlbedo * lightIntensity),(firstShadeAlbedo * lightColor), _Is_LightColor_1st_Shade);
    float3 Set_2nd_ShadeColor = lerp((secondShadeAlbedo * lightIntensity),(secondShadeAlbedo * lightColor), _Is_LightColor_2nd_Shade);
    float halfLambert = 0.5 * dot(lerp(vertexNormalWS, perturbedNormalWS, _Is_NormalMapToBase), lightDirection) + 0.5;
    
    float Set_ShadingGrade = saturate(sgMapLevel) * lerp(halfLambert,(halfLambert * saturate(tweakShadows)), _Set_SystemShadowsToBase);
    
    
    float Set_FinalShadowMask = saturate(
        (1.0 + ((Set_ShadingGrade - (firstShadeColorStep - _1st_ShadeColor_Feather)) * (0.0 - 1.0)) / (
            firstShadeColorStep - (firstShadeColorStep - _1st_ShadeColor_Feather)))); // Base and 1st Shade Mask
	
    //v.2.0.5
    float Set_ShadeShadowMask = saturate(
        (1.0 + ((Set_ShadingGrade - (secondShadeColorStep - _2nd_ShadeColor_Feather)) * (0.0 - 1.0)) / (
            secondShadeColorStep - (secondShadeColorStep - _2nd_ShadeColor_Feather)))); // 1st and 2nd Shades Mask
    
    
    //Composition: 3 Basic Colors as finalColor
    float3 finalColor =
        lerp(
            Set_BaseColor,
            lerp(
                Set_1st_ShadeColor,
                Set_2nd_ShadeColor,
                Set_ShadeShadowMask
            ),
            Set_FinalShadowMask);    

    float specular = 0.5 * dot(halfDirection, lerp(vertexNormalWS, perturbedNormalWS, _Is_NormalMapToHighColor)) + 0.5;
    
    // Specular
    float tweakHighColorMask = (saturate((highlightMaskTex.g + _Tweak_HighColorMaskLevel)) * lerp(
        (1.0 - step(specular, (1.0 - pow(abs(_HighColor_Power), 5)))),
        pow(abs(specular), exp2(lerp(11, 1, _HighColor_Power))), _Is_SpecularToHighColor));

    float3 highColor = (lerp(highlightAlbedo, highlightAlbedo * lightColor, _Is_LightColor_HighColor) 
        * tweakHighColorMask);
    
    //Composition: 3 Basic Colors and HighColor as Set_HighColor
    finalColor = lerp(SATURATE_IF_SDR((finalColor-tweakHighColorMask)), finalColor,specularBlendModeLerp); 
    finalColor = finalColor + lerp(
        lerp(highColor,(highColor * ((1.0 - Set_FinalShadowMask) + (Set_FinalShadowMask * _TweakHighColorOnShadow))),_Is_UseTweakHighColorOnShadow), float3(0, 0, 0), filterHighlightInForwardAdd);
    
    
    outShadeResult = finalColor;
    outShadowMask = Set_FinalShadowMask;
}



void frag(VertexOutput i, out float4 finalRGBA : SV_Target0
#ifdef _WRITE_RENDERING_LAYERS
          , out float4 outRenderingLayers : SV_Target1
#endif
) {
    i.normalDir = normalize(i.normalDir);
    
    const float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
    const float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
    const float2 Set_UV0 = i.uv0;


    // todo. not necessary to calc gi factor in  shadowcaster pass.
    SurfaceData surfaceData;
    InitializeStandardLitSurfaceDataUTS(i.uv0, surfaceData);

    InputData inputData;
    Varyings input = (Varyings)0;

    // todo.  it has to be cared more.
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
#ifdef LIGHTMAP_ON

#else
    input.vertexSH = i.vertexSH;
#endif
    input.uv = i.uv0;
    input.positionCS = i.pos;
#if defined(_ADDITIONAL_LIGHTS_VERTEX)

    input.fogFactorAndVertexLight = i.fogFactorAndVertexLight;
#else
    input.fogFactor = i.fogFactor;
#endif

#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    input.shadowCoord = i.shadowCoord;
#endif

#ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
    input.positionWS = i.posWorld.xyz;
#endif
    input.normalWS = half3(i.normalDir);
    InitializeInputData(input, surfaceData.normalTS, inputData);
    InitializeBakedGIData(input, inputData);
    BRDFData brdfData;
    InitializeBRDFData(surfaceData.albedo,
        surfaceData.metallic,
        surfaceData.specular,
        surfaceData.smoothness,
        surfaceData.alpha, brdfData);

    half3 envColor = GlobalIlluminationUTS(brdfData, inputData.bakedGI, surfaceData.occlusion, inputData.normalWS,
        inputData.viewDirectionWS, i.posWorld.xyz, inputData.normalizedScreenSpaceUV);
    envColor *= 1.8f;

    UtsLight mainLight = GetMainUtsLightByID(i.mainLightID, i.posWorld.xyz, inputData.shadowCoord);

#if defined(_LIGHT_LAYERS)
    uint meshRenderingLayers = GetMeshRenderingLayer();
#endif

    half3 mainLightColor = GetLightColor(
        mainLight
#ifdef _LIGHT_LAYERS
        , meshRenderingLayers
#endif
    );
    const float2 mainTexUV = TRANSFORM_TEX(i.uv0, _MainTex);

    float4 tempMainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainTexUV);
    const float4 highlightTex = tex2D(_HighColor_Tex, TRANSFORM_TEX(Set_UV0, _HighColor_Tex));
    const float4 highlightMaskTex = tex2D(_Set_HighColorMask, TRANSFORM_TEX(Set_UV0, _Set_HighColorMask));

    const float3 highlightAlbedo = highlightTex.rgb * _HighColor.rgb;
    
    const float3 normalTex = UnpackNormalScale(
        SAMPLE_TEXTURE2D(_NormalMap, sampler_MainTex, TRANSFORM_TEX(Set_UV0, _NormalMap)), _BumpScale);
    float3 tempNormalDirection = normalize(mul(normalTex, tangentTransform)); // Perturbed normals
    
    //Decal
    ApplyDecalToSurfaceDataUTS(input.positionCS, tempMainTex.rgb, surfaceData, tempNormalDirection);
    const float4 mainTex = tempMainTex;
    const float3 normalDirection = tempNormalDirection;

    const float4 firstShadeTex = lerp(SAMPLE_TEXTURE2D(_1st_ShadeMap, sampler_MainTex, mainTexUV),mainTex, _Use_BaseAs1st);
    const float4 secondShadeTex = lerp(SAMPLE_TEXTURE2D(_2nd_ShadeMap, sampler_MainTex, mainTexUV),firstShadeTex, _Use_1stAs2nd);
    
    const float3 baseAlbedo = _BaseColor.rgb * mainTex.rgb;
    const float3 firstShadeAlbedo = _1st_ShadeColor.rgb * firstShadeTex.rgb;
    const float3 secondShadeAlbedo = _2nd_ShadeColor.rgb * secondShadeTex.rgb;
    
    const float4 sgMapTex = tex2Dlod(_ShadingGradeMap, float4(TRANSFORM_TEX(Set_UV0, _ShadingGradeMap), 0.0, _BlurLevelSGM));

    //[TODO-sin: 2026-2-4] We only need one channel ?
    const float sgMapLevel = sgMapTex.r < 0.95 ? sgMapTex.r + _Tweak_ShadingGradeMapLevel : 1;

#ifdef _IS_TRANSCLIPPING_OFF
    //
#elif _IS_TRANSCLIPPING_ON

    float4 _ClippingMask_var = SAMPLE_TEXTURE2D(_ClippingMask, sampler_MainTex, mainTexUV);
    float Set_MainTexAlpha = mainTex.a;
    float _IsBaseMapAlphaAsClippingMask_var =
        lerp(_ClippingMask_var.r, Set_MainTexAlpha, _IsBaseMapAlphaAsClippingMask);
    float _Inverse_Clipping_var = lerp(_IsBaseMapAlphaAsClippingMask_var, (1.0 - _IsBaseMapAlphaAsClippingMask_var),
        _Inverse_Clipping);
    float Set_Clipping = saturate((_Inverse_Clipping_var + _Clipping_Level));
    clip(Set_Clipping - 0.5);
#endif


    float shadowAttenuation = 1.0;

#if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)
    shadowAttenuation = mainLight.shadowAttenuation;
#endif


    const float firstStepMinusFeather = _1st_ShadeColor_Step - _1st_ShadeColor_Feather;
    const float secondStepMinusFeather = _2nd_ShadeColor_Step - _2nd_ShadeColor_Feather;
    
    //v.2.0.4

    float3 defaultLightDirection = normalize(UNITY_MATRIX_V[2].xyz + UNITY_MATRIX_V[1].xyz);
    //v.2.0.5
    float3 defaultLightColor = saturate(max(half3(0.05, 0.05, 0.05) * _Unlit_Intensity,
        max(ShadeSH9(half4(0.0, 0.0, 0.0, 1.0)), ShadeSH9(half4(0.0, -1.0, 0.0, 1.0)).rgb) * _Unlit_Intensity));
    float3 customLightDirection = normalize(mul(GetObjectToWorldMatrix(),
        float4(
            ((float3(1.0, 0.0, 0.0) * _Offset_X_Axis_BLD * 10) + (float3(0.0, 1.0, 0.0) * _Offset_Y_Axis_BLD * 10) + (
                float3(0.0, 0.0, -1.0) * lerp(-1.0, 1.0, _Inverse_Z_Axis_BLD))), 0)).xyz);
    float3 lightDirection = normalize(
        lerp(defaultLightDirection, mainLight.direction.xyz, any(mainLight.direction.xyz)));
    lightDirection = lerp(lightDirection, customLightDirection, _Is_BLD);
    //v.2.0.5:

    half3 originalLightColor = mainLightColor.rgb;

    float3 lightColor = lerp(max(defaultLightColor, originalLightColor),
        max(defaultLightColor, saturate(originalLightColor)), _Is_Filter_LightColor);


#ifdef _IS_PASS_FWDBASE

    //the value of shadowAttenuation is darker than legacy and it cuases noise in terminaters.
    shadowAttenuation *= 2.0f;
    shadowAttenuation = saturate(shadowAttenuation);

    //v.2.0.6
    //Minmimum value is same as the Minimum Feather's value with the Minimum Step's value as threshold.
    float _SystemShadowsLevel_var = (shadowAttenuation * 0.5) + 0.5 + _Tweak_SystemShadowsLevel > 0.001
                                        ? (shadowAttenuation * 0.5) + 0.5 + _Tweak_SystemShadowsLevel
                                        : 0.0001;
    
    float lightIntensity = 1;
    float tweakShadows = _SystemShadowsLevel_var;
    float firstShadeColorStep = _1st_ShadeColor_Step;
    float secondShadeColorStep = _2nd_ShadeColor_Step;
    float specularBlendModeLerp = lerp(_Is_BlendAddToHiColor, 1.0, _Is_SpecularToHighColor);
    float filterHighlightInForwardAdd = 0;
    float3 Set_HighColor = float3(0,0,0);
    float Set_FinalShadowMask;
    ToonShadingSG(
        highlightAlbedo, highlightMaskTex.rgb,
        lightColor, lightIntensity, tweakShadows, 
        baseAlbedo, firstShadeAlbedo, secondShadeAlbedo,
        firstShadeColorStep, secondShadeColorStep,
        i.normalDir, normalDirection, lightDirection, viewDirection,
        specularBlendModeLerp, filterHighlightInForwardAdd, sgMapLevel, 
        Set_HighColor, Set_FinalShadowMask);

    float4 _Set_RimLightMask_var = tex2D(_Set_RimLightMask, TRANSFORM_TEX(Set_UV0, _Set_RimLightMask));

    float3 _Is_LightColor_RimLight_var = lerp(_RimLightColor.rgb, (_RimLightColor.rgb * lightColor),
        _Is_LightColor_RimLight);
    float _RimArea_var = abs(1.0 - dot(lerp(i.normalDir, normalDirection, _Is_NormalMapToRimLight), viewDirection));
    float _RimLightPower_var = pow(_RimArea_var, exp2(lerp(3, 0, _RimLight_Power)));
    float _Rimlight_InsideMask_var = saturate(lerp(
        (0.0 + ((_RimLightPower_var - _RimLight_InsideMask) * (1.0 - 0.0)) / (1.0 - _RimLight_InsideMask)),
        step(_RimLight_InsideMask, _RimLightPower_var), _RimLight_FeatherOff));
    float _VertHalfLambert_var = 0.5 * dot(i.normalDir, lightDirection) + 0.5;
    float3 _LightDirection_MaskOn_var = lerp((_Is_LightColor_RimLight_var * _Rimlight_InsideMask_var),
        (_Is_LightColor_RimLight_var * saturate(
            (_Rimlight_InsideMask_var - ((1.0 - _VertHalfLambert_var) + _Tweak_LightDirection_MaskLevel)))),
        _LightDirection_MaskOn);
    float _ApRimLightPower_var = pow(_RimArea_var, exp2(lerp(3, 0, _Ap_RimLight_Power)));
    float3 Set_RimLight = (saturate((_Set_RimLightMask_var.g + _Tweak_RimLightMaskLevel)) * lerp(
        _LightDirection_MaskOn_var,
        (_LightDirection_MaskOn_var + (
            lerp(_Ap_RimLightColor.rgb, (_Ap_RimLightColor.rgb * lightColor), _Is_LightColor_Ap_RimLight) *
            saturate((lerp(
                (0.0 + ((_ApRimLightPower_var - _RimLight_InsideMask) * (1.0 - 0.0)) / (1.0 - _RimLight_InsideMask)),
                step(_RimLight_InsideMask, _ApRimLightPower_var),
                _Ap_RimLight_FeatherOff) - (saturate(_VertHalfLambert_var) + _Tweak_LightDirection_MaskLevel))))),
        _Add_Antipodean_RimLight));
    //Composition: HighColor and RimLight as _RimLight_var
    float3 _RimLight_var = lerp(Set_HighColor, (Set_HighColor + Set_RimLight), _RimLight);
    //Matcap
    //v.2.0.6 : CameraRolling Stabilizer
    //Mirror Script Determination: if sign_Mirror = -1, determine "Inside the mirror".
    //v.2.0.7
    fixed _sign_Mirror = i.mirrorFlag;
    //
    float3 _Camera_Right = UNITY_MATRIX_V[0].xyz;
    float3 _Camera_Front = UNITY_MATRIX_V[2].xyz;
    float3 _Up_Unit = float3(0, 1, 0);
    float3 _Right_Axis = cross(_Camera_Front, _Up_Unit);
    //Invert if it's "inside the mirror".
    if (_sign_Mirror < 0)
    {
        _Right_Axis = -1 * _Right_Axis;
        _Rotate_MatCapUV = -1 * _Rotate_MatCapUV;
    }
    else
    {
        _Right_Axis = _Right_Axis;
    }
    float _Camera_Right_Magnitude = sqrt(
        _Camera_Right.x * _Camera_Right.x + _Camera_Right.y * _Camera_Right.y + _Camera_Right.z * _Camera_Right.z);
    float _Right_Axis_Magnitude = sqrt(
        _Right_Axis.x * _Right_Axis.x + _Right_Axis.y * _Right_Axis.y + _Right_Axis.z * _Right_Axis.z);
    float _Camera_Roll_Cos = dot(_Right_Axis, _Camera_Right) / (_Right_Axis_Magnitude * _Camera_Right_Magnitude);
    float _Camera_Roll = acos(clamp(_Camera_Roll_Cos, -1, 1));
    fixed _Camera_Dir = _Camera_Right.y < 0 ? -1 : 1;
    float _Rot_MatCapUV_var_ang = (_Rotate_MatCapUV * 3.141592654) - _Camera_Dir * _Camera_Roll *
        _CameraRolling_Stabilizer;
    //v.2.0.7
    float2 _Rot_MatCapNmUV_var = RotateUV(Set_UV0, (_Rotate_NormalMapForMatCapUV * 3.141592654), float2(0.5, 0.5), 1.0);
    //V.2.0.6

    float3 _NormalMapForMatCap_var = UnpackNormalScale(
        tex2D(_NormalMapForMatCap, TRANSFORM_TEX(_Rot_MatCapNmUV_var, _NormalMapForMatCap)), _BumpScaleMatcap);

    //v.2.0.5: MatCap with camera skew correction
    float3 viewNormal = (mul(UNITY_MATRIX_V,
        float4(lerp(i.normalDir, mul(_NormalMapForMatCap_var.rgb, tangentTransform).rgb, _Is_NormalMapForMatCap),
            0))).rgb;
    float3 NormalBlend_MatcapUV_Detail = viewNormal.rgb * float3(-1, -1, 1);
    float3 NormalBlend_MatcapUV_Base = (mul(UNITY_MATRIX_V, float4(viewDirection, 0)).rgb * float3(-1, -1, 1)) +
        float3(0, 0, 1);
    float3 noSknewViewNormal = NormalBlend_MatcapUV_Base * dot(NormalBlend_MatcapUV_Base, NormalBlend_MatcapUV_Detail) /
        NormalBlend_MatcapUV_Base.b - NormalBlend_MatcapUV_Detail;
    float2 _ViewNormalAsMatCapUV = (lerp(noSknewViewNormal, viewNormal, _Is_Ortho).rg * 0.5) + 0.5;
    //
    //v.2.0.7
    float2 _Rot_MatCapUV_var = RotateUV(
        (0.0 + ((_ViewNormalAsMatCapUV - (0.0 + _Tweak_MatCapUV)) * (1.0 - 0.0)) / ((1.0 - _Tweak_MatCapUV) - (0.0 +
            _Tweak_MatCapUV))), _Rot_MatCapUV_var_ang, float2(0.5, 0.5), 1.0);
    //If it is "inside the mirror", flip the UV left and right.

    if (_sign_Mirror < 0)
    {
        _Rot_MatCapUV_var.x = 1 - _Rot_MatCapUV_var.x;
    }
    else
    {
        _Rot_MatCapUV_var = _Rot_MatCapUV_var;
    }


    float4 _MatCap_Sampler_var = tex2Dlod(_MatCap_Sampler,
        float4(TRANSFORM_TEX(_Rot_MatCapUV_var, _MatCap_Sampler), 0.0, _BlurLevelMatcap));
    float4 _Set_MatcapMask_var = tex2D(_Set_MatcapMask, TRANSFORM_TEX(Set_UV0, _Set_MatcapMask));


    //
    //MatcapMask
    float _Tweak_MatcapMaskLevel_var = saturate(
        lerp(_Set_MatcapMask_var.g, (1.0 - _Set_MatcapMask_var.g), _Inverse_MatcapMask) + _Tweak_MatcapMaskLevel);
    float3 _Is_LightColor_MatCap_var = lerp((_MatCap_Sampler_var.rgb * _MatCapColor.rgb),
        ((_MatCap_Sampler_var.rgb * _MatCapColor.rgb) * lightColor), _Is_LightColor_MatCap);
    //v.2.0.6 : ShadowMask on Matcap in Blend mode : multiply
    float3 Set_MatCap = lerp(_Is_LightColor_MatCap_var,
        (_Is_LightColor_MatCap_var * ((1.0 - Set_FinalShadowMask) + (Set_FinalShadowMask * _TweakMatCapOnShadow)) +
            lerp(Set_HighColor * Set_FinalShadowMask * (1.0 - _TweakMatCapOnShadow), float3(0.0, 0.0, 0.0),
                _Is_BlendAddToMatCap)), _Is_UseTweakMatCapOnShadow);

    //
    //v.2.0.6
    //Composition: RimLight and MatCap as finalColor
    //Broke down finalColor composition
    float3 matCapColorOnAddMode = _RimLight_var + Set_MatCap * _Tweak_MatcapMaskLevel_var;
    float _Tweak_MatcapMaskLevel_var_MultiplyMode = _Tweak_MatcapMaskLevel_var * lerp(1,
        (1 - (Set_FinalShadowMask) * (1 - _TweakMatCapOnShadow)), _Is_UseTweakMatCapOnShadow);
    float3 matCapColorOnMultiplyMode = Set_HighColor * (1 - _Tweak_MatcapMaskLevel_var_MultiplyMode) + Set_HighColor *
        Set_MatCap * _Tweak_MatcapMaskLevel_var_MultiplyMode + lerp(float3(0, 0, 0), Set_RimLight, _RimLight);
    float3 matCapColorFinal = lerp(matCapColorOnMultiplyMode, matCapColorOnAddMode, _Is_BlendAddToMatCap);
    //v.2.0.4
#if defined(_IS_ANGELRING_ON)
    float3 finalColor = lerp(_RimLight_var, matCapColorFinal, _MatCap); // Final Composition before AR
    //v.2.0.7 AR Camera Rolling Stabilizer
    float3 _AR_OffsetU_var = lerp(mul(UNITY_MATRIX_V, float4(i.normalDir, 0)).xyz, float3(0, 0, 1), _AR_OffsetU);
    float2 AR_VN = _AR_OffsetU_var.xy * 0.5 + float2(0.5, 0.5);
    float2 AR_VN_Rotate = RotateUV(AR_VN, -(_Camera_Dir * _Camera_Roll), float2(0.5, 0.5), 1.0);
    float2 _AR_OffsetV_var = float2(AR_VN_Rotate.x, lerp(i.uv1.y, AR_VN_Rotate.y, _AR_OffsetV));
    float4 _AngelRing_Sampler_var = tex2D(_AngelRing_Sampler, TRANSFORM_TEX(_AR_OffsetV_var, _AngelRing_Sampler));
    float3 _Is_LightColor_AR_var = lerp((_AngelRing_Sampler_var.rgb * _AngelRing_Color.rgb),
        ((_AngelRing_Sampler_var.rgb * _AngelRing_Color.rgb) * lightColor), _Is_LightColor_AR);
    float3 Set_AngelRing = _Is_LightColor_AR_var;
    float Set_ARtexAlpha = _AngelRing_Sampler_var.a;
    float3 Set_AngelRingWithAlpha = (_Is_LightColor_AR_var * _AngelRing_Sampler_var.a);
    //Composition: MatCap and AngelRing as finalColor
    finalColor = lerp(finalColor,
        lerp((finalColor + Set_AngelRing), ((finalColor * (1.0 - Set_ARtexAlpha)) + Set_AngelRingWithAlpha),
            _ARSampler_AlphaOn), _AngelRing); // Final Composition before Emissive
#else
    float3 finalColor = lerp(_RimLight_var, matCapColorFinal, _MatCap); // Final Composition before Emissive
#endif
    //v.2.0.7
#ifdef _EMISSIVE_SIMPLE
    float4 _Emissive_Tex_var = tex2D(_Emissive_Tex, TRANSFORM_TEX(Set_UV0, _Emissive_Tex));
    float emissiveMask = _Emissive_Tex_var.a;
    emissive = _Emissive_Tex_var.rgb * _Emissive_Color.rgb * emissiveMask;
#elif _EMISSIVE_ANIMATION
    //v.2.0.7 Calculation View Coord UV for Scroll
    float3 viewNormal_Emissive = (mul(UNITY_MATRIX_V, float4(i.normalDir, 0))).xyz;
    float3 NormalBlend_Emissive_Detail = viewNormal_Emissive * float3(-1, -1, 1);
    float3 NormalBlend_Emissive_Base = (mul(UNITY_MATRIX_V, float4(viewDirection, 0)).xyz * float3(-1, -1, 1)) +
        float3(0, 0, 1);
    float3 noSknewViewNormal_Emissive = NormalBlend_Emissive_Base * dot(NormalBlend_Emissive_Base,
        NormalBlend_Emissive_Detail) / NormalBlend_Emissive_Base.z - NormalBlend_Emissive_Detail;
    float2 _ViewNormalAsEmissiveUV = noSknewViewNormal_Emissive.xy * 0.5 + 0.5;
    float2 _ViewCoord_UV = RotateUV(_ViewNormalAsEmissiveUV, -(_Camera_Dir * _Camera_Roll), float2(0.5, 0.5), 1.0);
    //鏡の中ならUV左右反転.
    if (_sign_Mirror < 0)
    {
        _ViewCoord_UV.x = 1 - _ViewCoord_UV.x;
    }
    else
    {
        _ViewCoord_UV = _ViewCoord_UV;
    }
    float2 emissive_uv = lerp(i.uv0, _ViewCoord_UV, _Is_ViewCoord_Scroll);
    //
    float4 _time_var = _Time;
    float _base_Speed_var = (_time_var.g * _Base_Speed);
    float _Is_PingPong_Base_var = lerp(_base_Speed_var, sin(_base_Speed_var), _Is_PingPong_Base);
    float2 scrolledUV = emissive_uv + float2(_Scroll_EmissiveU, _Scroll_EmissiveV) * _Is_PingPong_Base_var;
    float rotateVelocity = _Rotate_EmissiveUV * 3.141592654;
    float2 _rotate_EmissiveUV_var = RotateUV(scrolledUV, rotateVelocity, float2(0.5, 0.5), _Is_PingPong_Base_var);
    float4 _Emissive_Tex_var = tex2D(_Emissive_Tex, TRANSFORM_TEX(Set_UV0, _Emissive_Tex));
    float emissiveMask = _Emissive_Tex_var.a;
    _Emissive_Tex_var = tex2D(_Emissive_Tex, TRANSFORM_TEX(_rotate_EmissiveUV_var, _Emissive_Tex));
    float _colorShift_Speed_var = 1.0 - cos(_time_var.g * _ColorShift_Speed);
    float viewShift_var = smoothstep(0.0, 1.0, max(0, dot(normalDirection, viewDirection)));
    float4 colorShift_Color = lerp(_Emissive_Color, lerp(_Emissive_Color, _ColorShift, _colorShift_Speed_var),
        _Is_ColorShift);
    float4 viewShift_Color = lerp(_ViewShift, colorShift_Color, viewShift_var);
    float4 emissive_Color = lerp(colorShift_Color, viewShift_Color, _Is_ViewShift);
    emissive = emissive_Color.rgb * _Emissive_Tex_var.rgb * emissiveMask;
#endif
    //
    //v.2.0.6: GI_Intensity with Intensity Multiplier Filter

    float3 envLightColor = envColor.rgb;

    float envLightIntensity = min(Intensity(envLightColor), 1);


    float3 pointLightColor = 0;
#ifdef _ADDITIONAL_LIGHTS

    int pixelLightCount = GetAdditionalLightsCount();

#if USE_FORWARD_PLUS
    for (uint loopCounter = 0; loopCounter < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); loopCounter++)
    {
        int iLight = loopCounter;
        // if (iLight != i.mainLightID)
        {
            float notDirectional = 1.0f; //_WorldSpaceLightPos0.w of the legacy code.
            UtsLight additionalLight = GetUrpMainUtsLight(0);
            additionalLight = GetAdditionalUtsLight(loopCounter, inputData.positionWS);
            half3 additionalLightColor = GetLightColor(
                additionalLight
#ifdef _LIGHT_LAYERS
                , meshRenderingLayers
#endif
            );

            float3 lightDirection = additionalLight.direction;
            //v.2.0.5:
            float3 addPassLightColor = (0.5 * dot(lerp(i.normalDir, normalDirection, _Is_NormalMapToBase), lightDirection) +
                0.5) * additionalLightColor.rgb;
            float pureIntencity = max(0.001,
                Intensity(additionalLightColor));
            float3 lightColor = max(float3(0.0, 0.0, 0.0), lerp(addPassLightColor,
                lerp(float3(0.0, 0.0, 0.0), min(addPassLightColor, addPassLightColor / pureIntencity), notDirectional),
                _Is_Filter_LightColor));
            float3 halfDirection = normalize(viewDirection + lightDirection); // has to be recalced here.

            //v.2.0.5: If Added lights is directional, set 0 as _LightIntensity
            float _LightIntensity = lerp(0,
                Intensity(additionalLightColor),
                notDirectional);
            
            float lightIntensity = _LightIntensity;
            float tweakShadows = 1.0 + _Tweak_SystemShadowsLevel;
            float firstShadeColorStep = saturate(_1st_ShadeColor_Step + _StepOffset);
            float secondShadeColorStep = saturate(_2nd_ShadeColor_Step + _StepOffset);
            float specularBlendModeLerp = 1;
            float filterHighlightInForwardAdd = _Is_Filter_HiCutPointLightColor;
            float3 finalColor = float3(0,0,0);
            float unused = 0;
            //[TODO-sin: 2026-2-6] We should normalize i.normalDir too here.
            ToonShadingSG(
                highlightAlbedo, highlightMaskTex.rgb,
                lightColor, lightIntensity, tweakShadows, 
                baseAlbedo, firstShadeAlbedo, secondShadeAlbedo,
                firstShadeColorStep, secondShadeColorStep,
                i.normalDir, normalDirection, lightDirection, viewDirection,
                specularBlendModeLerp, filterHighlightInForwardAdd, sgMapLevel, 
                finalColor, unused);
                        
            

            finalColor = SATURATE_IF_SDR(finalColor);

            pointLightColor += finalColor;
        }
    }
#endif  // USE_FORWARD_PLUS
    // determine main light inorder to apply light culling properly

    // when the loop counter start from negative value, MAINLIGHT_IS_MAINLIGHT = -1, some compiler doesn't work well.
    // for (int iLight = MAINLIGHT_IS_MAINLIGHT; iLight < pixelLightCount ; ++iLight)
    UTS_LIGHT_LOOP_BEGIN(pixelLightCount - MAINLIGHT_IS_MAINLIGHT)
#if USE_FORWARD_PLUS
    int iLight = lightIndex;
#else
    int iLight = loopCounter + MAINLIGHT_IS_MAINLIGHT;
    if (iLight != i.mainLightID)
#endif
    {
        float notDirectional = 1.0f; //_WorldSpaceLightPos0.w of the legacy code.
        UtsLight additionalLight = GetUrpMainUtsLight(0);
        if (iLight != MAINLIGHT_IS_MAINLIGHT)
        {
            additionalLight = GetAdditionalUtsLight(iLight, inputData.positionWS);
        }
        half3 additionalLightColor = GetLightColor(
            additionalLight
#ifdef _LIGHT_LAYERS
            , meshRenderingLayers
#endif
        );
        
        float3 lightDirection = additionalLight.direction;
        //v.2.0.5:
        float3 addPassLightColor = (0.5 * dot(lerp(i.normalDir, normalDirection, _Is_NormalMapToBase), lightDirection) +
            0.5) * additionalLightColor.rgb;
        float pureIntencity = max(0.001,
            Intensity(additionalLightColor));
        float3 lightColor = max(float3(0.0, 0.0, 0.0), lerp(addPassLightColor,
            lerp(float3(0.0, 0.0, 0.0), min(addPassLightColor, addPassLightColor / pureIntencity), notDirectional),
            _Is_Filter_LightColor));
        float3 halfDirection = normalize(viewDirection + lightDirection); // has to be recalced here.

        //v.2.0.5: If Added lights is directional, set 0 as _LightIntensity
        float _LightIntensity = lerp(0,
            Intensity(additionalLightColor),
            notDirectional);

        float lightIntensity = _LightIntensity;
        float tweakShadows = 1.0 + _Tweak_SystemShadowsLevel;
        float firstShadeColorStep = saturate(_1st_ShadeColor_Step + _StepOffset);
        float secondShadeColorStep = saturate(_2nd_ShadeColor_Step + _StepOffset);
        float specularBlendModeLerp = 1;
        float filterHighlightInForwardAdd = _Is_Filter_HiCutPointLightColor;
        float3 finalColor = float3(0,0,0);
        float unused = 0;
        //[TODO-sin: 2026-2-6] We should normalize i.normalDir too here.
        ToonShadingSG(
            highlightAlbedo, highlightMaskTex.rgb,
            lightColor, lightIntensity, tweakShadows, 
            baseAlbedo, firstShadeAlbedo, secondShadeAlbedo,
            firstShadeColorStep, secondShadeColorStep,
            i.normalDir, normalDirection, lightDirection, viewDirection,
            specularBlendModeLerp, filterHighlightInForwardAdd, sgMapLevel, 
            finalColor, unused);
            
        finalColor = SATURATE_IF_SDR(finalColor);
        pointLightColor += finalColor;
    }
    UTS_LIGHT_LOOP_END

#endif // _ADDITIONAL_LIGHTS

    //
    //Final Composition

    finalColor = SATURATE_IF_SDR(finalColor) + (envLightColor * envLightIntensity * _GI_Intensity * smoothstep(1, 0,
        envLightIntensity / 2)) + emissive;

    finalColor += pointLightColor;

#endif


    //v.2.0.4
#ifdef _IS_TRANSCLIPPING_OFF

    finalRGBA = fixed4(finalColor, 1);

#elif _IS_TRANSCLIPPING_ON
    float Set_Opacity = SATURATE_IF_SDR((_Inverse_Clipping_var + _Tweak_transparency));

    finalRGBA = fixed4(finalColor, Set_Opacity);

#endif

#ifdef _WRITE_RENDERING_LAYERS
    uint renderingLayers = GetMeshRenderingLayer();
    outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
#endif
}
