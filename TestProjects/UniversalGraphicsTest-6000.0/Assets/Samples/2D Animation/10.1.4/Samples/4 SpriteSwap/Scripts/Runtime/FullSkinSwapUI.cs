using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;

namespace Unity.U2D.Animation.Sample
{
    internal class FullSkinSwapUI : MonoBehaviour
    {
        [SerializeField]
        SpriteLibraryAsset[] m_SpriteLibraryAssets;

        [SerializeField]
        SpriteLibrary m_TargetSpriteLibrary;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();

            var description = uiDocument.rootVisualElement.Q<Label>("Description");
            description.text = "The entire character visual can be swapped by changing the SpriteLibraryAsset that is being used by the SpriteLibrary.";

            var choices = new List<string>(m_SpriteLibraryAssets.Length);
            var dropdownField = uiDocument.rootVisualElement.Q<DropdownField>();
            foreach (var spriteLibraryAsset in m_SpriteLibraryAssets)
                choices.Add(spriteLibraryAsset.name);
            dropdownField.choices = choices;
            dropdownField.value = m_SpriteLibraryAssets[0].name;
            dropdownField.RegisterValueChangedCallback(OnDropdownValueChanged);

            uiDocument.rootVisualElement.MarkDirtyRepaint();
        }

        void OnDropdownValueChanged(ChangeEvent<string> evt)
        {
            SpriteLibraryAsset selectedAsset = null;
            foreach (var asset in m_SpriteLibraryAssets)
            {
                if (asset.name == evt.newValue)
                {
                    selectedAsset = asset;
                    break;
                }
            }

            if (selectedAsset != null)
                m_TargetSpriteLibrary.spriteLibraryAsset = selectedAsset;
        }
    }
}
