# Normal Map Settings

Normal Map is a texture that dictates the bumpiness of the material. The **Unity Toon Shader** allows control over **Normal Map** strength and which areas it applies to.

* [Normal Map](#normal-map)
* [Normal Map Effectiveness](#normal-map-effectiveness)
* [Example of Normal Map Effectiveness Operation](#example-of-normal-map-effectiveness-operation)
<br><br>

## Normal Map
A Normal Map texture and its strength.

![A example of a normal map. A square surface with 16 small raised squares in a regular layout. The edges of the raised squares use a different color for each normal direction.](images/DecoPlane_Nromal.png)

Examples of a normal map with a strength of 1.0, 0.3, and 0.0.

![The example normal applied to a plane using the URP/Lit shader on the left side and the UTS shader on the right side, and a strength of 1.0. The raised squares are clearly visible on both halves.](images/NormalMapStrength1.png)

![The same example with a strength of 0.3. The raised squares are less visible on the UTS side.](images/NormalMapStrength03.png)

![The same example with a strength of 0.0. The raised squares are not visible on the UTS side.](images/NormalMapStrength00.png)

## Normal Map Effectiveness


|Properties| Description |
| ---- | ---- |
| Three Basic Colors | The effectiveness of the Normal Map on Three Basic color areas, lit, the 1st shading and the 2nd. |
| Highlight | Normal map effectiveness to high lit areas. |
| Rim Light | Normal map effectiveness to rim lit areas. |

<br/><br/>

## Example of Normal Map Effectiveness Operation

<img src="images/NormalmapEffectiveness.gif" height="256">  


