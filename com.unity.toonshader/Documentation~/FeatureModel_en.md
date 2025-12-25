## Feature Differences in Each Render Pipeline

| Function                                                                                   | Built-In               | URP                | URP 3D as 2D (Unity6.3+) | HDRP                   |
|--------------------------------------------------------------------------------------------|------------------------|--------------------|--------------------------|------------------------|
| ***1. Modes***                                                                             |                        |                    |                       |                        |
| &ensp; Standard                                                                            | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; With Advanced Control Map                                                           | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***2. Shader Settings***                                                                   |                        |                    |                       |                        |
| &ensp; Culling                                                                             | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Stencil                                                                             | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :x:                    |
| &ensp; Stencil Value                                                                       | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :x:                    |
| &ensp; Clipping                                                                            | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Clipping Mask                                                                       | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Invert  Clipping Mask                                                               | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Use Base Map Alpha as Clipping Mask                                                 | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***3. Three Color Map and Control Map Settings***                                          |                        |                    |                       |                        |
| &ensp; Base Map                                                                            | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; 1st Shading Map                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; 2nd Shading Map                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Normal Map                                                                          | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Shadow Control Maps                                                                 | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***4. Shading Steps and Feather Settings***                                                |                        |                    |                       |                        |
| &ensp; Base Color Step                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Base Shading Feather                                                                | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Shading Color Step                                                                  | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Point Light Step Offset                                                             | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Filter Point Light Highlights                                                       | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***5. Highlight Settings***                                                                |                        |                    |                       |                        |
| &ensp; Highlight Power                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Specular Mode                                                                       | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Color Blending Mode                                                                 | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Highlight Blending on Shadows                                                       | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Highlight Mask                                                                      | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Highlight Mask Level                                                                | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***6. Rim Light Settings***                                                                |                        |                    |                       |                        |
| &ensp; Rim Light Color                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Rim Light Level                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Adjust Rim Light Area                                                               | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Inverted Light Direction Rim Light                                                  | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Rim Light Mask                                                                      | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***7. Material Capture(MatCap) Settings***                                                 |                        |                    |                       |                        |
| &ensp; MatCap Map                                                                          | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; MatCap Blur Level                                                                   | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Color Blending Mode                                                                 | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Scale MatCap UV                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Rotate MatCap UV                                                                    | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Stabilize Camera Rolling                                                            | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Normal Map                                                                          | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Rotate Normal Map UV                                                                | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; MatCap Blending on Shadows                                                          | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; MatCap Camera Mode                                                                  | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; MatCap Mask                                                                         | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; MatCap Mask Level                                                                   | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Invert MatCap Mask                                                                  | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***8. Emission Settings***                                                                 |                        |                    |                       |                        |
| &ensp; Emission Map                                                                        | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Use the alpha channel of Emissive Map as a Clipping mask                            | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Emission Map Animation                                                              | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Base Speed (Time)                                                                   | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Animation Mode                                                                      | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Scroll U/X direction                                                                | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Scroll V/Y direction                                                                | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Rotate around UV center                                                             | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Ping-pong moves for base                                                            | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Color Shifting with Time                                                            | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Color Shifting with View Angle                                                      | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***9. Angel Ring Projection Settings***                                                    |                        |                    |                       |                        |
| &ensp; Angel Ring                                                                          | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Offset U/V                                                                          | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Alpha Channel as Clipping Mask                                                      | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***10. [Scene Light Effectiveness Settings](SceneLight.md) for all UTS color properties*** | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***11. Metaverse Settings***                                                               |                        |                    |                       |                        |
| &ensp; Metaverse Light                                                                     | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :x:                    |
| &ensp; Metaverse Light Intensity                                                           | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :x:                    |
| &ensp; Metaverse Light Direction                                                           | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :x:                    |
| ***12. Outline Settings***                                                                 |                        |                    |                       |                        |
| &ensp; Outline Mode                                                                        | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Outline Width                                                                       | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Outline Color                                                                       | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Blend Base Color to Outline                                                         | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Offset Outline with Camera Z-axis                                                   | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Camera Distance for Outline Width                                                   | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Outline Color Map                                                                   | :heavy_check_mark:     | :heavy_check_mark: | :heavy_check_mark:    | :heavy_check_mark:     |
| &ensp; Rotate around UV center                                                             | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Ping-pong moves for base                                                            | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Color Shifting with Time                                                            | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| &ensp; Color Shifting with View Angle                                                      | :heavy_check_mark:     | :heavy_check_mark: | :x:                   | :heavy_check_mark:     |
| ***13.Tessellation Settings***                                                             | DX11/DX12/Vulkan/Metal | :no_entry_sign:    | :no_entry_sign:       | DX11/DX12/Vulkan/Metal |
| ***14. EV Adjustment in high intensity light scenes***                                     | :no_entry_sign:        | :no_entry_sign:    | :no_entry_sign:       | :heavy_check_mark:     |
| ***15. DXR (ray-traced) shadows***                                                         | :no_entry_sign:        | :no_entry_sign:    | :no_entry_sign:       | :heavy_check_mark:     |
| ***16. [Box Light](HDRPBoxLight.md)***                                                     | :no_entry_sign:        | :no_entry_sign:    | :no_entry_sign:       | :heavy_check_mark:     |
| ***17. Rendering Paths***                                                                  |                        |                    |                       |                        |
| &ensp; Forward                                                                             | :heavy_check_mark:     | :heavy_check_mark: | :no_entry_sign:       | :heavy_check_mark:     |
| &ensp; Forward+                                                                            | :no_entry_sign:        | :heavy_check_mark: | :no_entry_sign:       | :no_entry_sign:        |
| &ensp; Deferred                                                                            | :x:                    | :x:                | :no_entry_sign:       | :heavy_check_mark:     |
| &ensp; Deferred+                                                                           | :no_entry_sign:        | :x:                | :no_entry_sign:       | :no_entry_sign:        |
| &ensp; 2D                                                                                  | :no_entry_sign:        | :no_entry_sign:    | :heavy_check_mark:    | :no_entry_sign:        |

Notes:
* :heavy_check_mark: : Supported
* :no_entry_sign: : Not supported (e.g. limitations in the render pipeline, etc)
* :x: : Currently not available
