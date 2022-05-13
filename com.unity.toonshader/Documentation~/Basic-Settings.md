# Three Color Map and Control Map Settings

- [Base Map](#base-map)
- [Apply to 1st Shading Map](#apply-to-1st-shading-map)
- [1st Shading map](#1st-shading-map)
- [Apply to 2nd Shading Map](#apply-to-2nd-shading-map)
- [2nd Shading Map](#2nd-shading-map)
- [Normal Map](#normal-map)
- [NormalMap Effectiveness](#normalmap-effectiveness)
- [3 Basic Colors](#3-basic-colors)
- [Highlight](#highlight)
- [Rim Light](#rim-light)
- [Shadow Control Maps](#shadow-control-maps)
- [1st Shading Position Map](#1st-shading-map)
- [2nd Shading Position Map](#2nd-shading-map)

### Base Map
Base Color : Texture(sRGB) × Color(RGB). The defaula color is White.

|  Base Color Map (Face) | (Hair) | Result  |
| ---- | ---- |---- |
| <img src="images/yuko_face3_main.png" height="256">  |<img src="images/yuko_hair.png" height="256"> |<img src="images/YukoFace.png" height="256">  |


### Apply to 1st Shading Map
Apply Base Map to the 1st shading map. When **Apply to 1st Shading Map** is checked, texture map in **1st Shading Map** is not used fore redering and its texture UI is disabled.

Please refer to the image at [Apply to 2nd Shading Map](#apply-to-2nd-shading-map).


### 1st Shading Map
The map used for the brighter portions of the shadow. Texture(sRGB) × Color(RGB). The defaula color is White.
|  with 1st Shading Map (Face) | (Hair) | Result  |
| ---- | ---- | ---- |
| <img src="images/yuko_face3_B.png" height="256">   | <img src="images/yuko_hairB.png" height="256"> |<img src="images/YukoFace1stShadingMap.png" height="256">  |


### Apply to 2nd Shading Map
Apply **Base Map** or the **1st shading Map** to the **2nd Shading Map**. When **Apply to 2nd Shading Map** is checked, texture map in **2nd Shading Map** is not used fore redering and its texture UI is disabled.

<img src="images/ApplyTo1st2ndMap-3.gif" height="384"> 

### 2nd Shading Map
The map used for the darker portions of the shadow. Texture(sRGB) × Color(RGB). The defaula color is White.
|  with 2nd Shading Map (Face)  | (Hair) | Result  |
| ---- | ---- | ---- |
| <img src="images/yuko_face3_C.png" height="256">   | <img src="images/yuko_hairC.png" height="256"> |<img src="images/YukoFace2ndShadingMap.png" height="256">  |
### Normal Map
A texture that dictates the bumpiness of the material. The slider is for controlling strength.
| Normal Map | Result  |
| ---- | ---- |
| <img src="images/DecoPlane_Nromal.png" height="256"> |<img src="images/NormalMapSample.png" height="256">  |

<img src="images/UTSNrormalMap-2.gif" height="256"> 

## NormalMap Effectiveness

### 3 Basic Colors
Normal map effectiveness to 3 Basic color areas, lit, the 1st shading and the 2nd.

### Highlight
Normal map effectiveness to high lit areas.

### Rim Light
Normal map effectiveness to rim lit areas.




## Shadow Control Maps
### 1st Shading Position Map
Specify the position of fixed shadows that falls in 1st shade color areas in UV coordinates. 1st Position Map : Texture(linear)

### 2nd Shading Position Map
Specify the position of fixed shadows that falls in 2nd shade color areas in UV coordinates. 2nd Position Map : Texture(linear)
