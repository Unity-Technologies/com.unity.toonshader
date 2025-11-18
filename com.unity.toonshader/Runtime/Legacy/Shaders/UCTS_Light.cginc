//Note: requires VertexOutput to be defined before including this file

#ifndef __UCTS_LIGHT__
#define __UCTS_LIGHT__

#include "AutoLight.cginc"
#define UTS_LIGHT_ATTENUATION(destName, input, worldPos)    UNITY_LIGHT_ATTENUATION(destName, input, worldPos)

#endif //__UCTS_LIGHT__

