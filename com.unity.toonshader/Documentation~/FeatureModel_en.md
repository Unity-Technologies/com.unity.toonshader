## Feature Differences in Each Render Pipeline

| Function                                                                                   | Built-In           | URP                | HDRP               |
|--------------------------------------------------------------------------------------------|--------------------|--------------------|--------------------|
| ***1. Modes***                                                                             |                    |                    |                    |
| Standard                                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| With Advanced Control Map                                                                  | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***2. Shader Settings***                                                                   |                    |                    |                    |
| Culling                                                                                    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Stencil                                                                                    | :heavy_check_mark: | :heavy_check_mark: |                    |
| Stencil Value                                                                              | :heavy_check_mark: | :heavy_check_mark: |                    |
| Clipping                                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Clipping Mask                                                                              | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Invert  Clipping Mask                                                                      | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Use Base Map Alpha as Clipping Mask                                                        | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***3. Three Color Map and Control Map Settings***                                          |                    |                    |                    |
| Base Map                                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| 1st Shading Map                                                                            | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| 2nd Shading Map                                                                            | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Normal Map                                                                                 | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Shadow Control Maps                                                                        | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***4. Shading Steps and Feather Settings***                                                |                    |                    |                    |
| Base Color Step                                                                            | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Base Shading Feather                                                                       | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Shading Color Step                                                                         | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Point Light Step Offset                                                                    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Filter Point Light Highlights                                                              | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***5. Highlight Settings***                                                                |                    |                    |                    |
| Highlight Power                                                                            | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Specular Mode                                                                              | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Color Blending Mode                                                                        | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Highlight Blending on Shadows                                                              | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Highlight Mask                                                                             | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Highlight Mask Level                                                                       | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***6. Rim Light Settings***                                                                |                    |                    |                    |
| Rim Light Color                                                                            | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Rim Light Level                                                                            | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Adjust Rim Light Area                                                                      | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Inverted Light Direction Rim Light                                                         | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Rim Light Mask                                                                             | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***7. Material Capture(MatCap) Settings***                                                 |                    |                    |                    |
| MatCap Map                                                                                 | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| MatCap Blur Level                                                                          | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Color Blending Mode                                                                        | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Scale MatCap UV                                                                            | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Rotate MatCap UV                                                                           | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Stabilize Camera Rolling                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Normal Map                                                                                 | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Rotate Normal Map UV                                                                       | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| MatCap Blending on Shadows                                                                 | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| MatCap Camera Mode                                                                         | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| MatCap Mask                                                                                | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| MatCap Mask Level                                                                          | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Invert MatCap Mask                                                                         | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***8. Emission Settings***                                                                 |                    |                    |                    |
| Emission Map                                                                               | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Use the alpha channel of Emissive Map as a Clipping mask                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Emission Map Animation                                                                     | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Base Speed (Time)                                                                          | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Animation Mode                                                                             | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Scroll U/X direction                                                                       | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Scroll V/Y direction                                                                       | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Rotate around UV center                                                                    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Ping-pong moves for base                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Color Shifting with Time                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Color Shifting with View Angle                                                             | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***9. Angel Ring Projection Settings***                                                    |                    |                    |                    |
| Angel Ring                                                                                 | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Offset U/V                                                                                 | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Alpha Channel as Clipping Mask                                                             | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***10. [Scene Light Effectiveness Settings](SceneLight.md) for all UTS color properties*** | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***11. Metaverse Settings***                                                               |                    |                    |                    |
| Metaverse Light                                                                            | :heavy_check_mark: | :heavy_check_mark: |                    |
| Metaverse Light Intensity                                                                  | :heavy_check_mark: | :heavy_check_mark: |                    |
| Metaverse Light Direction                                                                  | :heavy_check_mark: | :heavy_check_mark: |                    |
| ***12. Outline Settings***                                                                 |                    |                    |                    |
| Outline Mode                                                                               | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Outline Width                                                                              | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Outline Color                                                                              | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Blend Base Color to Outline                                                                | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Offset Outline with Camera Z-axis                                                          | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Camera Distance for Outline Width                                                          | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Outline Color Map                                                                          | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Rotate around UV center                                                                    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Ping-pong moves for base                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Color Shifting with Time                                                                   | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Color Shifting with View Angle                                                             | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| ***13.Tessellation Settings***                                                             |                    |                    |                    |
| Tessellation Settings                                                                      | DX11/Vulkan/Metal  | :x:                | DX11/Vulkan/Metal  |
| ***14. EV Adjustment***                                                                    |                    |                    |                    |
| EV Adjustment in high intensity light scenes                                               | :x:                | :x:                | :heavy_check_mark: |
| ***15. Render pipeline built-in ray-traced shadows***                                      |                    |                    |                    |
| DXR shadow supported in render pipelines                                                   | :x:                | :x:                | :heavy_check_mark: |
| ***16. [Box Light](HDRPBoxLight.md)***                                                     |                    |                    |                    |
| Substitute for directional light                                                           | :x:                | :x:                | :heavy_check_mark: |
| ***17. Rendering Paths***                                                                  |                    |                    |                    |
| Forward                                                                                    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| Forward+                                                                                   | :x:                | :heavy_check_mark: | :x:                |
| Deferred                                                                                   |                    |                    | :heavy_check_mark: |
| Deferred+                                                                                  | :x:                |                    | :x:                |

Notes:
* :heavy_check_mark: : Supported
* :x: : Not supported (e.g. limitations in the render pipeline, etc)
* (blank) : Potential future support (not currently available)
