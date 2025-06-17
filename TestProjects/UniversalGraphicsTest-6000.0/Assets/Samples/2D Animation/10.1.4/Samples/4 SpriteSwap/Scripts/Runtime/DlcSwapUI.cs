using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;

namespace Unity.U2D.Animation.Sample
{
    internal class DlcSwapUI : MonoBehaviour
    {
        const string k_AssetBundleName = "2DAnimationSampleAssetBundles";

#if ASSETBUNDLE_ENABLED
        AssetBundle m_Bundle;
#endif

        [SerializeField]
        SpriteLibrary m_FirstLibrary;

        [SerializeField]
        SpriteLibrary m_SecondLibrary;

        [SerializeField]
        SpriteLibraryAsset[] m_InitialSpriteLibraryAssets;

        List<SpriteLibraryAsset> m_LoadedSpriteLibraryAssets = new List<SpriteLibraryAsset>();

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();

            var description = uiDocument.rootVisualElement.Q<Label>("Description");
            description.text = "These characters have downloadable content with additional visuals.\n\n" +
                "This can be achieved by exporting Sprite Library Assets in Asset Bundles to be loaded later.";

            var loadButton = uiDocument.rootVisualElement.Q<Button>("LoadButton");
            loadButton.text = "Click here to load more visuals from an Asset Bundle";
            loadButton.clicked += OnLoadClicked;

            var dropdownLeft = uiDocument.rootVisualElement.Q<DropdownField>("DropdownLeft");
            dropdownLeft.SetValueWithoutNotify(m_InitialSpriteLibraryAssets[0].name);
            dropdownLeft.RegisterValueChangedCallback(evt => SetLibrary(m_FirstLibrary, evt.newValue));

            var dropdownRight = uiDocument.rootVisualElement.Q<DropdownField>("DropdownRight");
            dropdownRight.SetValueWithoutNotify(m_InitialSpriteLibraryAssets[0].name);
            dropdownRight.RegisterValueChangedCallback(evt => SetLibrary(m_SecondLibrary, evt.newValue));

            UpdateAssetBundleDropdownSelection();
        }

        void SetLibrary(SpriteLibrary library, string assetName)
        {
            var availableAssets = new List<SpriteLibraryAsset>();
            availableAssets.AddRange(m_InitialSpriteLibraryAssets);
            availableAssets.AddRange(m_LoadedSpriteLibraryAssets);

            SpriteLibraryAsset newAsset = null;

            foreach (var spriteLibraryAsset in availableAssets)
            {
                if (spriteLibraryAsset.name == assetName)
                {
                    newAsset = spriteLibraryAsset;
                    break;
                }
            }

            library.spriteLibraryAsset = newAsset;
        }

        void LoadAssetBundle()
        {
#if ASSETBUNDLE_ENABLED
            AssetBundle.UnloadAllAssetBundles(true);

            var assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, k_AssetBundleName);
            var assetBundlePath = Path.Combine(assetBundleDirectory, k_AssetBundleName);
            m_Bundle = AssetBundle.LoadFromFile(assetBundlePath);

            Debug.Assert(m_Bundle != null, "Asset bundle failed to load.");
#endif
        }

        void OnLoadClicked()
        {
#if ASSETBUNDLE_ENABLED
            LoadAssetBundle();
            if (m_Bundle != null)
            {
                var manifest = m_Bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest == null)
                {
                    Debug.LogWarning("Unable to load manifest");
                    return;
                }

                var assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, k_AssetBundleName);
                foreach (var assetBundleName in manifest.GetAllAssetBundles())
                {
                    var subBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundleDirectory, assetBundleName));
                    var assets = subBundle.LoadAllAssets();
                    foreach (var asset in assets)
                    {
                        if (asset is SpriteLibraryAsset spriteLibraryAsset)
                            m_LoadedSpriteLibraryAssets.Add(spriteLibraryAsset);
                    }
                }
            }
#endif

            var loadButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("LoadButton");
            loadButton.SetEnabled(false);

            UpdateAssetBundleDropdownSelection();
        }

        void UpdateAssetBundleDropdownSelection()
        {
            var uiDocument = GetComponent<UIDocument>();
            var dropdownLeft = uiDocument.rootVisualElement.Q<DropdownField>("DropdownLeft");
            var dropdownRight = uiDocument.rootVisualElement.Q<DropdownField>("DropdownRight");

            var choices = new List<string>();
            foreach (var spriteLibraryAsset in m_InitialSpriteLibraryAssets)
                choices.Add(spriteLibraryAsset.name);
            foreach (var spriteLibraryAsset in m_LoadedSpriteLibraryAssets)
                choices.Add(spriteLibraryAsset.name);

            dropdownLeft.choices = choices;
            dropdownRight.choices = choices;
        }
    }
}
