# Installing the sample scenes

The **Unity Toon Shader** package provides 
sample scenes for every supported render pipeline. 
Import the sample set that matches the render pipeline currently assigned to your project.

## Before you import

- Confirm that the [render pipeline](https://docs.unity3d.com/2022.2/Documentation/Manual/render-pipelines.html) you intend to use is installed and active.

## Import samples through Package Manager

1. Open the [Package Manager](https://docs.unity3d.com/2022.2/Documentation/Manual/Packages.html).
2. Select **Unity Toon Shader** from the package list.
3. In the **Samples** section, choose the collection that matches your render pipeline.
4. Click **Import**. Unity creates an `Assets/Samples/Unity Toon Shader/<version>/...` folder containing the selected samples.

> Tip: You can re-import the samples at any time. Unity prompts you before overwriting existing files.

## Sample scene overview

### Universal Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/Universal Render Pipeline`

- `AngelRing/AngelRing.unity` &mdash; Setup for the [Angel Ring](AngelRing.md) projection.
- `Baked Normal/Cube_HardEdge.unity` &mdash; Reference for baked-normal workflows.
- `BoxProjection/BoxProjection.unity` &mdash; Dark-room lighting with box projection probes.
- `EmissiveAnimation/EmissiveAnimation.unity` &mdash; Animated [Emission](Emission.md) sequences.
- `LightAndShadows/LightAndShadows.unity` &mdash; Comparison between the built-in PBR shader and UTS.
- `MatCapMask/MatCapMask.unity` &mdash; Using [MatCap](MatCap.md) masks for accent lighting.
- `Mirror/MirrorTest.unity` &mdash; Mirror material setup and testing.
- `NormalMap/NormalMap.unity` &mdash; Normal-map techniques tuned for toon shading.
- `PointLightTest/PointLightTest.unity` &mdash; Point- and spot-light cel shading examples.
- `Sample/KageBall.unity` &mdash; Guided tour of the core toon material settings.
- `UnityChan/UnityChan.unity` &mdash; Illustration-style shading showcase.
- `UnityChan_CelLook/UnityChan_CelLook.unity` &mdash; Classic cel-look configuration for characters.
- `UnityChan_Emissive/UnityChan_Emissive.unity` &mdash; Demonstrates [Emission](Emission.md) layering.
- `UnityChan_Firefly/UnityChan_Firefly.unity` &mdash; Multiple point lights without firefly artifacts.

### Built-in Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/Legacy Render Pipeline`

- Mirrors the URP scenes but configured for the built-in forward renderer.
- Includes lighting presets and materials that match the legacy pipeline defaults.

### High Definition Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/High Definition Render Pipeline`

- Demonstrates HDRP-specific features such as additional light types, high-quality shadows, and volumetric effects.
- Only open these scenes when the project uses `HDRenderPipelineAsset_UTS` to avoid missing-shader warnings.