// using System;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;
// using UnityEngine.Rendering;
//
// public class BlurRenderPass : ScriptableRenderPass
// {
//     private static readonly int horizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
//     private static readonly int verticalBlurId = Shader.PropertyToID("_VerticalBlur");
//
//     private BlurSettings defaultSettings;
//     private Material material;
//
//     private RenderTexture m_rt;
//
//     public BlurRenderPass(RenderTexture rt) {
//         m_rt = rt;
//     }
//
//     public override void Configure(CommandBuffer cmd,
//         RenderTextureDescriptor cameraTextureDescriptor)
//     {
//     }
//
//
//     public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//     {
//         
//         CommandBuffer cmd = CommandBufferPool.Get();
//
//         RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
//
//
//         // Blit from the camera target to the temporary render texture,
//         // using the first shader pass.
//         Blit(cmd, m_rt, cameraTargetHandle, material, 0);
//
//         //Execute the command buffer and release it back to the pool.
//         context.ExecuteCommandBuffer(cmd);
//         CommandBufferPool.Release(cmd);
//     }
//
//     public void Dispose()
//     {
//     #if UNITY_EDITOR
//         if (EditorApplication.isPlaying)
//         {
//             Object.Destroy(material);
//         }
//         else
//         {
//             Object.DestroyImmediate(material);
//         }
//     #else
//                 Object.Destroy(material);
//     #endif
//
//         if (blurTextureHandle != null) blurTextureHandle.Release();
//     }
// }
//
// //--------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
// public class BlurRendererFeature : ScriptableRendererFeature
// {
//     [SerializeField] private BlurSettings settings;
//     private BlurRenderPass blurRenderPass;
//     
//
//     [SerializeField] private RenderTexture m_renderTexture;
//
//         
//     public override void Create()
//     {
//         if (m_renderTexture == null)
//         {
//             return;
//         }
//         blurRenderPass = new BlurRenderPass(m_renderTexture);
//         
//         blurRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
//     }
//
//     public override void AddRenderPasses(ScriptableRenderer renderer,
//         ref RenderingData renderingData)
//     {
//         if (renderingData.cameraData.cameraType == CameraType.Game)
//         {
//             renderer.EnqueuePass(blurRenderPass);
//         }
//     }
//
//     protected override void Dispose(bool disposing)
//     {
//         blurRenderPass.Dispose();
// #if UNITY_EDITOR
//         if (EditorApplication.isPlaying)
//         {
//             Destroy(material);
//         }
//         else
//         {
//             DestroyImmediate(material);
//         }
// #else
//                 Destroy(material);
// #endif
//     }
// }
//
// [Serializable]
// public class BlurSettings
// {
//     [Range(0, 0.4f)] public float horizontalBlur;
//     [Range(0, 0.4f)] public float verticalBlur;
// }