using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class Blit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable() {
        //RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            
    }

    void OnDisable() {
        //RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;        
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
    
    
    private void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cams) {
        foreach (Camera cam in cams) {
            if (cam == m_camera) {
                Debug.Log("Doing Blit");
                Graphics.Blit(m_tex, cam.targetTexture);
                return;
            }
        }
    }
    
    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        
        if (camera.cameraType != CameraType.Game)
            return;        
        
        // Filter which cameras should receive the background
        if (camera!= m_camera)
            return;

        if (m_tex == null)
            return;

        // For URP, the current camera target is an internal RT. We can blit to it at the start
        // to "prime" the background, then URP will draw opaque/transparent on top (depending on clear flags).
        // Ensure the camera Clear Flags are set to Don't Clear or Depth Only if you want to keep the background.
        var cmd = CommandBufferPool.Get("Blit Background");

        // Try to ensure we're drawing into the camera's current color target.
        CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget);
        cmd.Blit(m_tex, BuiltinRenderTextureType.CameraTarget);
        
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }    

    private void OnValidate() {
        m_camera = GetComponent<Camera>();
    }

//--------------------------------------------------------------------------------------------------------------------------------------------------------------



    [SerializeField] private RenderTexture m_tex;
    [SerializeField] Camera m_camera = null;

}
