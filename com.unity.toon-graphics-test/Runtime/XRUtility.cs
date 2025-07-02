using System.Collections;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Unity.ToonShader.GraphicsTest {
    
public static class XRUtility {

public static IEnumerator EnableXRCoroutine()
{
    if (XRGeneralSettings.Instance.Manager.activeLoader ||
        XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        DisableXR();
        yield return null;
    }

    if (!XRGeneralSettings.Instance.Manager.activeLoader) {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
    }

    if (XRGeneralSettings.Instance.Manager.activeLoader 
        && XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }
}


public static void DisableXR() {

    if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }
    
    XRSettings.enabled = false;
}
    
    
}

} //end namespace
