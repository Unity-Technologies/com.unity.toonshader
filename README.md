# Latest official docs
- [English](https://docs.unity3d.com/Packages/com.unity.toonshader@latest)
- [日本語](https://docs.unity3d.com/ja/Packages/com.unity.toonshader@latest)
# Unity Toon Shader overview

![An image from the toon-shaded film The Phantom Knowledge in the Scene view of the Unity Editor. A close-up of a girl raising a weapon with a ruined cityscape in the background.](com.unity.toonshader/Documentation~/images/TPK_04.png)

The **Unity Toon Shader** (UTS3) is a set of toon shaders designed to meet the needs of creators working on cel-shaded 3D-CG animations.

The **Unity Toon Shader** is compatible with all render pipelines: the **Built-in Render Pipeline**, the **Universal Render Pipeline (URP)**, and the **High Definition Render Pipeline (HDRP)**. However, there are some differences in the features supported by different render pipelines. Please refer to [Feature Difference](com.unity.toonshader/Documentation~/FeatureModel_en.md) for details about each render pipeline.

**Unity Toon Shader** is the successor to **Unity-chan Toon Shader ver 2.0 (UTS2)**, which was popular for years in games and animations. However, UTS2 was designed solely for the **Built-in Render Pipeline**, consisted of many different shaders, could not be installed through the [Package Manager window](https://docs.unity3d.com/Manual/upm-ui.html), and had a different license from other Unity software.

## Features of Unity Toon Shader

In animation production, color specialists specify detailed color designs for every scene. UTS can apply these color specifications as written in the design instructions. UTS allows users to apply specific colors for light and shadow areas regardless of the actual light colors in the scene. This feature is essential for cel-shaded character designs. UTS provides detailed control over whether the directional light color affects materials.

Typical color design instruction example:

![A stylized girl in a sailor uniform. A: White highlights on the hair. B: The main blonde hair color. C: Hair in shadow, with a tan color. D: Hair in darker shadow, with a darker tan color.](com.unity.toonshader/Documentation~/images/UTS4Color2.png)

- A: Highlight Color
- B: Base Color
- C: 1st Shading Color
- D: 2nd Shading Color

Three basic colors—**base color**, **1st shading color**, and **2nd shading color**—play key roles in character design with the **Unity Toon Shader**. Please refer to [Three Color Map and Control Map Settings](com.unity.toonshader/Documentation~/Basic.md) for more information.

Besides the three basic colors, the **Unity Toon Shader** provides a wide variety of features such as [Highlight](Highlight.md), [Rim Light](Rimlight.md), [Emission](Emission.md), [Angel Ring](AngelRing.md), [Material Capture (MatCap)](com.unity.toonshader/Documentation~/MatCap.md), and some special maps.

To get started with cel-shading, see [Getting Started](com.unity.toonshader/Documentation~/GettingStarted.md).

# Table of Contents

* [Unity Toon Shader](com.unity.toonshader/Documentation~/index.md)
* [Requirements](com.unity.toonshader/Documentation~/System-Requirements.md)
* [Installation](com.unity.toonshader/Documentation~/installation.md)
* [Getting started](com.unity.toonshader/Documentation~/GettingStarted.md)
* [Material Converter](com.unity.toonshader/Documentation~/MaterialConverter.md)
* [Inspector Window Settings](com.unity.toonshader/Documentation~/Parameter-Settings.md)
  * [Modes](com.unity.toonshader/Documentation~/Modes.md)
  * [Shader Settings](com.unity.toonshader/Documentation~/Shader.md)
  * [Three Color Map and Control Map Settings](com.unity.toonshader/Documentation~/Basic.md)
  * [Shading Step and Feather Settings](com.unity.toonshader/Documentation~/ShadingStepAndFeather.md)
  * [Normal Map Settings](com.unity.toonshader/Documentation~/NormalMap.md)
  * [Highlight Settings](com.unity.toonshader/Documentation~/Highlight.md)
  * [Rim Light Settings](com.unity.toonshader/Documentation~/Rimlight.md)
  * [Material Capture (MatCap) Settings](com.unity.toonshader/Documentation~/MatCap.md)
  * [Emission Settings](com.unity.toonshader/Documentation~/Emission.md)
  * [Angel Ring Projection Settings](com.unity.toonshader/Documentation~/AngelRing.md)
  * [Scene Light Effectiveness Settings](com.unity.toonshader/Documentation~/SceneLight.md)
  * [Metaverse Settings (Experimental)](com.unity.toonshader/Documentation~/Metaverse.md)
  * [Outline Settings](com.unity.toonshader/Documentation~/Outline.md)
  * [Tessellation Settings (the Built-in Render Pipeline)](com.unity.toonshader/Documentation~/TessellationLegacy.md)
  * [Tessellation Settings (HDRP)](com.unity.toonshader/Documentation~/TessellationHDRP.md)
* Additional features for HDRP
  * [Box Light](com.unity.toonshader/Documentation~/HDRPBoxLight.md)
  * [Toon EV Adjustment](com.unity.toonshader/Documentation~/ToonEVAdjustment.md)
* [Samples](com.unity.toonshader/Documentation~/sample-instlation.md)
* [Feature Difference](com.unity.toonshader/Documentation~/FeatureModel_en.md)
* [Known Issues](com.unity.toonshader/Documentation~/Known-issue.md)


# Post-Cloning Setup on Windows

This repository utilizes symbolic links to share code across multiple projects.
On Windows 10 or later, symbolic link creation is restricted by default for regular users,
which can lead to issues when working with this repository.

To resolve this, follow the steps below to enable symbolic link creation and configure Git appropriately.

1. Open **Local Security Policy** by typing `secpol.msc` on Windows Command Prompt

![](https://raw.githubusercontent.com/Unity-Technologies/com.unity.toonshader/master/Images/CreateSymbolicLinks_LocalSecurityPolicy.jpg)


2. Under **User Rights Assignment**, find a policy called **Create symbolic links** and open it.
  - Click **Add User or Group**
  - Click **Object Types**
  - Make sure **Groups** is checked and click **OK**.

![](https://raw.githubusercontent.com/Unity-Technologies/com.unity.toonshader/master/Images/CreateSymbolicLinks_Properties.jpg)

3. Type **USERS** inside the textbox and click on **Check Names** to verify it, then click **OK**.

![](https://raw.githubusercontent.com/Unity-Technologies/com.unity.toonshader/master/Images/CreateSymbolicLinks_SelectUsers.jpg)

4. Configure git to allow symbolic links. For example, by typing the following in Git Bash:

```
git config --local core.symlinks true
git config --global core.symlinks true
```



*Auto-generated on Wed Oct 15 14:06:35 UTC 2025*
