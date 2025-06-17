using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;

namespace Unity.U2D.Animation.Sample
{
    internal class SkeletonSharingUI : MonoBehaviour
    {
        [SerializeField]
        SpriteLibraryAsset[] m_SpriteLibraryAssets;

        [SerializeField]
        SpriteLibrary m_TargetSpriteLibrary;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();

            var description = uiDocument.rootVisualElement.Q<Label>("Description");
            description.text = "The entire character visual can be swapped by changing the SpriteLibraryAsset that is being used by the SpriteLibrary.\n\n" +
                "The Variant character is rigged using Skeleton Sharing where the it shares the bone structure with the Primary asset";

            var choices = new List<string>(m_SpriteLibraryAssets.Length);
            var dropdownField = uiDocument.rootVisualElement.Q<DropdownField>();
            foreach (var spriteLibraryAsset in m_SpriteLibraryAssets)
                choices.Add(spriteLibraryAsset.name);
            dropdownField.choices = choices;
            dropdownField.value = m_SpriteLibraryAssets[0].name;
            dropdownField.RegisterValueChangedCallback(OnDropdownValueChanged);
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
