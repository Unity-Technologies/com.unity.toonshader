| Feature Description 	|  Legacy 	| URP 	| HDRP 	| Note 	|
|-	|-	|-	|-	|-	|
| 	|  	|  	|  	| as of 0.7.3-preview	|
| ***1. Modes*** 	|  	|  	|  	|  	|
| Standard	| OK 	| OK 	| OK 	|  	|
| With Advanced Control Map 	| OK 	| OK	| OK 	|  	|
| ***2. Shader Settings*** 	|  	|  	|  	|  	|
| Culling  	| OK 	| OK 	| OK 	|  	|
| Stencil  	| OK 	| OK 	| N.A. 	|  	|
| Stencil Value 	| OK 	| OK 	| N.A. 	|  	|
| Clipping 	| OK 	| OK 	| OK	|  	|
| Clipping Mask | OK 	|OK	| OK 	|  	|
| Invert  Clipping Mask 	| OK 	| OK 	| OK 	|  	|
| Use Base Map Alpha as Clipping Mask 	| OK 	|OK	| OK	|  	|
| ***3. Three Color Map and Control Map Settings*** 	|  	|  	|  	|  	|
| Base Map　| OK 	| OK	| OK 	|  	|
| 1st Shading Map	| OK 	| OK 	| OK	|  	|
| 2nd Shading Map	| OK 	| OK 	| OK 	|  	|
| Normal Map	| OK 	| OK 	| OK 	|  	|
| Shadow Control Maps	| OK 	| OK 	| OK 	|  	|
| ***4. Shading Steps and Feather Settings***	|  	|  	|  	|  	|
| Base Color Step	| OK 	| OK 	| OK 	|  	|
| Base Shading Feather| OK 	| OK 	| OK 	|  	|
| Shading Color Step 	| OK 	| OK 	| OK 	|  	|
| Point Light Step Offset	| OK 	| OK 	| OK 	|  	|
| Filter Point Light Highlights 	| OK 	| OK 	| OK 	|  	|
| ***5. Highlight Settingss*** 	|  	|  	|  	|  	|
| Highlight Power | OK 	| OK 	| OK 	|  	|
| Specular Mode 	| OK 	| OK 	| OK 	|  	|
| Color Blending Mode 	| OK 	| OK 	| OK 	|  	|
| Highlight Blending on Shadows 	| OK 	| OK 	| OK 	|  	|
| Highlight Mask	| OK 	| OK 	| OK 	|  	|
| Highlight Mask Level	| OK 	| OK 	| OK 	|  	|
| ***6. Rim Light Settings*** 	|  	|  	|  	|  	|
| Rim Light Color	| OK 	| OK 	| OK 	|  	|
| Rim Light Level	| OK 	| OK 	| OK 	|  	|
| Adjust Rim Light Area 	| OK 	| OK 	| OK 	|  	|
| Inversed Light Direction Rim Light 	| OK 	| OK 	| OK 	|  	|
| Rim Light Mask	| OK 	| OK 	| OK 	|  	|
| ***7. Material Capture(MatCap) Settings*** 	|  	|  	|  	|  	|
| MatCap Map 	| OK 	| OK 	| OK 	|  	|
| MatCap Blur Level	| OK 	| OK 	| OK 	|  	|
| Color Blending Modee 	| OK 	| OK 	| OK 	|  	|
| Scale MatCap UV	| OK 	| OK 	| OK 	|  	|
| Rotate MatCap UV	| OK 	| OK 	| OK 	|  	|
| Stabilize Camera Rolling	| OK 	| OK 	| OK 	|  	|
| Normal Map	| OK 	| OK 	| OK 	|  	|
| Rotate Normal Map UV 	| OK 	| OK 	| OK 	|  	|
| MatCap Blending on Shadows 	| OK 	| OK 	| OK 	|  	|
| MatCap Camera Mode	| OK 	| OK 	| OK 	|  	|
| MatCap Mask	| OK 	| OK 	| OK 	|  	|
| MatCap Mask Level 	| OK 	| OK 	| OK 	|  	|
| Invert MatCap Mask	| OK 	| OK 	| OK 	|  	|
| ***8. Emission Settings*** 	|  	|  	|  	|  	|
| Emission Map 	| OK 	| OK 	| OK 	|  	|
| Use the alpha channel of Emissive Map as a Clipping mask 	| OK 	| OK 	| OK 	|  	|
| Emission Map Animation	| OK 	| OK 	| OK 	|  	|
| Base Speed (Time)	| OK 	| OK 	| OK 	|  	|
| Animation Mode	| OK 	| OK 	| OK 	|  	|
| Scroll U/X direction 	| OK 	| OK 	| OK 	|  	|
| Scroll V/Y direction	| OK 	| OK 	| OK 	|  	|
| Rotate around UV center 	| OK 	| OK 	| OK 	|  	|
| Ping-pong moves for base 	| OK 	| OK 	| OK 	|  	|
| Color Shifting with Time 	| OK 	| OK 	| OK 	|  	|
| Color Shifting with View Angle	| OK 	| OK 	| OK 	|  	|
| ***9. Angel Ring Projection Settings*** 	|  	|  	|  	|  	|
| Angel Ring	| OK 	| OK 	| OK 	|  	|
| Offset U/V	| OK 	| OK 	| OK 	|  	|
| Alpha Chennel as Clipping Mask	| OK 	| OK 	| OK 	|  	|
| ***9. Outline features*** 	|  	|  	|  	|  	|
| Switch the method of Outline Mode either Normal Direction or Position Scaling 	| OK 	| OK 	| OK 	|  	|
| Set the width and color of the outline 	| OK 	| OK 	| OK 	|  	|
| Blend the color of the outline with the surface's base color 	| OK 	| OK 	| OK 	|  	|
| Control the width of the outline's stroke with Outline Sampler 	| OK 	| OK 	| OK 	|  	|
| Offset the outline along with the Z-axis of the camera coordinate 	| OK 	| OK 	| OK 	|  	|
| Control the width of the outline with the distance from the camera. Set the threshold values of the farthest and nearest distances 	| OK 	| OK 	| OK 	|  	|
| Use Outline Texture for the outline's color 	| OK 	| OK 	| OK 	|  	|
| Use Baked NormalMap for Outline in the Normal Direction method 	| OK 	| OK 	| OK 	|  	|
| ***10. Tessellation feature*** 	|  	|  	|  	|  	|
| Tessellation function. Tweak the value of Edge Length, Phong Strength, or Extrusion Amount 	| DX11 	| N.A. 	| DX11/Vulkan/Metal 	|  	|
| ***11. LightColor Contribution feature*** 	|  	|  	|  	|  	|
| Turn on/off Realtime LightColor Contribution to each color:<br> i.e., BaseColor, 1st Shade Color, 2nd Shade Color, HighColor, RimLight, Ap_RimLight, MatCap, AngelRing, or Outline 	| OK 	| OK 	| OK 	|  	|
| ***12. Environmental Lighting Contributions features*** 	|  	|  	|  	|  	|
| Tweak the value of GI Intensity toward material from light probes 	| OK 	| OK 	| OK 	|  	|
| Tweak the value of Unlit Intensity of material in the scene where is no realtime lighting source 	| OK 	| OK 	| OK 	|  	|
| SceneLights Hi-Cut Filter function. Adjust the high intensity from lights avoiding the overshoot of material's colors 	| OK 	| OK 	| OK 	|  	|
| Built-in Light Direction function. <br>Activate the virtual light for each material and Tweak the offset value of each axis of the light's direction 	| OK 	| OK 	| OK 	|  	|
| **UTS Feature Model 2.2** 	|  	|  	|  	|  	|
| ***1. Integrate two workflows and shader variations as an Uber Shader*** 	|  	|  	|  	|  	|
| UniversalToon / Uber shader custom user interface 	| OK	| OK 	| OK 	|  	|
| Switch Workflow Mode either DoubleShadeWithFeather or ShadingGradeMap 	| OK	| OK 	| OK 	|  	|
| Select Auto Queue or Custom Render Queue 	| OK	| OK 	| OK 	|  	|
| Switch Transparent Mode 	| OK	| OK 	| OK 	|  	|
| Switch Stencil Mode either Off, StencilOut or StencilMask 	| OK	| OK 	| N.A. 	|  	|
| Switch Clipping Mode 	| OK	| OK 	| OK 	|  	|
| Switch TransClipping Mode 	| OK	| OK 	| OK 	|  	|
| Activate Outline function 	| OK	| OK 	| OK 	|  	|
| ***2. Rendering per Channels feature*** 	|  	|  	|  	|  	|
| Set the color and visibility of each channel:<br> i.e., BaseColor, 1st Shade, 2nd Shade, HighColor, AngelRing, RimLight, or Outline 	| N.A. 	| N.A. 	| OK 	|  	|
| **UTS Feature Model 3.0** 	|  	|  	|  	|  	|
| **UTS Feature Model 3.1** 	|  	|  	|  	|  	|
| ***2. EV Adjustment*** 	|  	|  	|  	|  	|
| EV Adjustment in high intensity light scenes	 	| N.A	| N.A.	| OK	|  	|
| ***3. Render pipeline built-in raytraced shadows*** 	|  	|  	|  	|  	|
| DXR shadow supported in render pipelines 	| N.A.	| N.A.	| OK	|  	|
| ***4. Box Light*** 	|  	|  	|  	|  	|
| Substitute for directional light 	| N.A.	| N.A.	| OK	| In order to get around the limitation that multiple directional lights cannot cast shadows at the same time. |
| **UTS Feature Model 3.2** 	|  	|  	|  	|  	|
| ***1. Instanced stereo rendering***	|  	|  	|  	|  	|
| instanced Stereo Rendering 	| 3.2	| 3.2	| 3.2 ( Tested with HDRP newer than 10.6 )	| Stereo instance rendering is available for DX11. Due to the pendemic, PS4 and some other consoles are not yet checked.  |



