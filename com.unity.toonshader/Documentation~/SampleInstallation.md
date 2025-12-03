# Installing the sample scenes

The **Unity Toon Shader (UTS)** package provides 
sample scenes for every supported render pipeline. 
Import the sample set that matches the render pipeline currently assigned to your project.

## Before you import

- Confirm that the [render pipeline](https://docs.unity3d.com/6000.0/Documentation/Manual/render-pipelines.html) you intend to use is installed and active.

## Import samples through Package Manager

1. Open the [Package Manager](https://docs.unity3d.com/6000.0/Documentation/Manual/Packages.html).
2. Select **Unity Toon Shader** from the package list.
3. In the **Samples** section, choose the collection that matches your render pipeline.
4. Click **Import**. Unity creates an `Assets/Samples/Unity Toon Shader/<version>/...` folder containing the selected samples.

> Tip: You can re-import the samples at any time. Unity prompts you before overwriting existing files.

## Sample scene overview

### Universal Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/URP`

- `AngelRing/AngelRing.unity` &mdash; Setup for the [Angel Ring](AngelRing.md) feature.
- `Cube_HardEdge/Cube_HardEdge.unity` &mdash; Reference for baked normals.
- `BoxProjection/BoxProjection.unity` &mdash; Dark room lighting with box projection probes.
- `EmissiveAnimation/EmissiveAnimation.unity` &mdash; Animated [Emission](Emission.md) sequences.
- `LightAndShadows/LightAndShadows.unity` &mdash; Comparison between the PBR shader and UTS.
- `MatCapMask/MatCapMask.unity` &mdash; Using [MatCap](MatCap.md) masks.
- `Mirror/MirrorTest.unity` &mdash; Mirror material setup and testing.
- `NormalMap/NormalMap.unity` &mdash; Normal map techniques tuned with UTS.
- `PointLightTest/PointLightTest.unity` &mdash; Cel-shading examples using point lights.
- `KageBall/KageBall.unity` &mdash; The basics.
- `UnityChan/UnityChan.unity` &mdash; SD Unity-chan model showcasing illustration-style shading.
- `UnityChan_CelLook/UnityChan_CelLook.unity` &mdash; SD Unity-chan model with classic cel-look settings for characters.
- `UnityChan_Emissive/UnityChan_Emissive.unity` &mdash; SD Unity-chan model showing the [Emission](Emission.md) feature.
- `UnityChan_Firefly/UnityChan_Firefly.unity` &mdash; SD Unity-chan model with multiple point lights.

> Unity-chan assets are licensed under the [Unity-Chan License](http://unity-chan.com/contents/guideline_en/)

### Built-in Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/Built-In RP`

- Mirrors the URP scenes but configured for built-in render pipeline.

### High Definition Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/HDRP`

- Mirrors the URP scenes but configured for HDRP.

