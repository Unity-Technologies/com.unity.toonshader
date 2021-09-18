//Unity Toon Shader/HDRP
//nobuyuki@unity3d.com
//toshiyuki@unity3d.com (Universal RP/HDRP) 


#ifndef DirectionalShadowType
# if (SHADEROPTIONS_RAYTRACING && (defined(SHADER_API_D3D11) || defined(SHADER_API_D3D12)) && !defined(SHADER_API_XBOXONE) && !defined(SHADER_API_PSSL))
#   define DirectionalShadowType float3
# else
#   define DirectionalShadowType float
# endif
#endif

float3 UTS_SelfShdowMainLight(LightLoopContext lightLoopContext, FragInputs input, int mainLightIndex, out float inverseClipping, out float channelOutAlpha, out UTSData utsData)
{
    channelOutAlpha = 1.0f;
    uint2 tileIndex = uint2(input.positionSS.xy) / GetTileSize();
    inverseClipping = 0;
    // input.positionSS is SV_Position
    PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, tileIndex);


#ifdef VARYINGS_NEED_POSITION_WS
    float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
#else
    // Unused
    float3 V = float3(1.0, 1.0, 1.0); // Avoid the division by 0
#endif

    SurfaceData surfaceData;
    BuiltinData builtinData;
    GetSurfaceAndBuiltinData(input, V, posInput, surfaceData, builtinData);

    BSDFData bsdfData = ConvertSurfaceDataToBSDFData(input.positionSS.xy, surfaceData);

    PreLightData preLightData = GetPreLightData(V, posInput, bsdfData);
    /* todo. these should be put int a struct */
    float4 Set_UV0 = input.texCoord0;
    float3x3 tangentTransform = input.tangentToWorld;
    //UnpackNormalmapRGorAG(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, texCoords))
    float4 n = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, Set_UV0.xy);
    //    float3 _NormalMap_var = UnpackNormalScale(tex2D(_NormalMap, TRANSFORM_TEX(Set_UV0, _NormalMap)), _BumpScale);
    float3 _NormalMap_var = UnpackNormalScale(n, _BumpScale);
    float3 normalLocal = _NormalMap_var.rgb;
    utsData.normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals

    float4 _MainTex_var = 1.0; //  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex));
    float3 i_normalDir = surfaceData.normalWS;
    utsData.viewDirection = V;
    /* to here todo. these should be put int a struct */




    float shadowAttenuation = (float)lightLoopContext.shadowValue;


    float3 mainLihgtDirection = -_DirectionalLightDatas[mainLightIndex].forward;
    float3 mainLightColor = ApplyCurrentExposureMultiplier(_DirectionalLightDatas[mainLightIndex].color);
    //    float4 tmpColor = EvaluateLight_Directional(context, posInput, _DirectionalLightDatas[mainLightIndex]);
    //    float3 mainLightColor = tmpColor.xyz;
    float3 defaultLightDirection = normalize(UNITY_MATRIX_V[2].xyz + UNITY_MATRIX_V[1].xyz);
    float3 defaultLightColor = saturate(max(float3(0.05, 0.05, 0.05) * _Unlit_Intensity, max(ShadeSH9(float4(0.0, 0.0, 0.0, 1.0)), ShadeSH9(float4(0.0, -1.0, 0.0, 1.0)).rgb) * _Unlit_Intensity));
    float3 customLightDirection = normalize(mul(UNITY_MATRIX_M, float4(((float3(1.0, 0.0, 0.0) * _Offset_X_Axis_BLD * 10) + (float3(0.0, 1.0, 0.0) * _Offset_Y_Axis_BLD * 10) + (float3(0.0, 0.0, -1.0) * lerp(-1.0, 1.0, _Inverse_Z_Axis_BLD))), 0)).xyz);
    float3 lightDirection = normalize(lerp(defaultLightDirection, mainLihgtDirection.xyz, any(mainLihgtDirection.xyz)));
    lightDirection = lerp(lightDirection, customLightDirection, _Is_BLD);
    float3 originalLightColor = mainLightColor;

    originalLightColor = lerp(originalLightColor, clamp(originalLightColor, ConvertFromEV100(_ToonEvAdjustmentValueMin), ConvertFromEV100(_ToonEvAdjustmentValueMax)), _ToonEvAdjustmentCurve);
    float3 lightColor = lerp(max(defaultLightColor, originalLightColor), max(defaultLightColor, saturate(originalLightColor)), max(_Is_Filter_LightColor, _ToonLightHiCutFilter));


    ////// Lighting:
    float3 halfDirection = normalize(utsData.viewDirection + lightDirection);
    //v.2.0.5
    _Color = _BaseColor;
    float3 Set_LightColor = lightColor.rgb;
    float3 Set_BaseColor = lerp((_MainTex_var.rgb * _BaseColor.rgb), ((_MainTex_var.rgb * _BaseColor.rgb) * Set_LightColor), _Is_LightColor_Base);
    float3 clippingColor = float3(1.0f, 1.0f, 1.0f);


    //v.2.0.5
    float4 _1st_ShadeMap_var = float4(0.0, 0.0, 0.0, 1.0);
    Set_LightColor = 0.0;
    float3 Set_1st_ShadeColor = lerp((_1st_ShadeColor.rgb * _1st_ShadeMap_var.rgb), ((_1st_ShadeColor.rgb * _1st_ShadeMap_var.rgb) * Set_LightColor), _Is_LightColor_1st_Shade);


    //v.2.0.5
    float4 _2nd_ShadeMap_var = float4(0.0, 0.0, 0.0, 1.0);
    float3 Set_2nd_ShadeColor = lerp((_2nd_ShadeColor.rgb * _2nd_ShadeMap_var.rgb), ((_2nd_ShadeColor.rgb * _2nd_ShadeMap_var.rgb) * Set_LightColor), _Is_LightColor_2nd_Shade);
