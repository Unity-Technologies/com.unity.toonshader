# Shader Settings

The Shader Settings provides basic settings that are not specific to  cel-shading but are needed for general CG.

- [Culling Mode](#culling-Mode)
- [Auto Render Queue](#auto-render-queue)
- [Render Queue](#render-queue)
- [Transparency](#transparency)

### Culling Mode
Controls which sides of polygons should be culled (not drawn).

|  Value   |  Description  | 
| ---- | ---- | 
|  Back   |  Don’t render polygons that are facing away from the viewer (default) i.e. back-facing polygons are culled.  |
|  Front  | Don’t render polygons that are facing towards the viewer. Used for turning objects inside-out. |
|  Off  |  Disables culling - all faces are drawn. Used for special effects. |

### Auto Render Queue
When enabled, rendering order is determined by system automatically.

### Render Queue
Rendering order in the scene.

### Transparency
Enables different modes that allow the simulation of a variety of transparent objects.



