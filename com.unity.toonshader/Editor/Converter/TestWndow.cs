using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
namespace UnityEditor.Rendering.Toon
{
    public class TestWndow : EditorWindow
    {
        public VisualTreeAsset converterEditorAsset;


        [MenuItem("Window/Rendering/Unity Toon Shader TestWindow", false, 51)]
        public static void ShowWindow()
        {
            TestWndow wnd = GetWindow<TestWndow>();
            wnd.titleContent = new GUIContent("Test Window");

            wnd.minSize = new Vector2(650f, 400f);
            wnd.Show();
        }

        private void CreateGUI()
        {
            converterEditorAsset.CloneTree(rootVisualElement);
        }
    }
}