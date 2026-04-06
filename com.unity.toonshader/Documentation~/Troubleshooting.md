# Troubleshooting and Tips

This page contains common issues, workarounds, and tips for using **Unity Toon Shader** effectively.

## Shadow Acne

When [Receive shadows](ShadingStepAndFeather.md#receive-shadows) is enabled, you may observe **shadow acne** -
areas within shadows that appear noisy. 
This is due to self-shadowing artifacts where the shader's shadow calculations interact with the geometry of the model, 
causing small discrepancies in depth that result in visible noise.

![](images/Troubleshooting_ShadowAcne.png)

### Workaround for Universal Render Pipeline (URP)

![](images/Troubleshooting_Shadow_URPLayers.png)

To work around shadow acne issues in URP, we can 

1. **Use Rendering Layers / Shadow Layers**: Configure rendering layers on your lights and objects to control which
   objects cast shadows on which surfaces. This gives you fine-grained control over shadow casting and can help
   eliminate unwanted shadow artifacts.

   This is an example of a step-by-step setup using ** Rendering Layers / Shadow Layers**:

   a. **Enable Rendering Layers in URP Asset**:
      - Select your URP Asset
      - Enable [**Rendering Layers**]((https://docs.unity3d.com/Manual/urp/features/rendering-layers-lights.html)) 
        with **Custom Shadow Layers**

   b. **Set up the scene**:
      - Add a **Plane** GameObject as the floor
      - Add a **Sphere** with a Toon material. Set its **Rendering Layer Mask** to **"Default"**
      - Add another **Sphere** with a Toon material. Set its **Rendering Layer Mask** to **"Default"** and
        **"Light Layer 1"**
      - Add a **Plane** GameObject above the spheres that will cast shadows. Set its **Rendering Layer Mask** to
        **"Default"** and **"Light Layer 1"**

   c. **Configure the Light**:
      - Select your Light (typically a Directional Light)
      - Set the light to render to both layers: **"Default"** and **"Light Layer 1"**
      - Set the light to **only cast shadows on "Light Layer 1"**

   This configuration allows you to control which objects receive shadows while all objects remain lit, helping to
   eliminate shadow acne on specific objects.

   For more information on rendering layers in URP, refer to the
   [Universal Render Pipeline documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest).

2. Disable [Receive shadows](ShadingStepAndFeather.md#receive-shadows) or adjust the system shadow level property.

## Additional Tips

### Shadow Control Maps

When experiencing unexpected shadow behavior, review your [Shadow Control Maps](Basic.md#shadow-control-maps)
configuration:
- **1st Shading Position Map**: Controls where the first shade appears
- **2nd Shading Position Map**: Controls where the second shade appears

These maps give you artistic control over shadow placement independent of lighting calculations.

### System Shadow Interaction

The shader's [Shading Step and Feather](ShadingStepAndFeather.md) settings control how Unity's system shadows
interact with the stylized shading. Adjust the **System Shadows Level** parameter to control the intensity of system
shadows on your material.
