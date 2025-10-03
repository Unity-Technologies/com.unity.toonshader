# What's new for Unity Toon Shader 0.8.x-preview

## Three render pipeline integrated shader

* From 0.8.0-preview onward, the **Unity Toon Shader** handles all render pipelines such as Built-in, URP, and HDRP with two shaders: **Toon** and **Toon (Tessellation)**. The Unity Toon Shader no longer includes shaders designed solely for a single render pipeline.
* 0.8.x-preview is compatible with Unity 2020.3, 2021.3, 2022.1, and 2022.2. This version no longer supports Unity 2019.4 as it utilizes the Shader Package Requirement feature.
* All materials created with versions older than 0.7.x-preview must be converted. 
* The converter is capable of converting materials from **Unity-chan Toon Shader 2.0.7** and **Universal Toon Shader**.

Please refer to [Unity Toon Shader Material Converter](MaterialConverter.md) for details.

