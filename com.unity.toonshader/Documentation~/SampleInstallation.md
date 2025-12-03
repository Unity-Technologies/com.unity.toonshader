# Installing the sample scenes

The **Unity Toon Shader** package includes sample scenes that demonstrate lighting setups, material presets, and authoring workflows for every supported render pipeline. Import the sample set that matches the render pipeline currently assigned to your project.

## Before you import

- Confirm that the [render pipeline](https://docs.unity3d.com/2022.2/Documentation/Manual/render-pipelines.html) you intend to use is installed and active.
- In **Project Settings ▸ Graphics**, make sure the correct pipeline asset is assigned. URP samples expect the `UTS2URPPipelineAsset`, while HDRP samples expect `HDRenderPipelineAsset_UTS`.
- Close scenes you do not want modified—the importer adds lighting assets, materials, and scene files.

## Import samples through Package Manager

1. Open the [Package Manager](https://docs.unity3d.com/2022.2/Documentation/Manual/Packages.html).
2. Select **Unity Toon Shader** from the package list.
3. In the **Samples** section, choose the collection that matches your render pipeline.
4. Click **Import**. Unity creates an `Assets/Samples/Unity Toon Shader/<version>/...` folder containing the selected samples.
5. (Recommended) Re-open **Project Settings ▸ Graphics** to ensure the correct pipeline asset remains assigned.

> Tip: You can re-import the samples at any time. Unity prompts you before overwriting existing files.

## Sample scene overview

### Universal Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/Universal Render Pipeline`

| Scene | Description |
| --- | --- |
| `Sample/Sample.unity` | Guided tour of the core toon material settings. |
| `ToonShader.unity` | Illustration-style shading showcase. |
| `ToonShader_CelLook.unity` | Classic cel-look configuration for characters. |
| `ToonShader_Emissive.unity` | Demonstrates [Emission](Emission.md) layering. |
| `ToonShader_Firefly.unity` | Multiple point lights without firefly artifacts. |
| `AngelRing/AngelRing.unity` | Setup for the [Angel Ring](AngelRing.md) projection. |
| `Baked Normal/Cube_HardEdge.unity` | Reference for baked-normal workflows. |
| `BoxProjection/BoxProjection.unity` | Dark-room lighting with box projection probes. |
| `EmissiveAnimation/EmissiveAnimation.unity` | Animated [Emission](Emission.md) sequences. |
| `LightAndShadows/LightAndShadows.unity` | Comparison between the built-in PBR shader and UTS. |
| `MatCapMask/MatCapMask.unity` | Using [MatCap](MatCap.md) masks for accent lighting. |
| `Mirror/MirrorTest.unity` | Mirror material setup and testing. |
| `NormalMap/NormalMap.unity` | Normal-map techniques tuned for toon shading. |
| `PointLightTest/PointLightTest.unity` | Point- and spot-light cel shading examples. |

### Built-in Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/Legacy Render Pipeline`

- Mirrors the URP scenes but configured for the built-in forward renderer.
- Includes lighting presets and materials that match the legacy pipeline defaults.

### High Definition Render Pipeline

`Assets/Samples/Unity Toon Shader/<version>/High Definition Render Pipeline`

- Demonstrates HDRP-specific features such as additional light types, high-quality shadows, and volumetric effects.
- Only open these scenes when the project uses `HDRenderPipelineAsset_UTS` to avoid missing-shader warnings.
