# Three Color Map and Control Map Settings

**Three Color Map and Control Map Settings** provide basic cel-shading settings in the **Unity Toon Shader**. These settings allow you to control the rendering of light and shadow areas independently from the actual light color. UTS provides detailed control over whether the directional light color affects materials. Please refer to [Scene Light Effectiveness Settings](SceneLight.md) for more information.

* [Three Basic Color Maps](#Three-basic-color-maps)
  * [Base Map](#base-map)
    * [Apply to 1st Shading Map](#apply-to-1st-shading-map)
  * [1st Shading map](#1st-shading-map)
    * [Apply to 2nd Shading Map](#apply-to-2nd-shading-map)
  * [2nd Shading Map](#2nd-shading-map)
  * [Example of Three Color Map Operation](#example-of-Three-color-map-operation)
<br><br>


* [Shadow Control Maps](#shadow-control-maps)
  * [1st Shading Position Map](#1st-shading-map)
  * [2nd Shading Position Map](#2nd-shading-map)
  * [Example of Shadow Control Map Application](#example-of-shadow-control-map-application)
<br><br>

## Three Basic Color Maps

## Base Map
Base Color: Texture(sRGB) × Color(RGB). The default color is white. The base color represents the color of the unshaded areas of objects or characters.

|  Base Color Map (Face) | (Hair) | Result  |
| ---- | ---- |---- |
| <img alt="A yellow texture map with different-colored areas for skin, ears, eyes, cheeks, and other parts of a face." src="images/yuko_face3_main.png" height="256">  | <img alt="A grey texture map with two brown areas for cat-like ears, and lighter grey brushstrokes for parts of the hair." src="images/yuko_hair.png" height="256"> |<img alt="A chibi-style face with yellow skin, grey hair, brown cat ears, large eyes, and rosy cheeks." src="images/YukoFace.png" height="256">  |


### Apply to 1st Shading Map
Apply **Base Map** to the **1st Shading Map**. When you check **Apply to 1st Shading Map**, the texture map in **1st Shading Map** is not applied for rendering and the Inspector window disables its texture UI.


## 1st Shading Map
The map used for the brighter portions of the shadow. Texture(sRGB) × Color(RGB). The default color is white.

|   **1st Shading Map** (Face) | (Hair) | Result  |
| ---- | ---- | ---- |
| <img alt="A similar texture map to the base map, but the background is now tan." src="images/yuko_face3_B.png" height="256">   | <img alt="A similar hair map to the base map, but the background is darker, and the brushstrokes have a blue gradient." src="images/yuko_hairB.png" height="256"> |<img alt="The chibi-style face, now with shadows at the bottom of the hair and face, and over the eyes." src="images/YukoFace1stShadingMap.png" height="256">  |


### Apply to 2nd Shading Map
Apply **Base Map** or the **1st Shading Map** to the **2nd Shading Map**. When you check **Apply to 2nd Shading Map**, the texture map in **2nd Shading Map** is not applied for rendering and the Inspector window disables its texture UI.


### 2nd Shading Map
The map used for the darker portions of the shadow. Texture(sRGB) × Color(RGB). The default color is white.

|  **2nd Shading Map** (Face)  | (Hair) | Result  |
| ---- | ---- | ---- |
| <img alt="A similar texture map to the base map, but the background is now a dark orange." src="images/yuko_face3_C.png" height="256">   | <img alt="A similar hair map to the base map, but the background is even darker." src="images/yuko_hairC.png" height="256"> |<img alt="The chibi-style face, now with darker shadows at the edges of the hair." src="images/YukoFace2ndShadingMap.png" height="256">  |


## Shadow Control Maps
Textures that dictate the fixed shadows of the material.

### 1st Shading Position Map
Specifies the position of fixed shadows that fall in 1st shade color areas in UV coordinates. **1st Position Map**: Texture(linear).

### 2nd Shading Position Map
Specifies the position of fixed shadows that fall in 2nd shade color areas in UV coordinates. **2nd Position Map**: Texture(linear).


<br><br>
## Example of Shadow Control Map Application
| Base Map | 1st Shading Map | Shading Position Map |
| ---- | ---- | ---- |
| <img alt="A UV map texture that contains all the parts of a chibi-style character model." src="images/utc_all2_light.png" height="256"> |<img alt="The same UV map but some areas have a darker color." src="images/utc_all2_dark.png" height="256"> |<img alt="A mostly white texture, with 3 black hair shapes." src="images/utc_all2_offsetdark.png" height="256"> |

No Shadow Control Maps:
![A chibi-style character model with rabbit ears. In the Inspector window, the 1st Shading Position Map and 2nd Shading Position Map properties are empty.](images/ShadowControlMap0.png)

1st Shading Position Map:
![The same model. In the Inspector window, the 1st Shading Position Map property is set to the shading position map texture.](images/ShadowControlMap1.png)

2nd Shading Position Map:
![The same model. In the Inspector window, the 2nd Shading Position Map property is set to the shading position map texture.](images/ShadowControlMap2.png)

Both:
![The same model. In the Inspector window, both the 1st Shading Position Map and 2nd Shading Position Map properties are set to the shading position map texture. ](images/ShadowControlMap3.png)
