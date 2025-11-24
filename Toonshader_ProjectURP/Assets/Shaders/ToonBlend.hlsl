#pragma once

float3 ToonDiffuseBlend(float3 shapeLight, float _ShapeLightBlendFactors) {

    const float3 directionalLightColorAndUse = _DirectionalLight_Color * _DirectionalLight_Use; 

    const float3 diffuseLightFactor = (shapeLight.rgb * _ShapeLightBlendFactors * _2DLightStrength )
        + (directionalLightColorAndUse * _DirectionalLight_DiffuseStrength);

    return diffuseLightFactor;
}
