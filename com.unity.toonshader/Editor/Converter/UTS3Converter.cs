using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace UnityEditor.Rendering.Toon
{
    internal class UTS3Converter : EditorWindow
    {
        [Serializable]
        internal struct ConverterItems
        {
            public List<ConverterItemDescriptor> itemDescriptors;
        }

        public VisualTreeAsset converterEditorAsset;
        public VisualTreeAsset converterItem;
        public VisualTreeAsset converterWidgetMainAsset;
        internal static string versionString => "0.8.0-preview";

        [MenuItem("Window/Rendering/Unity Toon Shader Converter", false, 51)]
        public static void ShowWindow()
        {
            UTS3Converter wnd = GetWindow<UTS3Converter>();
            wnd.titleContent = new GUIContent("Unity Toon Shader Converter");

            wnd.minSize = new Vector2(650f, 400f);
            wnd.Show();
        }

        private void CreateGUI()
        {
            converterEditorAsset.CloneTree(rootVisualElement);
            converterItem.CloneTree(rootVisualElement);
            converterWidgetMainAsset.CloneTree(rootVisualElement);
        }
    }
}