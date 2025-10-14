using UnityEngine.UIElements;

namespace UnityEditor.Rendering.Toon {
    
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    internal partial class PopupVE : PopupField<string> {

#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<PopupVE> {
        }
#endif
    }
}