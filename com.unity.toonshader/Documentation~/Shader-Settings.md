# Shader Settings

The Shader Settings provides basic settings that are not specific to  cel-shading but are needed for general CG.

- [Culling Mode](#culling-Mode)
- [Auto Render Queue](#auto-render-queue)
- [Render Queue](#render-queue)
- [Transparency](#transparency)
- [Stencil](#stencil)
- [Stencil Value](#stencil-value)

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

### Stencil
Control the stencil buffer which manipulates pixel drawing.

|  Value   |  Description  | 
| ---- | ---- | 
|  Replace Stencil Buffer with  | Replaces a pixel value in stencil buffer with [Stencil Value](#stencil-value) when drawing.|
|  Draw If Not Equal to  | Draw a pixel when the vaule in stencil buffer is not equal to [Stencil Value](#stencil-value).|
|  Off  |  Nothing is written to stencil buffer and [Stencil Value](#stencil-value) doesn't affect at all when drawing.|

### Stencil Value
Stencil value that is submitted to the stencil buffer for controlling the per-pixel drawing. Min is 0. Max is 255. The dafualt is 0.