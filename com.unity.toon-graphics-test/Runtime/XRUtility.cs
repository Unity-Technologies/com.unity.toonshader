using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Unity.ToonShader.GraphicsTest {
    
public static class XRUtility {

public static void EnableXR() {
    //Disable everything first
    if (XRGeneralSettings.Instance.Manager.activeLoader ||
        XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        DisableXR();
    }

    if (!XRGeneralSettings.Instance.Manager.activeLoader) {
        XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
    }
    
    
    if (XRGeneralSettings.Instance.Manager.activeLoader 
        && XRGeneralSettings.Instance.Manager.isInitializationComplete)
    {
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }
    
    List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
    SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);
    int count = xrDisplaySubsystems.Count;
    for (int i = 0; i < count; i++) {
        xrDisplaySubsystems[i].Start();
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
    int count = xrDisplaySubsystems.Count;
    for (int i = 0; i < count; i++) {
        xrDisplaySubsystems[i].Stop();
    }
    
}
    
    
}

} //end namespace
