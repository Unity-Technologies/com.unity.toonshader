# Requirements and compatibility

* **Requires Unity 2019.4.21f1 or higher**. 

## Render pipeline compatibility

*  Unity Toon Sahder supports **Legacy**, **Universal RP** and **HDRP**. Please refer to the documentation of each rendering pipeline for supported platforms. However, HDRP uses stencil buffers internally, so UTS stencil effects cannot be used. Please check the [Feature Model documentation](./en/FeatureModel_en.md)  for the different support status of UTS in each render pipeline.

* The behavior of the Unity Toon Shader varies slightly depending on the render pipeline. Please see the Feateure Model for details.
 
* Unity Toon Shader uses **a forward rendering**. Using **a linear color space** is recommended. (A gamma color space can also be used, but this tends to strengthen shadow gradiation. For more details, see [Linear or Gamma Workflow](https://docs.unity3d.com/Manual/LinearRendering-LinearOrGammaWorkflow.html).)

* Due to the pandemic, we are currently unable to test on the consoles, so please bear this in mind.
