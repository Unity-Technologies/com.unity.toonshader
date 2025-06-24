# Highlight Settings

The ability to display specular highlights in a cel-animation-like manner is essential for Toon Shaders. The **Unity Toon Shader** provides a wide variety of expressions with controllability to illuminate the area independently of light color and intensity for impressive cel-shading.

* [Highlight](#highlight)
* [Highlight Power](#highlight-power)
* [Specular Mode](#specular-mode)
  * [Color Blending Mode](#color-blending-mode)
* [Highlight Blending on Shadows](#highlight-blending-on-shadows)
  * [Blending Level](#blending-level)
* [Highlight Mask](#highlight-mask)
* [Highlight Mask Level](#highlight-mask-level)


## Highlight
Highlight : Texture(sRGB) × Color(RGB) Default:White. Pattern and color of specularly illuminated area.

<canvas class="image-comparison" role="img" aria-label="A toon-shaded sphere in a room textured with graphs. The sphere has green and purple bands of color, and a bright white specular highlight. Then the same sphere, with the color picker window open and the color #FF0000 selected. The specular highlight on the sphere is now orange and yellow.">
    <img src="images/Highlight0.png" title="The default color.">
    <img src="images/Highlight1.png" title="A different light color applied.">
</canvas>
<br />Drag the slider to compare the images.

## Highlight Power

The size of the Highlight controlled through the High light power slider. The size increase with the formula: pow(x,5).


## Specular Mode

UTS provides two modes for the highlight for different occasions and effect. The hard mode provides a crisp and solid edge to the highlight while the soft mode provides a blended blurred effect.

<canvas class="image-comparison" role="img" aria-label="A close-up of a white specular highlight on a green sphere. The highlight is a clear white disc with a hard edge. Then the same close-up. The specular highlight is now a blurred white disc.">
    <img src="Images/SpecularHard.png" title="Specular Mode set to Hard.">
    <img src="Images/SpecularSoft.png" title="Specular Mode set to Soft.">
</canvas>
<br />Drag the slider to compare the images.

<br><br>

### Color Blending Mode
Specular color blending mode allows the user to control the hardness of the colour applied to the highlight. Users have two options: Multiply or Add. Note that **Color Blending Mode** is disabled when **Specular** Mode is  **Soft**.

![A close-up of a yellow specular highlight on a green sphere. The highlight is a clear yellow disc with a hard edge.](images/SpecularMultiply.png)<br/>
Color Blending Mode set to Multiply.

![The same close-up. The specular highlight is now yellow, and changes brightness with the color of the sphere.](images/SpecularAdd.png)<br/>
Color Blending Mode set to Add.


## Highlight Blending on Shadows
Control the blending for the highlights in shadows. Please refer to the image at [Blending Level](#blending-level).

### Blending Level
Adjusts the intensity of highlight applied to shadow areas.

## Highlight Mask
A gray scale texture which utilises its brightness to control highlight intensity. Applying the highlight mask allows to fine-tune the reflectivity on the material.

![A square texture with a black and grey checkerboard pattern. Each square has a small plus symbol in its center.](images/UVCheckGrid.png)<br/>
An example of a grayscale texture highlight mask.

<canvas class="image-comparison" role="img" aria-label="A toon-shaded sphere in a room textured with graphs. The sphere has green and purple bands of color, and circular green, white, and purple specular highlights. Then the same sphere. The specular highlights now reflect the checkerboard pattern.">
    <img src="images/HighlightMaskOff.jpg" title="Highlight Mask disabled">
    <img src="images/HighlightMaskOff.jpg" title="Highlight Mask enabled">
</canvas>
<br />Drag the slider to compare the images.

## Highlight Mask Level
Highlight mask texture blending level to highlights.
