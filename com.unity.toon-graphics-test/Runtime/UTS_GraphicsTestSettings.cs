using UnityEngine;
using UnityEngine.TestTools.Graphics;

namespace Tests
{
    public class UTS_GraphicsTestSettings : MonoBehaviour
    {
        public int WaitFrames = 0;
        public bool XRCompatible = true;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        public bool CheckMemoryAllocation = false;
#else
        public bool CheckMemoryAllocation = true;
#endif //#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        
        [SerializeField] 
        private UTSImageComparisonSO m_imageComparisonSO;
        
        public ImageComparisonSettings FindImageComparisonSettings() {
            return m_imageComparisonSO != null ? m_imageComparisonSO.GetImageComparisonSettings() : null;
        }
        
    }
    
}