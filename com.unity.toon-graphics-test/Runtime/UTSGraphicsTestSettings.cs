using UnityEngine;

namespace Unity.ToonShader.GraphicsTest
{
public class UTSGraphicsTestSettings : MonoBehaviour {
    public UTSGraphicsTestSettingsSO SO;

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    public bool CheckMemoryAllocation = false;
#else
    public bool CheckMemoryAllocation = true;
#endif //#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

}

}