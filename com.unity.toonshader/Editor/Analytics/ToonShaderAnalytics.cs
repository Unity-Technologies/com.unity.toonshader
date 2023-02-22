namespace UnityEditor.Rendering.Toon {
internal partial class ToonShaderAnalytics {
    private const string EVENT_NAME_PREFIX = "toonshader_";
    
    [InitializeOnLoadMethod]
    private static void OnLoad() {
        AnimeToolboxAnalytics.SendEvent(new LoadEvent(
#if HDRP_IS_INSTALLED_FOR_UTS
            "high-definition"
#elif URP_IS_INSTALLED_FOR_UTS
            "universal"
#else
            "built-in"
#endif
        ));
    }
}
}
