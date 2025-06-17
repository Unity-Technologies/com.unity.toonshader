using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;

namespace Unity.U2D.Animation.Sample
{
    internal class RuntimeSwapUI : MonoBehaviour
    {
        [Serializable]
        class SwapEntry
        {
            public Sprite sprite = null;
            public string category = "";
            public string entry = "";
        }

        [Serializable]
        class SwapGroup
        {
            public string name;
            public Sprite defaultSprite;
            public Sprite overrideSprite;
            public SwapEntry[] swapEntries = null;
        }

        [SerializeField]
        SwapGroup[] m_SwapGroup = null;

        [SerializeField]
        SpriteLibrary m_SpriteLibraryTarget = null;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();

            var description = uiDocument.rootVisualElement.Q<Label>("Description");
            description.text = "The example uses the Sprite Library's inline override functionality to show how visual can be swap in Runtime without setting up additional Sprite Library Asset.";

            foreach (var swapGroup in m_SwapGroup)
            {
                var group = swapGroup;

                var groupName = group.name;
                var visualElement = uiDocument.rootVisualElement.Q<VisualElement>(groupName);

                var add = visualElement.Q<Image>("AddOverride");
                add.sprite = swapGroup.overrideSprite;
                add.RegisterCallback<PointerDownEvent>(_ => OverrideEntry(group));

                var remove = visualElement.Q<Image>("RemoveOverride");
                remove.sprite = swapGroup.defaultSprite;
                remove.RegisterCallback<PointerDownEvent>(_ => ResetEntry(group));
            }
        }

        void OverrideEntry(SwapGroup swapGroup)
        {
            foreach (var entry in swapGroup.swapEntries)
                m_SpriteLibraryTarget.AddOverride(entry.sprite, entry.category, entry.entry);
        }

        void ResetEntry(SwapGroup swapGroup)
        {
            foreach (var entry in swapGroup.swapEntries)
                m_SpriteLibraryTarget.RemoveOverride(entry.category, entry.entry);
        }
    }
}
