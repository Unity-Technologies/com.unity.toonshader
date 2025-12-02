using UnityEngine.UIElements;

namespace UnityChan {

public static class UIToolkitUtility {

    public static void AddStyleSheetToRoot(VisualElement visualElement, StyleSheet sheet) {
        VisualElement root = visualElement.parent;
        while (root.parent != null)
            root = root.parent;

        root.styleSheets.Add(sheet);
    }
}

} //end namespace