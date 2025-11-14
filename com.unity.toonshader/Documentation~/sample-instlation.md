# How to install samples

Unity provides **Unity Toon Shader** sample scenes for the three render pipelines. Please install the set of samples that fits the render pipeline you use.

1. Make sure the [render pipeline](https://docs.unity3d.com/2022.2/Documentation/Manual/render-pipelines.html) you want to use is installed and set up before installing the UTS samples.
1. Open the [Package Manager window](https://docs.unity3d.com/2022.2/Documentation/Manual/Packages.html).
1. Click **Unity Toon Shader** on the left side of the window.
1. Press **Import** on the right side of the window.
1. Set the Graphics Pipeline Asset in the [Project Settings window](https://docs.unity3d.com/2022.2/Documentation/Manual/comp-ManagerGroup.html). URP samples require `UTS2URPPipelineAsset`, whereas HDRP samples require `HDRenderPipelineAsset_UTS`.
<br/><br/>

 The `Assets/Samples/Unity Toon Shader/0.9.3-preview/Universal render pipeline` folder contains the following scenes:

* Sample/Sample.unity: A scene to introduce the basics.
* ToonShader.unity: An illustration-style shading sample scene.
* ToonShader_CelLook.unity: A cel-style shading sample scene.
* ToonShader_Emissive.unity: A sample scene for [Emission](Emission.md).
* ToonShader_Firefly.unity: A sample scene for multiple point lights.
* AngelRing/AngelRing.unity: A sample scene for [Angel Ring](AngelRing.md).
* Baked Normal/Cube_HardEdge.unity: Baked Normal reference.
* BoxProjection/BoxProjection.unity: A sample scene lighting a dark room using Box Projection.
* EmissiveAnimation/EmisssiveAnimation.unity: [Emission](Emission.md) animation sample.
* LightAndShadows/LightAndShadows.unity: Comparison between the PBR shader and the **Unity Toon Shader**.
* MatCapMask/MatCapMask.unity: A [MatCap](MatCap.md) mask sample scene.
* Mirror/MirrorTest.unity: A sample scene for testing mirror objects.
* NormalMap/NormalMap.unity: Techniques for using normal maps with the **Unity Toon Shader**.
* PointLightTest/PointLightTest.unity: A sample for cel-shading content with point lights.


Sample scenes for other render pipelines are in the following folders:
* For the **Built-in Render Pipeline**: `Assets/Samples/Unity Toon Shader/0.9.6-preview/Legacy render pipeline` folder.
* For the **High Definition Render Pipeline**: `Assets/Samples/Unity Toon Shader/0.9.3-preview/High definition render pipeline` folder.