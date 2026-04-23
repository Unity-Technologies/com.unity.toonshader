#ifndef URP_INCLUDE_GUARDS_INCLUDED
#define URP_INCLUDE_GUARDS_INCLUDED

//This file is used to block include files inside com.unity.render-pipelines.universal package due to 
//custom toon requirements

#define UNIVERSAL_LIT_INPUT_INCLUDED //LitInput.hlsl: Toon has a custom CBUFFER 

#endif // URP_INCLUDE_GUARDS_INCLUDED