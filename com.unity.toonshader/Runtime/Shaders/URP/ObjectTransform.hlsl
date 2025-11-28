#pragma once

inline float4 UnityObjectToClipPosInstanced(in float3 pos) {
    return mul(UNITY_MATRIX_VP, mul(GetObjectToWorldMatrix(), float4(pos, 1.0)));
}
#define UnityObjectToClipPos UnityObjectToClipPosInstanced
            
            
inline float3 UnityObjectToWorldNormal(in float3 norm)
{
    #ifdef UNITY_ASSUME_UNIFORM_SCALING
    return UnityObjectToWorldDir(norm);
    #else
    // mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
    return normalize(mul(norm, (float3x3)GetWorldToObjectMatrix()));
    #endif
}
