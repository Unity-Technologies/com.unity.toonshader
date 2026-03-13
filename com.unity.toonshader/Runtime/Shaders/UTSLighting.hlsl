#ifndef UTS_LIGHTING_INCLUDED
#define UTS_LIGHTING_INCLUDED

inline float Intensity(float3 lightColor)
{
    return 0.299 * lightColor.r + 0.587 * lightColor.g + 0.114 * lightColor.b;
}

//----------------------------------------------------------------------------------------------------------------------

float3 ThreeColorsLinearShading(
    float3 baseColor,
    float3 firstColor,
    float3 secondColor,
    float  baseTo1stStart,     // t=0: use base, t=1: transition
    float  baseTo1stFeather,
    float  firstToSecondStart, //t=0: use base, t=1: transition
    float  firstToSecondFeather,
    float  dotNL) // dot(N.L)
{
    const float t = saturate(1 - dotNL); //t = 0: light, t=1: dark shaded

    const float invBaseTo1stStart = 1 - baseTo1stStart;
    const float invBaseTo2ndStart = 1 - firstToSecondStart;
                
    const float s1 = smoothstep(invBaseTo1stStart, invBaseTo1stStart + baseTo1stFeather,t); 
    const float s2 = smoothstep(invBaseTo2ndStart, invBaseTo2ndStart + firstToSecondFeather,t); 
                
    float3 c01 = lerp(baseColor,firstColor,  s1);
    float3 c12 = lerp(c01, secondColor, s2);
    return c12;
}

#endif // UTS_LIGHTING_INCLUDED
