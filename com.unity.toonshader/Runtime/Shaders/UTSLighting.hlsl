#ifndef UTS_LIGHTING_INCLUDED
#define UTS_LIGHTING_INCLUDED

inline float UTS_CalculateLightColorIntensity(float3 lightColor)
{
    return 0.299 * lightColor.r + 0.587 * lightColor.g + 0.114 * lightColor.b;
}

#endif // UTS_LIGHTING_INCLUDED
