<a id="BoxLight"></a>
## HDRP Box Light

When you want to use lighting that differs from the scene main light to make character face more clear or to add shadows that look ideal. But, in HDRP, one directional light cast shadows, not more than two. The [Box Light](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Light-Component.html#Shape) provides the solution for this. The **Unity Toon Shader** now supports the feature. For now, HDRP Ray-traced shadows isn't ready for this feature.

<canvas class="image-comparison" role="img" aria-label="The Scene view with a cube representing a box light, and the Game view with a chibi-style character model. When the box light is rotated, the shadows on the face of the model change.">
    <img src="images/BoxLight0.png" title="Box light">
    <img src="images/BoxLight1.png" title="Box light rotated">
</canvas>
<br />
Drag the slider to compare the images.

<small>Box light applied to a character's face. Note that editing the angle of the box changes the shadows falling on the face, but not on the body and the ground.</small>
