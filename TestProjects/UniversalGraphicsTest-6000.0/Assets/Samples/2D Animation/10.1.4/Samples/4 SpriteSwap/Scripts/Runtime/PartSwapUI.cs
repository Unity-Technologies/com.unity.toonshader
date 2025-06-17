using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;

namespace Unity.U2D.Animation.Sample
{
    [Serializable]
    internal struct SwapOptionData
    {
        public SpriteResolver spriteResolver;
        public string category;
    }

    internal class PartSwapUI : MonoBehaviour
    {
        [SerializeField]
        SpriteLibrary m_SpriteLibrary;

        [SerializeField]
        SwapOptionData[] m_SwapOptionData;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            var description = uiDocument.rootVisualElement.Q<Label>("Description");
            description.text = "Different character parts can be swapped by changing the SpriteResolver's Label property on that part.";

            foreach (var swapOption in m_SwapOptionData)
            {
                var libraryAsset = m_SpriteLibrary.spriteLibraryAsset;
                var labels = libraryAsset.GetCategoryLabelNames(swapOption.category);

                var dropdown = uiDocument.rootVisualElement.Q<VisualElement>(swapOption.category).Q<DropdownField>();
                dropdown.choices = new List<string>(labels);
                dropdown.value = swapOption.spriteResolver.GetLabel();
                dropdown.RegisterValueChangedCallback(evt =>
                {
                    swapOption.spriteResolver.SetCategoryAndLabel(swapOption.category, evt.newValue);
                });
            }
        }
    }
}
