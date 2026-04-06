# Troubleshooting and Tips

This page contains common issues, workarounds, and tips for using **Unity Toon Shader** effectively.

## Shadow Acne and Bright Shadow Areas

### Issue

When [Receive shadows](ShadingStepAndFeather.md#receive-shadows) is enabled, you may observe **shadow acne** - areas within shadows that 
appear bright or not black according to the settings of the 1st Shading Map or 2nd Shading Map, 
rather than being properly darkened.

### Workaround for Universal Render Pipeline (URP)

To work around shadow acne issues in URP:

1. **Use Rendering Layers / Shadow Layers**: Configure rendering layers on your lights and objects to control which objects cast shadows on which surfaces. This gives you fine-grained control over shadow casting and can help eliminate unwanted shadow artifacts.

   **Step-by-step setup for Rendering Layers with Custom Shadow Layers:**

   a. **Enable Rendering Layers in URP Asset**:
      - Select your URP Asset
      - Enable **Rendering Layers** with **Custom Shadow Layers**
      - For more information, see [Unity's Rendering Layers documentation](https://docs.unity3d.com/Manual/urp/features/rendering-layers-lights.html)

   b. **Set up the scene**:
      - Add a **Plane** GameObject as the floor
      - Add a **Sphere** with a Toon material. Set its **Rendering Layer Mask** to **"Default"**
      - Add another **Sphere** with a Toon material. Set its **Rendering Layer Mask** to **"Default"** and **"Light Layer 1"**
      - Add a **Plane** GameObject above the spheres that will cast shadows. Set its **Rendering Layer Mask** to **"Default"** and **"Light Layer 1"**

   c. **Configure the Light**:
      - Select your Light (typically a Directional Light)
      - Set the light to render to both layers: **"Default"** and **"Light Layer 1"**
      - Set the light to **only cast shadows on "Light Layer 1"**

   This configuration allows you to control which objects receive shadows while all objects remain lit, helping to eliminate shadow acne on specific objects.

2. **Adjust Shadow Bias Settings**: In your light component, try adjusting the **Depth Bias** and **Normal Bias** values. Small increases can help eliminate shadow acne without disconnecting shadows entirely.

3. **Shadow Resolution**: Increase the shadow resolution in your URP Asset settings (**Main Light Shadow Resolution** or **Additional Lights Shadow Resolution**) to reduce pixelation artifacts.

For more information on rendering layers in URP, refer to the [Universal Render Pipeline documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest).

## Additional Tips

### Shadow Control Maps

When experiencing unexpected shadow behavior, review your [Shadow Control Maps](Basic.md#shadow-control-maps) configuration:
- **1st Shading Position Map**: Controls where the first shade appears
- **2nd Shading Position Map**: Controls where the second shade appears

These maps give you artistic control over shadow placement independent of lighting calculations.

### System Shadow Interaction

The shader's [Shading Step and Feather](ShadingStepAndFeather.md) settings control how Unity's system shadows interact with the stylized shading. Adjust the **System Shadows Level** parameter to control the intensity of system shadows on your material.
