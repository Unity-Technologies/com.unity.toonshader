# Getting started with Unity Toon Shader

The **Unity Toon Shader** (UTS) provides many properties for professional cel-shading. On this page, you'll learn the key concepts for basic cel-shading step by step.

The steps for simple cel-shading are:
* [Make sure at least one directional light is in the scene](#put-a-directional-light-in-the-scene).
* [Create materials for cel-shading and set the appropriate shader](#creating-a-new-material-and-applying-unity-toon-shader).
* [Set up three basic colors](#setting-up-three-basic-colors).
* [Determine the character's visual style](#adjusting-the-edges-of-the-three-basic-color-regions).
* [Set up the outline](#set-up-the-outline).

After mastering the basics, you might want to explore:
* [Advanced techniques](#advanced-techniques).
  * [Eliminate outlines around eyes](#eliminating-outlines-around-eyes).
  * [Add luster to hair](#adding-luster-to-hair).
* [Options for stunning professional cel-shading](#more-options-for-stunning-professional-cel-shading).


## Put a directional light in the scene
To make cel-shading work, you need to place at least one [directional light](https://docs.unity3d.com/2022.2/Documentation/Manual/Lighting.html) in the scene.

## Creating a new material and applying Unity Toon Shader

Start by [creating a material](https://docs.unity3d.com/6000.0/Documentation/Manual/materials-introduction.html).

![A fully white chibi-style character model with long hair and rabbit ears. The Inspector window is open with the Lit material of the Universal Render Pipeline.](images/UrpLitMaterial.png)<br/>
Next, select the appropriate shader for the material.

Because the **Unity Toon Shader** (UTS) is compatible with all render pipelines—the Built-in Render Pipeline, URP, and HDRP—the shaders you need to choose are simply **Toon** or **Toon (Tessellation)**. They are not listed under Universal Render Pipeline or HDRP in the menus.

![The same character model with the Toon shader selected in the Inspector window. The model is now a flat white silhouette.](images/AppliedUTS.png)

You'll notice that the directional light doesn't affect the shading like usual. This is because UTS controls the lighting response according to the artist's intentions. UTS provides detailed control over whether the directional light color affects materials. Please refer to [Scene Light Effectiveness Settings](SceneLight.md) for more information. However, while learning this section, it's recommended to set the light color to **white**.

## Setting up three basic colors

The most basic function of UTS is to render the mesh in three regions: **Base Map** for regions with no shadows, **1st Shading Map** for lighter shaded regions, and **2nd Shading Map** for darker shaded regions. [Three Color Map and Control Map Settings](Basic.md) provides the properties to control these fundamental settings. For basic cel-shading, two maps—**Base Map** and **1st Shading Map**—work fine.

![The same character model with the same shader selected. The Base Map property and the 1st Shading Map properties in the Inspector window are set to UV texture maps that have the shapes and colors for the model. The character is now fully textured.](images/AppliedTextures.png)

Apply the Base Map and 1st Shading Map to the material. The difference between the two textures is the color tone. In this example, two different textures are applied. However, it's also possible to apply one texture and use different colors.

![A UV map texture that contains all the parts of a chibi-style model](images/utc_all2_light.png)<br/>
An example Base Map.

![The same UV map but some areas have a darker color.](images/utc_all2_dark.png)</br>
An example 1st Shading Map.

## Adjusting the edges of the three basic color regions

The visual style of the image is one of the most important factors that determine the overall look of the work. [Shading Steps and Feather Settings](ShadingStepAndFeather.md) provides ways to adjust the position of the border between regions and whether they're clearly separated or blended. First, you should adjust **Base Color Step** to make the **1st Shading Map** (the darker texture shown above) visible.

![The same character model. In the Inspector window of the Toon shader, the Base Color Step property is set to 0.609. The shadows on the model are more prominent.](images/WithoutOutline.png)

In the image above, the boundary between **Base Map** and **1st Shading Map** is clearly separated. Try adjusting **Base Shading Feather** to see how to control the boundary sharpness. Sometimes, blended borders are more desirable.

![The same character model. In the Inspector window of the Toon shader, the Base Shading Feather property is set to 0.279. The shadows on the model are more blended.](images/AdjustingFeather.png)

## Set up the outline
The outline is another important factor that determines the animation style. The color of the outline should either be close to the background or clearly distinguishable, and its thickness affects the overall style of the animation. [Outline Settings](Outline.md) provides the properties to control these aspects.

<canvas class="image-comparison" role="img" aria-label="The same character model. In the Inspector window of the Toon shader, the Outline Color property is set to gray, and the Outline Width property is set to 4, then 6.44.">
    <img src="images/ThinOutline2.png" title="Outline Width: 4">
    <img src="images/BoldOutline2.png" title="Outline Width: 6.44">
</canvas>
<br />Drag the slider to compare the images.

## Advanced techniques
Now that you've learned basic cel-shading, professional cel-shading often requires additional techniques. You'll learn a couple of advanced techniques in this section.

### Eliminating outlines around eyes
Look at the character's face in the images above again. You'll notice that outlines around the character's eyes can detract from the overall appearance. UTS provides an [Outline Width Map](Outline.md#outline-width-map) to solve this issue. By applying this map, you can control the thickness of outlines around every part of the mesh to your satisfaction.

![The same character model. In the Inspector window of the Toon shader, the Outline Width Map property is set to a texture. There's no longer an outline around the eyes of the character.](images/OutlineWidthMap3.png)


### Adding luster to hair
You might notice that the image still appears flat and lacks three-dimensionality.
[Highlight](Highlight.md) on hair is a common technique in anime production.
[Angel Ring](AngelRing.md) and [Material Capture (MatCap)](MatCap.md) are more specialized techniques for hair luster. In this example, we'll apply MatCap to the hair. Create another material, then set the [MatCap map](MatCap.md#matcap-map) in it.

![The same character model. In the Inspector window of the Toon shader, the MatCap Map property is set to a texture. The shadows and outlines on the hair look more three-dimensional.](images/Luster3.png)



## More options for stunning professional cel-shading
The following features are also essential in modern animation and game production when using cel-shading. Please try these out after mastering the techniques on this page.

* [Emission](Emission.md)
* [Normal Map](NormalMap.md)
* [Rim Light](Rimlight.md)

