# Material Capture(MatCap) Settings

* [MatCap Map](#matcap-map)
* [MatCap Blur Level](#matcap-blur-level)
* [Color Blending Mode](#color-blending-mode)
* [Scale MatCap UV](#scale-matcap-uv)
* [Rotate MatCap UV](#rotate-matcap-uv)
* [Stabilize Camera rolling](#stabilize-camera-rolling)
* [Normal Map Specular Mask for MatCap](#normal-map-specular-mask-for-matcap)
* [Normal Map](#normal-map)
* [Rotate Normal Map UV](#rotate-normal-map-uv)
* [MatCap Blending on Shadows](#matcap-blending-on-shadows)
* [Blending Level](#blending-level)
* [MatCap Camera Mode](#matcap-camera-mode)
* [MatCap Mask](#matcap-mask)
* [MatCap Mask Level](#matcap-mask-level)
* [Invert MatCap Mask](#invert-matcap-mask)

### MatCap Map
MatCap Color : Texture(sRGB) × Color(RGB) Default:White

### MatCap Blur Level
Blur MatCap Map using the Mip Map feature; to enable Mip Map, turn on Advanced > Generate Mip Maps in the Texture Import Settings. Default is 0 (no blur)

### Color Blending Mode
MatCap color blending mode. Multiply or Additive.

### Scale MatCap UV
Scaling UV of MatCap Map.

### Rotate MatCap UV
Rotating UV of MatCap Map.

### Stabilize Camera rolling
Stabilize Camera rolling when capturing materials with camera.
### Normal Map Specular Mask for MatCap
If enabled, gives a normal map specifically for MatCap.If you are using MatCap as speculum lighting, you can use this to mask it.
### Normal Map
A texture that dictates the bumpiness of the material.
### Rotate Normal Map UV
Rotates the MatCap normal map UV based on its center.

### MatCap Blending on Shadows
Adjusts the blending rate of the MatCap range in shadows.
### Blending Level
Adjusts the intensity of MatCap applied to shadow areas.
### MatCap Camera Mode
Control how the MatCap Map is rendered based on the type of camera.
### MatCap Mask
The MatCap mask is positioned with respect to the UV coordinates of the mesh onto which the MatCap is projected, and the pixels on black areas are hidden.
### MatCap Mask Level
Adjusts the level of the MatCap Mask. When the value is 1, MatCap is displayed 100% irrespective of whether or not there is a mask. When the value is -1, MatCap will not be displayed at all and MatCap will be the same as in the off state.
### Invert MatCap Mask
When enabled, MatCap Mask Texture is inversed.