//    float _HalfLambert_var = 0.5 * dot(lerp(i_normalDir, utsData.normalDirection, _Is_NormalMapToBase), lightDirection) + 0.5;
    float _HalfLambert_var =dot(i_normalDir, lightDirection);
    float4 _Set_2nd_ShadePosition_var = 1.0; // tex2D(_Set_2nd_ShadePosition, TRANSFORM_TEX(Set_UV0, _Set_2nd_ShadePosition));
    float4 _Set_1st_ShadePosition_var = 1.0; // tex2D(_Set_1st_ShadePosition, TRANSFORM_TEX(Set_UV0, _Set_1st_ShadePosition));


    float _2ndColorFeatherForMask = 0; // lerp(_1st2nd_Shades_Feather, 0.0f, max(_SecondShadeOverridden, _ComposerMaskMode));
    
    _BaseColor_Step = 0.00001;
    _ShadeColor_Step = 0;
    //v.2.0.6
    //Minmimum value is same as the Minimum Feather's value with the Minimum Step's value as threshold.
    float Set_FinalShadowMask = saturate((1.0 + (_HalfLambert_var * ( - _Set_1st_ShadePosition_var.r )) / (_BaseColor_Step )));
    //
    //Composition: 3 Basic Colors as Set_FinalBaseColor
    float Set_BaseColorAlpha = 1.0;
    float Set_1st_ShadeAlpha = 1.0;
    float Set_2nd_ShadeAlpha = 1.0;
    float3 Set_FinalBaseColor = lerp(Set_BaseColor, lerp(Set_1st_ShadeColor, Set_2nd_ShadeColor, saturate((1.0 + ((_HalfLambert_var - (_ShadeColor_Step - _2ndColorFeatherForMask)) * ((1.0 - _Set_2nd_ShadePosition_var.rgb).r - 1.0)) / (_ShadeColor_Step - (_ShadeColor_Step - _2ndColorFeatherForMask))))), Set_FinalShadowMask); // Final Color
    channelOutAlpha = lerp(Set_BaseColorAlpha, lerp(Set_1st_ShadeAlpha, Set_2nd_ShadeAlpha, saturate((1.0 + ((_HalfLambert_var - (_ShadeColor_Step - _2ndColorFeatherForMask)) * ((1.0 - _Set_2nd_ShadePosition_var.rgb).r - 1.0)) / (_ShadeColor_Step - (_ShadeColor_Step - _2ndColorFeatherForMask))))), Set_FinalShadowMask);

    utsData.signMirror = 0.0; // i.mirrorFlag; todo.
    utsData.cameraRoll = 0.0;
    utsData.cameraDir = -1;
    return Set_FinalBaseColor;


}
