using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public class BlitTextureRendererFeature : ScriptableRendererFeature {

    public override void Create() {
        m_customPass = new BlitTexturePass(m_textureToBlit);

        m_customPass.renderPassEvent = m_renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(m_customPass);
    }

//----------------------------------------------------------------------------------------------------------------------
    class BlitTexturePass : ScriptableRenderPass {

        public BlitTexturePass(Texture tex) {
            m_texture = tex;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext) {
            if (null==m_texture) {
                return;
            }

            using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("BlitTexturePass",
                       out PassData passData))
            {
                //Set target to the active camera color texture
                UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();
                TextureHandle cameraColor = resourceData.activeColorTexture;
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                //Create a temp RenderTexture with only color format, since we don't know if the source has depth or not
                RenderTextureDescriptor colorOnlyDesc = new RenderTextureDescriptor(
                    m_texture.width,
                    m_texture.height
                );
                colorOnlyDesc.graphicsFormat = m_texture.graphicsFormat;
                colorOnlyDesc.depthStencilFormat = GraphicsFormat.None;

                passData.TempRT = RenderTexture.GetTemporary(colorOnlyDesc);
                Graphics.Blit(m_texture, passData.TempRT); //Blit first

                //allocate and pass RTHandle
                RTHandle rtHandle = RTHandles.Alloc(passData.TempRT);
                TextureHandle textureToRead = renderGraph.ImportTexture(rtHandle);

                passData.BlitSrc = textureToRead;

                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
        }

        static void ExecutePass(PassData data, RasterGraphContext context) {
            Blitter.BlitTexture(context.cmd, data.BlitSrc, new Vector4(1.0f,1.0f,0,0), 0, false);

            // Dispose resources
            RTHandles.Release(data.BlitSrc);
            RenderTexture.ReleaseTemporary(data.TempRT);
        }

        private class PassData {
            internal TextureHandle BlitSrc;
            internal RenderTexture TempRT;
        }

        private Texture m_texture;
    }

//----------------------------------------------------------------------------------------------------------------------

    [SerializeField] Texture m_textureToBlit;
    [SerializeField] RenderPassEvent m_renderPassEvent = RenderPassEvent.BeforeRendering;

    BlitTexturePass m_customPass;
}
