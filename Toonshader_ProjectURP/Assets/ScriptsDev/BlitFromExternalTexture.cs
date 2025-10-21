using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public class BlitFromExternalTexture : ScriptableRendererFeature
{
    // The texture to use as input 
    public RenderTexture textureToUse;
    public Texture2D texture2DToUse;
    
    BlitFromTexture customPass;
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRendering;

    
    public override void Create()
    {
        // Create an instance of the render pass, and pass in the input texture 
        customPass = new BlitFromTexture(textureToUse, texture2DToUse);

        customPass.renderPassEvent = renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(customPass);
    }

    class BlitFromTexture : ScriptableRenderPass
    {
        class PassData
        {
            internal TextureHandle textureToRead;
            internal RenderTexture tempColorTexture;
        }

        private RenderTexture texturePassedIn;
        private Texture2D  texture2D;

        public BlitFromTexture(RenderTexture textureIn, Texture2D tex2d)
        {
            // In the render pass's constructor, set the input texture
            texturePassedIn = textureIn;
            texture2D = tex2d;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
        {
            
            Debug.Log("RecordRenderGraph");
            
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Blit from external textures", out var passData))
            {
                // Create a temporary texture and set it as the render target
                RenderTextureDescriptor textureProperties = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
                textureProperties.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
                textureProperties.depthStencilFormat = GraphicsFormat.None;

                // TextureHandle texture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureProperties, "My texture", false);
                // Import the camera's color target
                
                UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();
                //UniversalCameraData cameraData = frameContext.Get<UniversalCameraData>();

                TextureHandle cameraColor = resourceData.activeColorTexture;
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                // Create a render texture from the input texture
                // RTHandle rtHandle = RTHandles.Alloc(texture2D);
                // TextureHandle textureToRead = renderGraph.ImportTexture(rtHandle);

                // Import with explicit RenderTargetInfo to override any depth/stencil format
                RenderTargetInfo rtInfo = new RenderTargetInfo();
                rtInfo.width = texturePassedIn.width;
                rtInfo.height = texturePassedIn.height;
                rtInfo.volumeDepth = 1;
                rtInfo.msaaSamples = 1;
                rtInfo.format = GraphicsFormat.R8G8B8A8_UNorm;
                
                // Create a temporary RenderTexture with only color format
                RenderTextureDescriptor colorOnlyDesc = new RenderTextureDescriptor(
                    texturePassedIn.width, 
                    texturePassedIn.height
                );
                colorOnlyDesc.graphicsFormat = texturePassedIn.graphicsFormat;
                colorOnlyDesc.depthStencilFormat = GraphicsFormat.None;
                colorOnlyDesc.msaaSamples = texturePassedIn.antiAliasing;

                passData.tempColorTexture = RenderTexture.GetTemporary(colorOnlyDesc);
                Graphics.Blit(texturePassedIn, passData.tempColorTexture);                
                
                //RTHandle rtHandle = RTHandles.Alloc(width:texturePassedIn.width, height: texturePassedIn.height, format: GraphicsFormat.R8G8B8A8_UNorm);
                RTHandle rtHandle = RTHandles.Alloc(passData.tempColorTexture);
                TextureHandle textureToRead = renderGraph.ImportTexture(rtHandle);
                
                //public static RTHandle Alloc(int width, int height, int slices = 1, DepthBits depthBufferBits = DepthBits.None, GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_SRGB, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureDimension dimension = TextureDimension.Tex2D, bool enableRandomWrite = false, bool useMipMap = false, bool autoGenerateMips = true, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0, MSAASamples msaaSamples = MSAASamples.None, bool bindTextureMS = false, bool useDynamicScale = false, bool useDynamicScaleExplicit = false, RenderTextureMemoryless memoryless = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, string name = "")                
                //Alloc(int, int, GraphicsFormat, int, FilterMode, TextureWrapMode, TextureDimension, bool, bool, bool, bool, int, float, MSAASamples, bool, bool, bool, RenderTextureMemoryless, VRTextureUsage, string)
                //Alloc(int, int, TextureWrapMode, TextureWrapMode, TextureWrapMode, int, DepthBits, GraphicsFormat, FilterMode, TextureDimension, bool, bool, bool, bool, int, float, MSAASamples, bool, bool, bool, RenderTextureMemoryless, VRTextureUsage, string) 
                
                    

                // Add the texture to the pass data
                
                passData.textureToRead = textureToRead;

                // Set the texture as readable
                builder.UseTexture(passData.textureToRead, AccessFlags.Read);

                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
        }

        static void ExecutePass(PassData data, RasterGraphContext context)
        {          
            // Copy the imported texture to the render target
            Blitter.BlitTexture(context.cmd, data.textureToRead, new Vector4(1.0f,1.0f,0,0), 0, false);
            
            Debug.Log("a");

            // Dispose of the texture
            RTHandles.Release(data.textureToRead);
            RenderTexture.ReleaseTemporary(data.tempColorTexture);
        }
    }
}
