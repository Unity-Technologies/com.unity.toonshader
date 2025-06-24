# Shading Steps and Feather Settings

The **Unity Toon Shader** allows you to control the position of the boundaries of the area, to clearly demarcate the boundaries, or to blur or blend them.

* [Base Color Step](#base-color-step)
* [Base Shading Feather](#base-shading-feather)
* [Shading Color Step](#shading-color-step)
* [1st/2nd Shading Feather](#1st2nd-shading-feather)
* [Receive Shadows](#receive-shadows)
  * [System Shadow Level](#system-shadow-level)
* [Point Light Settings](#point-light-settings)
  * [Step Offset](#step-offset)
  * [Filter Point Light Highlights](#filter-point-light-highlights)


### Base Color Step
Sets the boundary between the Base Color and the Shade Colors.

<video title="The head of a chibi-style character model with feathered hair and cat ears. The shadow on the chin of the character grows more visible and sharper-edged." src="images/ColorStep.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>

## Base Shading Feather
Feathers the boundary between the Base Color and the Shade Colors.

<video title="The same chibi-style character. The visibility of the shadow on the chin changes, but the shadow doesn't disappear entirely." src="images/BaseShadingFeather.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>

## Shading Color Step
Sets the boundary between the 1st and 2nd Shade Colors. Set this to 0 if  2nd Shade Color is unnecessary.

<video title="The same chibi-style character. The lighter-colored shadow on the chin is gradually replaced with a darker shadow." src="images/ShadingColorStep.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>


### 1st/2nd Shading Feather
Feathers the boundary between the 1st and 2nd Shade Colors.

<video title="The same chibi-style character. The lighter-colored shadow on the chin is gradually replaced with a darker feathered shadow." src="images/1st2ndShadeFeather.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>

### Receive Shadows
Determine if the material reflects shadows.

<video title="The same chibi-style character. Shadows from the hair onto the face and eyes appear and disappear." src="images/RecieveSystemShadow.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>


#### System Shadow Level
Define the appearance of self-shadows and other received shadows that blend with the Too Shader.

<video title="The same chibi-style character. The face starts with only a shadow on the chin, then the shadows grow. The hair casts shadows onto the face, then the whole face is in shadow." src="images/SystemShadowLevel.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>


## Point Light Settings

### Step Offset
Fine tunes light steps (boundaries) added in the ForwardAdd pass, such as real-time point lights.


<video title="The same chibi-style character. Bands of red, orange, and yellow light grow and shrink on the hair." src="images/PointLightStepOffset.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>
<video title="A toon-shaded sphere in a room textured with graphs. The sphere has green and purple bands of color, which grow and shrink." src="images/PointLightStepOffset-Ball3.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>



### Filter Point Light Highlights
Show or hide highlight of point lights.

<video title="The head of a chibi-style character model with feathered hair and cat ears. The face is lit with a purple light, and small bright specular highlights appear and disappear." src="images/FilterHilightOnPointLight-2.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>
<video title="A toon-shaded sphere in a room textured with graphs. The sphere has green and purple bands of color, and bright circular highlights which appear and disappear." src="images/FilterPointLightHighlight-Ball.mp4" width="auto" height="auto" autoplay="true" loop="true" controls></video>
