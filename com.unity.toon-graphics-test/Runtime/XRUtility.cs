using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
    SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);
    foreach (XRDisplaySubsystem xrDisplay in xrDisplaySubsystems) {
        xrDisplay.Start();
    }
    
}


public static void DisableXR() {

    if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }
    
    
    List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
    SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);
    foreach (XRDisplaySubsystem xrDisplay in xrDisplaySubsystems) {
        xrDisplay.Stop();
    }
    
}
    
    
}

} //end namespace
