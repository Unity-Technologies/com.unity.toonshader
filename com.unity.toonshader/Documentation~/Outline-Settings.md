# Outline Settings

* [Outline](#outline)
  * [Outline Mode](#outline-mode)
  * [Outline Width](#outline-width)
  * [Outline Color](#outline-color)
  * [Blend Base Color to Outline](#blend-base-color-to-outline)
  * [Outline Width Map](#outline-width-map)
  * [Offset Outline with Camera Z-axis](#offset-outline-with-camera-z-axis)
  * [Camera Distance for Outline Width](#camera-distance-for-outline-width)
    * [Farthest Distance to vanish](#farthest-distance-to-vanish)
    * [Nearest Distance to draw with Outline Width](#nearest-distance-to-draw-with-outline-width)

  * [Outline Color Map](#outline-color-map)
  * [Baked Normal Map](#baked-normal-map)

## Outline 

| Outline Off | Outline On |
| -- | -- | 
| <img src="images/OutlineOff.png" height="256"> | <img src="images/OutlineOn.png" height="256"> |

### Outline Mode
Specifies how the inverted-outline object is spawned.

### Outline Width
Specifies the width of the outline. This value relies on the scale when the model was imported to Unity

### Outline Color
Specifies the color of outline.
### Blend Base Color to Outline
Base Color is blended into outline color.
### Outline Width Map
Outline Width Map as Grayscale Texture : Texture(linear).
### Offset Outline with Camera Z-axis
Offsets the outline in the depth (Z) direction of the camera.
### Camera Distance for Outline Width
#### Farthest Distance to vanish
Specify the furthest distance, where the outline width changes with the distance between the camera and the object. The outline will be zero at this distance.
#### Nearest Distance to draw with Outline Width
Specify the closest distance, where the outline width changes with the distance between the camera and the object. At this distance, the outline will be the maximum width set by Outline_Width.
### Outline Color Map
Apply a texture as outline color map.
### Baked Normal Map
Normal maps with vertex normals previously baked in from other models can be loaded as an addition when setting up normal inversion outlines. 