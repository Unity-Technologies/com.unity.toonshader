# Unity Toon Shader - Context for AI Agents

## Project Overview

This is the **Unity Toon Shader (UTS)** package, a cel-shading/toon shading solution for Unity that supports multiple render pipelines (Built-in, URP, and HDRP).

## Documentation Structure

All documentation is located in the `com.unity.toonshader/Documentation~` folder. Key documentation files include:

- **[Basic.md](com.unity.toonshader/Documentation~/Basic.md)**: Core concepts including Three Color Maps and Shadow Control Maps
- **[ShadingStepAndFeather.md](com.unity.toonshader/Documentation~/ShadingStepAndFeather.md)**: Detailed shading controls and system shadow integration
- **[Troubleshooting.md](com.unity.toonshader/Documentation~/Troubleshooting.md)**: Common issues, workarounds, and tips
- **[Known-issue.md](com.unity.toonshader/Documentation~/Known-issue.md)**: Known issues and limitations
- **[TableOfContents.md](com.unity.toonshader/Documentation~/TableOfContents.md)**: Full documentation index

## Common Shadow-Related Issues

### Shadow Acne Problem
When shadows are enabled from lighting, shadow acne may appear where shadowed areas appear bright instead of properly darkened according to 1st/2nd shading map settings.

**Solution (URP)**: Use rendering layers with custom shadow layers to selectively control which objects receive shadows. The setup involves:
- Enabling Rendering Layers with Custom Shadow Layers in the URP asset
- Setting up objects with different rendering layer masks (e.g., "Default" vs "Default + Light Layer 1")
- Configuring lights to render to multiple layers but only cast shadows on specific layers
- See [Troubleshooting.md](com.unity.toonshader/Documentation~/Troubleshooting.md) for detailed step-by-step instructions

## Key Concepts

- **Three Color System**: Base Color, 1st Shading (shadow), 2nd Shading (deeper shadow)
- **Shadow Control Maps**: Artist-driven control over where shading appears, independent of lighting
- **System Shadows**: Unity's built-in shadow system that can be blended with stylized shading
- **Shading Step and Feather**: Controls for adjusting the transition between light and shadow areas

## Render Pipeline Support

The shader supports three render pipelines with slight differences in features:
- Built-in Render Pipeline
- Universal Render Pipeline (URP)
- High Definition Render Pipeline (HDRP)

HDRP has additional features like Box Light support and Toon EV Adjustment.
