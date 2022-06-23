# Getting Started

The **Unity Toon Shader**(UTS) provides tons of parameters for professional cel-shading,though. In this chapter, you'll learn what are keys for basic cel-shading step by step. 

What you have to do for simple cel-shading are:
* [Make sure at least one directional light is in the scene.](#put-a-directional-light-in-the-scene)
* [Create materials for cel-shading and set appropriate shader path.](#creating-a-material-and-applying-unity-toon-shader)
* [Set up three basic colors](#setting-up-three-basic-colors).
* [Determine touch of the character](#adjusting-edge-of-three-basic-color-area).
* [Set highlight type.](#setting-highlight-type)
* [Adjust Outline.](#adjusting-outline)


## Put a directional light in the scene
To make cel-shading work. You need to place at least one [directional light](https://docs.unity3d.com/2022.2/Documentation/Manual/Lighting.html) in the scene.

## Creating a material and applying Unity Toon Shader

Start from [creating a material](https://docs.unity3d.com/2022.2/Documentation/Manual/materials-introduction.html) and selecting appropriate shader for it. 

Because the **Unity Toon Shader**(UTS) includes shaders for all the render pipelines,The Built-in Render Pipeline, URP, and HDRP, you need to choose appropriate shader for the render pipeline set to your project.

| Render Pipeline | Shader Path | Tesselation Shader Path |
|----|----|----|
|The Built-in Render Pipeline | Toon (Built-in) | ToonTessellation (Built-in) |
|URP | URP/Toon | N.A. |
|HDRP | HDRP/Toon | HDRP/ToonTessellation


## Setting up three basic colors

The most basic function of the UTS is to render the mesh in three regions. **Base Map** for regions with no shadows, **1st shading map** for regions with shaded lighter , and **2nd shading map** for regions with shaded darker. [Three Color Map and Control Map Settings](Basic.md) provides the parameters to control this fundamental settings.



## Adjusting edge of three basic color area

Touch of the image is one of the most important factors that determine the style of the work. [Shading Steps and Feather Settings](ShadingStepAndFeather.md) provides  ways to adjust the position of the border between the regions and whether they're clearly separated or blended.

## Setting highlight type

Japanese-style animation production tends to favor highlights that are clearly distinguished from other areas. Whereas, depending on the animation style, natural specular light may be desirable. UTS supports both, providing a rich variety of expression with a wide range of parameters in [Highlight Settings](Highlight.md).

## Adjusting outline
The Outline is another important factor that determines the animation touch. The color of the border should be close to the background or clearly distinguishable, and its thickness affects the style of the animation. [Outline Settings](Outline.md) provides the parameters to control them.

## Options to make the shading more impressive

The following factors are also essential in recent  animation production. Please try them after mastering this chapter.

* [Emission](Emission.md)
* [Normal Map](NormalMap.md)
* [Rim Light](Rimlight.md)
* [Material Capture (MatCap)](MatCap.md)
