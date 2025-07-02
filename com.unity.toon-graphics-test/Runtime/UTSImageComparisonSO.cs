using System;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

namespace Unity.ToonShader.GraphicsTest {

[Serializable]
[CreateAssetMenu(menuName = "Unity Toon Shader/UTS Image Comparison Settings")]
public class UTSImageComparisonSO : ScriptableObject {
    [SerializeField] private ImageComparisonSettings m_imageComparisonSettings = new ImageComparisonSettings() {
        TargetWidth = 960,
        TargetHeight = 540,
        PerPixelCorrectnessThreshold = 0.001f,
        AverageCorrectnessThreshold = 0.005f,
        UseHDR = false,
        UseBackBuffer = false,
        ImageResolution = ImageComparisonSettings.Resolution.w1920h1080,
    };

    public ImageComparisonSettings GetImageComparisonSettings() => m_imageComparisonSettings;
}


}