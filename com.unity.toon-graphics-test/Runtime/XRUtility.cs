using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Unity.ToonShader.GraphicsTest {
    
public static class XRUtility {

public static void EnableXR() {
    List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
    SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);
    int count = xrDisplaySubsystems.Count;
    for (int i = 0; i < count; i++) {
        xrDisplaySubsystems[i].Start();
    }
    
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
}


public static void DisableXR() {

    XRManagerSettings xrManager = XRGeneralSettings.Instance.Manager; 
    if (null!= xrManager && xrManager.isInitializationComplete)
    {
        xrManager.StopSubsystems();
        xrManager.DeinitializeLoader();
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
