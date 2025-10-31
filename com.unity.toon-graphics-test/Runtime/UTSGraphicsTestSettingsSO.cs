using System;
using UnityEngine;
using UnityEngine.TestTools.Graphics;

namespace Unity.ToonShader.GraphicsTest {
    
    [CreateAssetMenu(fileName = "UTSGraphicsSettings", menuName = "Toon Shader/UTSGraphicsSettings")]
    [Serializable]
    public class UTSGraphicsTestSettingsSO : ScriptableObject {
        public int WaitFrames = 480;
        public ImageComparisonSettings ImageComparisonSettings = new ImageComparisonSettings();
    }
}