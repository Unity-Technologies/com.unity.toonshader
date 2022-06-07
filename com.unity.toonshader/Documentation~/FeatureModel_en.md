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
| ***4. HighColor features*** 	|  	|  	|  	|  	|
| Set High Color and map and Tweak HighColor Power | OK 	| OK 	| OK 	|  	|
| Switch Specular Mode 	| OK 	| OK 	| OK 	|  	|
| Switch Color Blend Mode either Multiply or Additive 	| OK 	| OK 	| OK 	|  	|
| Mask High Color zone with system shadows and Tweak the masking power 	| OK 	| OK 	| OK 	|  	|
| Set HighColor Mask and Tweak the level of the mask 	| OK 	| OK 	| OK 	|  	|
| ***5. RimLight features*** 	|  	|  	|  	|  	|
| Add RimLight on the surface and Set the color and power of it 	| OK 	| OK 	| OK 	|  	|
| Tweak the power of RimLight Inside Mask and Cut the feather off on the edge of the mask 	| OK 	| OK 	| OK 	|  	|
| Light Direction Mask function. Tweak the level of the mask 	| OK 	| OK 	| OK 	|  	|
| Antipodean(Ap)_RimLight function. <br>Ap_RimLight appears on the opposite surface of the light direction. <br>Set the color of the Ap_RimLight, tweak the power of it and switch whether cutting the feather off 	| OK 	| OK 	| OK 	|  	|
| RimLight Mask function. Tweak the level of the mask 	| OK 	| OK 	| OK 	|  	|
| ***6. MatCap features*** 	|  	|  	|  	|  	|
| Add MatCap on the surface and Set the MatCap sampler and color 	| OK 	| OK 	| OK 	|  	|
| Blur the MatCap sampler and Tweak Tiling and Offset 	| OK 	| OK 	| OK 	|  	|
| Switch Color Blend Mode either Multiply or Additive 	| OK 	| OK 	| OK 	|  	|
| Tweak the Scale and Rotate of the UV Coordinate of MatCap Sampler 	| OK 	| OK 	| OK 	|  	|
| CameraRolling Stabilizer function, freezing the rotation of MatCap <br>projection following by camera rolls 	| OK 	| OK 	| OK 	|  	|
| Normal Map for MatCap function.  Tweak Bump scale, Tiling, Offset and UV Rotation 	| OK 	| OK 	| OK 	|  	|
| Mask MatCap projection with system shadows and Tweak the masking power 	| OK 	| OK 	| OK 	|  	|
| Switch the projection camera of MatCap, avoiding lens distortion 	| OK 	| OK 	| OK 	|  	|
| Set MatCap Mask, Tweak the level of the mask, or Inverse it. 	| OK 	| OK 	| OK 	|  	|
| ***7. AngelRing Projection features*** 	|  	|  	|  	|  	|
| Add AngelRing Projection on the surface and <br>Set the AngelRing Sampler and color 	| OK 	| OK 	| OK 	|  	|
| Offset the position of AngelRing with  the U or V axis 	| OK 	| OK 	| OK 	|  	|
| Use the alpha channel of AngelRing sampler as a Clipping mask 	| OK 	| OK 	| OK 	|  	|
| ***8. Emissive features*** 	|  	|  	|  	|  	|
| Set Emissive Map and Color and Tweak Tiling and Offset 	| OK 	| OK 	| OK 	|  	|
| Use the alpha channel of Emissive Map as a Clipping mask 	| OK 	| OK 	| OK 	|  	|
| Emissive Animation function 	| OK 	| OK 	| OK 	|  	|
| Set Base Speed of updating 	| OK 	| OK 	| OK 	|  	|
| Switch scroll coordinate either UV or View 	| OK 	| OK 	| OK 	|  	|
| Set scroll amounts of Emissive Map between U/X and V/Y directions 	| OK 	| OK 	| OK 	|  	|
| Set rotate amounts of Emissive Map around the center of UV Coordinate 	| OK 	| OK 	| OK 	|  	|
| Set PingPong like movement for scroll 	| OK 	| OK 	| OK 	|  	|
| ColorShift function. Set the speed of shift and the destination color 	| OK 	| OK 	| OK 	|  	|
| ViewShift function. Set the destination color to view shift. 	| OK 	| OK 	| OK 	|  	|
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



