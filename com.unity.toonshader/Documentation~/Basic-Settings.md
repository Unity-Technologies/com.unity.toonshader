# Three Color Map and Control Map Settings

- [Base Map](#base-map)
- [Apply to 1st Shading map](#apply-to-1st-shading-map)
- [1st Shading map](#1st-shading-map)
- [Apply to 2nd Shading map](#apply-to-2nd-shading-map)
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
Base Color : Texture(sRGB) × Color(RGB) Default:White

|  Base Color Map(Face) | Base Color Map(Hair) | Result  |
| ---- | ---- |
|   | <img src="images/YukoFace.png" width="240">  |

### Apply to 1st Shading map
Apply Base map to the 1st shading map.

### 1st Shading map
The map used for the brighter portions of the shadow.
|  1st Shading map  | Result  |
| ---- | ---- |
|   | <img src="images/YukoFace1stShadingMap.png" width="240">  |


### Apply to 2nd Shading map
Apply Base map or the 1st shading map to the 2st shading map.

### 2nd Shading Map
The map used for the darker portions of the shadow.

### Normal Map
A texture that dictates the bumpiness of the material.

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
