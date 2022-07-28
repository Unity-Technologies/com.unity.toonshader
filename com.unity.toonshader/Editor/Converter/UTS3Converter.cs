using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.IO;

namespace UnityEditor.Rendering.Toon
{
    internal class UTS3Converter : EditorWindow
    {
        [Serializable]
        internal struct ConverterItems
        {
            public List<ConverterItemDescriptor> itemDescriptors;
        }

        // Status for each row item to say in which state they are in.
        // This will make sure they are showing the correct icon
        [Serializable]
        enum Status
        {
            Pending,
            Warning,
            Error,
            Success
        }

        // This is the serialized class that stores the state of each item in the list of items to convert
        [Serializable]
        class ConverterItemState
        {
            public bool isActive;

            // Message that will be displayed on the icon if warning or failed.
            public string message;

            // Status of the converted item, Pending, Warning, Error or Success
            public Status status;

            internal bool hasConverted = false;
        }

        // Each converter uses the active bool
        // Each converter has a list of active items/assets
        // We do this so that we can use the binding system of the UI Elements
        [Serializable]
        class ConverterState
        {
            // This is the enabled state of the whole converter
            public bool isEnabled;
            public bool isActive;
            public bool isLoading; // to name
            public bool isInitialized;
            public List<ConverterItemState> items = new List<ConverterItemState>();

            public int pending;
            public int warnings;
            public int errors;
            public int success;
            internal int index;

            public bool isActiveAndEnabled => isEnabled && isActive;
            public bool requiresInitialization => !isInitialized && isActiveAndEnabled;
        }


        // these are not included in CoreEditorStyles in 2020.3 and so on.
        /// <summary> Warning icon </summary>
        public static Texture2D iconWarn;
        /// <summary> Help icon </summary>
        public static Texture2D iconHelp;
        /// <summary> Fail icon </summary>
        public static Texture2D iconFail;
        /// <summary> Success icon </summary>
        public static Texture2D iconSuccess;
        /// <summary> Complete icon </summary>
        public static Texture2D iconComplete;
        /// <summary> Pending icon </summary>
        public static Texture2D iconPending;

        Tuple<string, Texture2D> converterStateInfoDisabled;
        Tuple<string, Texture2D> converterStateInfoPendingInitialization;
        Tuple<string, Texture2D> converterStateInfoPendingConversion;
        Tuple<string, Texture2D> converterStateInfoPendingConversionWarning;
        Tuple<string, Texture2D> converterStateInfoCompleteErrors;
        Tuple<string, Texture2D> converterStateInfoComplete;

        public VisualTreeAsset converterEditorAsset;
        public VisualTreeAsset converterItem;
        public VisualTreeAsset converterWidgetMainAsset;
        internal static string versionString => "0.8.0-preview";

        static int s_materialCount = 0;
        static string[] s_guids;
        static int s_versionErrorCount = 0;
        const string legacyShaderPrefix = "UnityChanToonShader/";

        ScrollView m_ScrollView;
        VisualElement m_ConverterSelectedVE;
        Button m_ConvertButton;
        Button m_InitButton;
        Button m_InitAnConvertButton;
        Button m_ContainerHelpButton;

        bool m_InitAndConvert;

        List<RenderPipelineConverter> m_CoreConvertersList = new List<RenderPipelineConverter>();
        List<VisualElement> m_VEList = new List<VisualElement>();
        // This list needs to be as long as the amount of converters
        List<ConverterItems> m_ItemsToConvert = new List<ConverterItems>();

        //List<List<ConverterItemDescriptor>> m_ItemsToConvert = new List<List<ConverterItemDescriptor>>();
        SerializedObject m_SerializedObject;

        List<string> m_ContainerChoices = new List<string>();
        List<RenderPipelineConverterContainer> m_Containers = new List<RenderPipelineConverterContainer>();
        int m_ContainerChoiceIndex = 0;
        int m_WorkerCount;

        // This is a list of Converter States which holds a list of which converter items/assets are active
        // There is one for each Converter.
        [SerializeField] List<ConverterState> m_ConverterStates = new List<ConverterState>();

        TypeCache.TypeCollection m_ConverterContainers;

        RenderPipelineConverterContainer currentContainer => m_Containers[m_ContainerChoiceIndex];

        UTS3GUI.RenderPipeline m_selectedRenderPipeline;

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
#if false // UNITY_2021_1_OR_NEWER
            iconHelp = CoreEditorUtils.FindTexture("_Help");
#endif
            iconWarn = CoreEditorUtils.LoadIcon("icons", "console.warnicon", ".png");
            iconFail = CoreEditorUtils.LoadIcon("icons", "console.erroricon", ".png");
            iconSuccess = EditorGUIUtility.FindTexture("TestPassed");
            iconComplete = CoreEditorUtils.LoadIcon("icons", "GreenCheckmark", ".png");
            iconPending = EditorGUIUtility.FindTexture("Toolbar Minus");

            converterStateInfoDisabled = new Tuple<string, Texture2D>("Converter Disabled", null);
            converterStateInfoPendingInitialization = new Tuple<string, Texture2D>("Pending Initialization", iconPending);
            converterStateInfoPendingConversion = new Tuple<string, Texture2D>("Pending Conversion", iconPending);
            converterStateInfoPendingConversionWarning = new Tuple<string, Texture2D>("Pending Conversion with Warnings", iconWarn);
            converterStateInfoCompleteErrors = new Tuple<string, Texture2D>("Conversion Complete with Errors", iconFail);
            converterStateInfoComplete = new Tuple<string, Texture2D>("Conversion Complete", iconComplete);

            string theme = EditorGUIUtility.isProSkin ? "dark" : "light";
            InitIfNeeded();

            if (m_ConverterContainers.Any())
            {
                m_SerializedObject = new SerializedObject(this);
                converterEditorAsset.CloneTree(rootVisualElement);
#if false
                rootVisualElement.Q<DropdownField>("conversionsDropDown").choices = m_ContainerChoices;
                rootVisualElement.Q<DropdownField>("conversionsDropDown").index = m_ContainerChoiceIndex;
#else
                rootVisualElement.Q<PopupVE>("conversionsDropDown").choices = m_ContainerChoices;
                rootVisualElement.Q<PopupVE>("conversionsDropDown").index = m_ContainerChoiceIndex;
#endif
                // Getting the scrollview where the converters should be added
                m_ScrollView = rootVisualElement.Q<ScrollView>("convertersScrollView");

                m_ConvertButton = rootVisualElement.Q<Button>("convertButton");
                m_ConvertButton.RegisterCallback<ClickEvent>(Convert);

                m_InitButton = rootVisualElement.Q<Button>("initializeButton");
                m_InitButton.RegisterCallback<ClickEvent>(InitializeAllActiveConverters);

                m_InitAnConvertButton = rootVisualElement.Q<Button>("initializeAndConvert");
                m_InitAnConvertButton.RegisterCallback<ClickEvent>(InitializeAndConvert);
#if UNITY_2021_1_OR_NEWER
                m_ContainerHelpButton = rootVisualElement.Q<Button>("containerHelpButton");
//                m_ContainerHelpButton.RegisterCallback<ClickEvent>(GotoHelpURL);
                m_ContainerHelpButton.Q<Image>("containerHelpImage").image = CoreEditorStyles.iconHelp;
                m_ContainerHelpButton.RemoveFromClassList("unity-button");
                m_ContainerHelpButton.AddToClassList(theme);
#endif
                RecreateUI();
            }

        }


        static string packageFullPath
        {
            get; set;
        }
        void OnEnable()
        {
            InitIfNeeded();

        }

        private static string GetPackageFullPath()
        {
            const string kUtsPackageName = "com.unity.toonshader";
            // Check for potential UPM package
            string packagePath = Path.GetFullPath("Packages/" + kUtsPackageName);
            if (Directory.Exists(packagePath))
            {
                return packagePath;
            }
            return null;
        }
        void InitIfNeeded()
        {
            m_selectedRenderPipeline = UTS3GUI.currentRenderPipeline;
            packageFullPath = GetPackageFullPath();

            bool isUts2Installed = CheckUTS2isInstalled();
            bool isUts2SupportedVersion = CheckUTS2VersionError();

            if (m_CoreConvertersList.Any())
                return;
            m_CoreConvertersList = new List<RenderPipelineConverter>();
            // This is the drop down choices.
            m_ConverterContainers = TypeCache.GetTypesDerivedFrom<RenderPipelineConverterContainer>();
            foreach (var containerType in m_ConverterContainers)
            {
                var container = (RenderPipelineConverterContainer)Activator.CreateInstance(containerType);
                m_Containers.Add(container);
            }

            // this need to be sorted by Priority property
            m_Containers = m_Containers
                .OrderBy(o => o.priority).ToList();

            foreach (var container in m_Containers)
            {
                m_ContainerChoices.Add(container.name);
            }

            if (m_ConverterContainers.Any())
            {
                GetConverters();
            }
            else
            {
                ClearConverterStates();
            }
        }

        void GetConverters()
        {
            ClearConverterStates();
            var converterList = TypeCache.GetTypesDerivedFrom<RenderPipelineConverter>();

            for (int i = 0; i < converterList.Count; ++i)
            {
                // Iterate over the converters that are used by the current container
                RenderPipelineConverter conv = (RenderPipelineConverter)Activator.CreateInstance(converterList[i]);
                m_CoreConvertersList.Add(conv);
            }

            // this need to be sorted by Priority property
            m_CoreConvertersList = m_CoreConvertersList
                .OrderBy(o => o.priority).ToList();

            for (int i = 0; i < m_CoreConvertersList.Count; i++)
            {
                // Create a new ConvertState which holds the active state of the converter
                var converterState = new ConverterState
                {
                    isEnabled = m_CoreConvertersList[i].isEnabled,
                    isActive = false,
                    isInitialized = false,
                    items = new List<ConverterItemState>(),
                    index = i,
                };
                m_ConverterStates.Add(converterState);

                // This just creates empty entries in the m_ItemsToConvert.
                // This list need to have the same amount of entries as the converters
                List<ConverterItemDescriptor> converterItemInfos = new List<ConverterItemDescriptor>();
                m_ItemsToConvert.Add(new ConverterItems { itemDescriptors = converterItemInfos });
            }
        }
        void ClearConverterStates()
        {
            m_CoreConvertersList.Clear();
            m_ConverterStates.Clear();
            m_ItemsToConvert.Clear();
            m_VEList.Clear();
        }

        void InitializeAndConvert(ClickEvent evt)
        {
            m_InitAndConvert = ShouldCreateSearchIndex();

            InitializeAllActiveConverters(evt);
            if (!m_InitAndConvert)
            {
                Convert(evt);
            }
        }
        private bool SaveCurrentSceneAndContinue()
        {
#if false
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.isDirty)
            {
                if (EditorUtility.DisplayDialog("Scene is not saved.",
                    "Current scene is not saved. Please save the scene before continuing.", "Save and Continue",
                    "Cancel"))
                {
                    EditorSceneManager.SaveScene(currentScene);
                }
                else
                {
                    return false;
                }
            }
#endif
            return true;
        }

        bool ShouldCreateSearchIndex()
        {
            for (int i = 0; i < m_ConverterStates.Count; ++i)
            {
                if (m_ConverterStates[i].requiresInitialization)
                {
                    var converter = m_CoreConvertersList[i];
                    if (converter.needsIndexing)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        void AddToContextMenu(ContextualMenuPopulateEvent evt, int coreConverterIndex)
        {
            var ve = (VisualElement)evt.target;
            // Checking if this context menu should be enabled or not
            var isActive = m_ConverterStates[coreConverterIndex].items[(int)ve.userData].isActive &&
                !m_ConverterStates[coreConverterIndex].items[(int)ve.userData].hasConverted;
#if false
            evt.menu.AppendAction("Run converter for this asset",
                e =>
                {
                    ConvertIndex(coreConverterIndex, (int)ve.userData);
                    // Refreshing the list to show the new state
#if UNITY_2021_1_OR_NEWER
                    m_ConverterSelectedVE.Q<ListView>("converterItems").Rebuild();
#endif
                },
                isActive == true ? DropdownMenuAction.AlwaysEnabled : DropdownMenuAction.AlwaysDisabled);
#else
            if (isActive)
            {
                evt.menu.AppendAction("Run converter for this asset",
                    e =>
                    {
                        ConvertIndex(coreConverterIndex, (int)ve.userData);
                        // Refreshing the list to show the new state
#if UNITY_2021_1_OR_NEWER
                    m_ConverterSelectedVE.Q<ListView>("converterItems").Rebuild();
#endif
                    }, DropdownMenuAction.AlwaysEnabled);
            }
            else
            {
                evt.menu.AppendAction("Run converter for this asset",
                e =>
                {
                    ConvertIndex(coreConverterIndex, (int)ve.userData);
                    // Refreshing the list to show the new state
#if UNITY_2021_1_OR_NEWER
                                m_ConverterSelectedVE.Q<ListView>("converterItems").Rebuild();
#endif
                }, DropdownMenuAction.AlwaysDisabled);
            }
#endif
        }

        void UpdateInfo(int stateIndex, RunItemContext ctx)
        {
            if (ctx.didFail)
            {
                m_ConverterStates[stateIndex].items[ctx.item.index].message = ctx.info;
                m_ConverterStates[stateIndex].items[ctx.item.index].status = Status.Error;
                m_ConverterStates[stateIndex].errors++;
            }
            else
            {
                m_ConverterStates[stateIndex].items[ctx.item.index].status = Status.Success;
                m_ConverterStates[stateIndex].success++;
            }

            m_ConverterStates[stateIndex].pending--;

            // Making sure that this is set here so that if user is clicking Convert again it will not run again.
            ctx.hasConverted = true;

            VisualElement child = m_ScrollView[stateIndex];
#if UNITY_2021_1_OR_NEWER
            child.Q<ListView>("converterItems").Rebuild();
#endif
        }

        void InitializeAllActiveConverters(ClickEvent evt)
        {
            if (!SaveCurrentSceneAndContinue()) return;
#if false
            // If we use search index, go async
            if (ShouldCreateSearchIndex())
            {
                // Save the current worker count. So it can be reset after the index file has been created.
                m_WorkerCount = AssetDatabase.DesiredWorkerCount;
                AssetDatabase.ForceToDesiredWorkerCount();

                AssetDatabase.DesiredWorkerCount = System.Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.8));
                CreateSearchIndex(m_URPConverterIndex);
            }
            // Otherwise do everything directly
            else
            {
                ConverterCollectData(() => { EditorUtility.ClearProgressBar(); });
            }

            void CreateSearchIndex(string name)
            {
                // Create <guid>.index in the project
                var title = $"Building {name} search index";
                EditorUtility.DisplayProgressBar(title, "Creating search index...", -1f);

                Search.SearchService.CreateIndex(name, IndexingOptions.Temporary | IndexingOptions.Extended,
                    new[] { "Assets" },
                    new[] { ".prefab", ".unity", ".asset" },
                    null, OnSearchIndexCreated);
            }

            void OnSearchIndexCreated(string name, string path, Action onComplete)
            {
                EditorUtility.ClearProgressBar();
                ConverterCollectData(() =>
                {
                    if (m_InitAndConvert)
                    {
                        Convert(null);
                        m_InitAndConvert = false;
                    }
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.DesiredWorkerCount = m_WorkerCount;
                    AssetDatabase.ForceToDesiredWorkerCount();

                    RecreateUI();
                    onComplete();
                });
            }

            void ConverterCollectData(Action onConverterDataCollectionComplete)
            {
                EditorUtility.DisplayProgressBar($"Initializing converters", $"Initializing converters...", -1f);

                int convertersToConvert = 0;
                for (int i = 0; i < m_ConverterStates.Count; ++i)
                {
                    if (m_ConverterStates[i].requiresInitialization)
                    {
                        convertersToConvert++;
                        GetAndSetData(i, onConverterDataCollectionComplete);
                    }
                }

                // If we did not kick off any converter initialization
                // We can complete everything immediately
                if (convertersToConvert == 0)
                {
                    onConverterDataCollectionComplete?.Invoke();
                }
            }
#endif
            RecreateUI();
        }

        void BackToConverters(ClickEvent evt)
        {
            HideConverterLayout(m_ConverterSelectedVE);
        }

        void RecreateUI()
        {
            m_SerializedObject.Update();
            // This is temp now to get the information filled in
#if false
            rootVisualElement.Q<DropdownField>("conversionsDropDown").RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                m_ContainerChoiceIndex = rootVisualElement.Q<DropdownField>("conversionsDropDown").index;
                rootVisualElement.Q<TextElement>("conversionInfo").text = currentContainer.info;
                HideUnhideConverters();
            });
#else
            rootVisualElement.Q<PopupVE>("conversionsDropDown").RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                m_ContainerChoiceIndex = rootVisualElement.Q<PopupVE>("conversionsDropDown").index;
                rootVisualElement.Q<TextElement>("conversionInfo").text = currentContainer.info;
                HideUnhideConverters();
            });
#endif


            rootVisualElement.Q<TextElement>("conversionInfo").text = currentContainer.info;
            m_ScrollView.Clear();

            for (int i = 0; i < m_CoreConvertersList.Count; ++i)
            {
                // Making an item using the converterListAsset as a template.
                // Then adding the information needed for each converter
                VisualElement item = new VisualElement();
                converterWidgetMainAsset.CloneTree(item);
                // Adding the VE so that we can hide it and unhide it when needed
                m_VEList.Add(item);

                RenderPipelineConverter conv = m_CoreConvertersList[i];
                item.name = $"{conv.name}#{conv.container.AssemblyQualifiedName}";
                item.SetEnabled(conv.isEnabled);
                item.Q<Label>("converterName").text = conv.name;
                item.Q<Label>("converterInfo").text = conv.info;

                int id = i;
                var converterEnabledToggle = item.Q<Toggle>("converterEnabled");
                converterEnabledToggle.RegisterCallback<ClickEvent>((evt) =>
                {
                    ConverterStatusInfo(id, item);
                    InitOrConvert();
                    // This toggle needs to stop propagation since it is inside another clickable element
                    evt.StopPropagation();
                });
                var topElement = item.Q<VisualElement>("converterTopVisualElement");
                topElement.RegisterCallback<ClickEvent>((evt) =>
                {
                    ShowConverterLayout(item);
                    item.Q<VisualElement>("allNoneVE").style.display = DisplayStyle.Flex;
                    item.Q<Label>("all").RegisterCallback<ClickEvent>(evt =>
                    {
                        ToggleAllNone(evt, id, true, item);
                    });
                    item.Q<Label>("none").RegisterCallback<ClickEvent>(evt =>
                    {
                        ToggleAllNone(evt, id, false, item);
                    });
                });

                // setup the images
                item.Q<Image>("pendingImage").image = iconPending;
                item.Q<Image>("pendingImage").tooltip = "Pending";
                var pendingLabel = item.Q<Label>("pendingLabel");
                item.Q<Image>("warningImage").image = iconWarn;
                item.Q<Image>("warningImage").tooltip = "Warnings";
                var warningLabel = item.Q<Label>("warningLabel");
                item.Q<Image>("errorImage").image = iconFail;
                item.Q<Image>("errorImage").tooltip = "Failed";
                var errorLabel = item.Q<Label>("errorLabel");
                item.Q<Image>("successImage").image = iconComplete;
                item.Q<Image>("successImage").tooltip = "Success";
                var successLabel = item.Q<Label>("successLabel");

                converterEnabledToggle.bindingPath =
                    $"{nameof(m_ConverterStates)}.Array.data[{i}].{nameof(ConverterState.isActive)}";
                pendingLabel.bindingPath =
                    $"{nameof(m_ConverterStates)}.Array.data[{i}].{nameof(ConverterState.pending)}";
                warningLabel.bindingPath =
                    $"{nameof(m_ConverterStates)}.Array.data[{i}].{nameof(ConverterState.warnings)}";
                errorLabel.bindingPath =
                    $"{nameof(m_ConverterStates)}.Array.data[{i}].{nameof(ConverterState.errors)}";
                successLabel.bindingPath =
                    $"{nameof(m_ConverterStates)}.Array.data[{i}].{nameof(ConverterState.success)}";

                ConverterStatusInfo(id, item);

                VisualElement child = item;
                ListView listView = child.Q<ListView>("converterItems");

                listView.showBoundCollectionSize = false;
                listView.bindingPath = $"{nameof(m_ConverterStates)}.Array.data[{i}].{nameof(ConverterState.items)}";

                // Update the amount of things to convert
                UpdateSelectedConverterItems(id, child);

                listView.makeItem = () =>
                {
                    var convertItem = converterItem.CloneTree();
                    // Adding the contextual menu for each item
                    convertItem.AddManipulator(new ContextualMenuManipulator(evt => AddToContextMenu(evt, id)));
                    return convertItem;
                };

                listView.bindItem = (element, index) =>
                {
                    m_SerializedObject.Update();
                    var property = m_SerializedObject.FindProperty($"{listView.bindingPath}.Array.data[{index}]");

                    // ListView doesn't bind the child elements for us properly, so we do that for it
                    // In the UXML our root is a BindableElement, as we can't bind otherwise.
                    var bindable = (BindableElement)element;
                    bindable.BindProperty(property);

                    // Adding index here to userData so it can be retrieved later
                    element.userData = index;

                    Status status = (Status)property.FindPropertyRelative("status").enumValueIndex;
                    string info = property.FindPropertyRelative("message").stringValue;

                    element.Q<Toggle>("converterItemActive").RegisterCallback<ClickEvent>((evt) =>
                    {
                        UpdateSelectedConverterItems(id, child);
                        DeselectAllNoneLabels(item);
                    });

                    ConverterItemDescriptor convItemDesc = m_ItemsToConvert[id].itemDescriptors[index];

                    element.Q<Label>("converterItemName").text = convItemDesc.name;
                    element.Q<Label>("converterItemPath").text = convItemDesc.info;

                    if (!string.IsNullOrEmpty(convItemDesc.helpLink))
                    {
                        element.Q<Image>("converterItemHelpIcon").image = iconHelp;
                        element.Q<Image>("converterItemHelpIcon").tooltip = convItemDesc.helpLink;
                    }

                    // Changing the icon here depending on the status.
                    Texture2D icon = null;

                    switch (status)
                    {
                        case Status.Pending:
                            icon = iconPending;
                            break;
                        case Status.Error:
                            icon = iconFail;
                            break;
                        case Status.Warning:
                            icon = iconWarn;
                            break;
                        case Status.Success:
                            icon = iconComplete;
                            break;
                    }

                    element.Q<Image>("converterItemStatusIcon").image = icon;
                    element.Q<Image>("converterItemStatusIcon").tooltip = info;
                };
#if UNITY_2022_2_OR_NEWER
                listView.selectionChanged += obj => { m_CoreConvertersList[id].OnClicked(listView.selectedIndex); };
#else
                listView.onSelectionChange += obj => { m_CoreConvertersList[id].OnClicked(listView.selectedIndex); };
#endif
                listView.unbindItem = (element, index) =>
                {
                    var bindable = (BindableElement)element;
                    bindable.Unbind();
                };

                m_ScrollView.Add(item);
            }

            InitOrConvert();
            HideUnhideConverters();
            rootVisualElement.Bind(m_SerializedObject);
        }

        private void HideUnhideConverters()
        {
            var type = currentContainer.GetType();
#if false //UNITY_2021_1_OR_NEWER
            if (DocumentationUtils.TryGetHelpURL(type, out var url))
            {
                m_ContainerHelpButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_ContainerHelpButton.style.display = DisplayStyle.None;
            }
#endif
            foreach (VisualElement childElement in m_ScrollView.Q<VisualElement>().Children())
            {
                var container = Type.GetType(childElement.name.Split('#').Last());
                if (container == type)
                {
                    childElement.style.display = DisplayStyle.Flex;
                }
                else
                {
                    childElement.style.display = DisplayStyle.None;
                }
            }
        }

        void DeselectAllNoneLabels(VisualElement item)
        {
            item.Q<Label>("all").AddToClassList("not_selected");
            item.Q<Label>("all").RemoveFromClassList("selected");

            item.Q<Label>("none").AddToClassList("not_selected");
            item.Q<Label>("none").RemoveFromClassList("selected");
        }

        void ConverterStatusInfo(int index, VisualElement item)
        {
            Tuple<string, Texture2D> info = converterStateInfoDisabled; ;
            // Check if it is active
            if (m_ConverterStates[index].isActive)
            {
                info = converterStateInfoPendingInitialization;
            }
            if (m_ConverterStates[index].isInitialized)
            {
                info = converterStateInfoPendingConversion;
            }
            if (m_ConverterStates[index].warnings > 0)
            {
                info = converterStateInfoPendingConversionWarning;
            }
            if (m_ConverterStates[index].errors > 0)
            {
                info = converterStateInfoCompleteErrors;
            }
            if (m_ConverterStates[index].errors == 0 && m_ConverterStates[index].warnings == 0 && m_ConverterStates[index].success > 0)
            {
                info = converterStateInfoComplete;
            }
            if (!m_ConverterStates[index].isActive)
            {
                info = converterStateInfoDisabled;
            }
            item.Q<Label>("converterStateInfoL").text = info.Item1;
            item.Q<Image>("converterStateInfoIcon").image = info.Item2;
        }

        void ShowConverterLayout(VisualElement element)
        {
            m_ConverterSelectedVE = element;
            rootVisualElement.Q<VisualElement>("converterEditorMainVE").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("singleConverterVE").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("singleConverterVE").Add(element);
            element.Q<VisualElement>("converterItems").style.display = DisplayStyle.Flex;
            element.Q<VisualElement>("informationVE").style.display = DisplayStyle.Flex;

            rootVisualElement.Q<Button>("backButton").RegisterCallback<ClickEvent>(BackToConverters);
        }

        void HideConverterLayout(VisualElement element)
        {
            rootVisualElement.Q<VisualElement>("converterEditorMainVE").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("singleConverterVE").style.display = DisplayStyle.None;
            rootVisualElement.Q<VisualElement>("singleConverterVE").Remove(element);

            element.Q<VisualElement>("converterItems").style.display = DisplayStyle.None;
            element.Q<VisualElement>("informationVE").style.display = DisplayStyle.None;

            RecreateUI();
            m_ConverterSelectedVE = null;
        }

        void ToggleAllNone(ClickEvent evt, int index, bool value, VisualElement item)
        {
            var conv = m_ConverterStates[index];
            if (conv.items.Count > 0)
            {
                foreach (var convItem in conv.items)
                {
                    convItem.isActive = value;
                }
                UpdateSelectedConverterItems(index, item);
                // Changing the look of the labels
                if (value)
                {
                    item.Q<Label>("all").AddToClassList("selected");
                    item.Q<Label>("all").RemoveFromClassList("not_selected");

                    item.Q<Label>("none").AddToClassList("not_selected");
                    item.Q<Label>("none").RemoveFromClassList("selected");
                }
                else
                {
                    item.Q<Label>("none").AddToClassList("selected");
                    item.Q<Label>("none").RemoveFromClassList("not_selected");

                    item.Q<Label>("all").AddToClassList("not_selected");
                    item.Q<Label>("all").RemoveFromClassList("selected");
                }
            }
        }

        void UpdateSelectedConverterItems(int index, VisualElement element)
        {
            int count = 0;
            foreach (ConverterItemState state in m_ConverterStates[index].items)
            {
                if (state.isActive)
                {
                    count++;
                }
            }

            element.Q<Label>("converterStats").text = $"{count}/{m_ItemsToConvert[index].itemDescriptors.Count} selected";
        }

        void InitOrConvert()
        {
            bool allSelectedHasInitialized = true;
            // Check if all ticked ones have been initialized.
            // If not then Init Button should be active
            // Get all active converters

            if (m_ConverterStates.Any())
            {
                foreach (ConverterState state in m_ConverterStates)
                {
                    if (state.isActiveAndEnabled && !state.isInitialized)
                    {
                        allSelectedHasInitialized = false;
                        break;
                    }
                }
            }
            else
            {
                // If no converters is active.
                // we should make the text somewhat disabled
                allSelectedHasInitialized = false;
            }

            if (allSelectedHasInitialized)
            {
                m_ConvertButton.style.display = DisplayStyle.Flex;
                m_InitButton.style.display = DisplayStyle.None;
            }
            else
            {
                m_ConvertButton.style.display = DisplayStyle.None;
                m_InitButton.style.display = DisplayStyle.Flex;
            }
        }

        internal static void DontSaveToLayout(EditorWindow wnd)
        {
#if true
            return;
#else
            // Making sure that the window is not saved in layouts.
            Assembly assembly = typeof(EditorWindow).Assembly;
            var editorWindowType = typeof(EditorWindow);
            var hostViewType = assembly.GetType("UnityEditor.HostView");
            var containerWindowType = assembly.GetType("UnityEditor.ContainerWindow");
            var parentViewField = editorWindowType.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
            var parentViewValue = parentViewField.GetValue(wnd);
            // window should not be saved to layout
            var containerWindowProperty =
                hostViewType.GetProperty("window", BindingFlags.Instance | BindingFlags.Public);
            var parentContainerWindowValue = containerWindowProperty.GetValue(parentViewValue);
            var dontSaveToLayoutField =
                containerWindowType.GetField("m_DontSaveToLayout", BindingFlags.Instance | BindingFlags.NonPublic);
            dontSaveToLayoutField.SetValue(parentContainerWindowValue, true);
#endif
        }
        void ConvertIndex(int coreConverterIndex, int index)
        {
            if (!m_ConverterStates[coreConverterIndex].items[index].hasConverted)
            {
                m_ConverterStates[coreConverterIndex].items[index].hasConverted = true;
                var item = new ConverterItemInfo()
                {
                    index = index,
                    descriptor = m_ItemsToConvert[coreConverterIndex].itemDescriptors[index],
                };
                var ctx = new RunItemContext(item);
                m_CoreConvertersList[coreConverterIndex].OnRun(ref ctx);
                UpdateInfo(coreConverterIndex, ctx);
            }
        }

        void Convert(ClickEvent evt)
        {
        }
        static bool CheckUTS2VersionError()
        {
            s_guids = AssetDatabase.FindAssets("t:Material", null);
            int materialCount = 0;

            for (int ii = 0; ii < s_guids.Length; ii++)
            {
                var guid = s_guids[ii];


                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                var shaderName = material.shader.ToString();
                if (!shaderName.StartsWith(legacyShaderPrefix))
                {
                    continue;

                }
                const string utsVersionProp = "_utsVersion";
                if (material.HasProperty(utsVersionProp))
                {
                    float utsVersion = material.GetFloat(utsVersionProp);
                    if (utsVersion < 2.07)
                    {
                        s_versionErrorCount++;
                        continue;
                    }
                }
                else
                {
                    s_versionErrorCount++;
                    continue;
                }
                materialCount++;
            }
            s_materialCount = materialCount;
            if (s_versionErrorCount > 0)
            {
                return true;
            }
            return false;
        }

        static bool CheckUTS2isInstalled()
        {
            var shaders = AssetDatabase.FindAssets("t:Shader", new string[] { "Assets" });
            foreach (var guid in shaders)
            {
                foreach (var shader in BuiltInRPShaders.stdShaders)
                {
                    if (guid == shader.m_Guid)
                    {
                        /*
                                            var filename = AssetDatabase.GUIDToAssetPath(guid);

                                            if (!filename.EndsWith(kLegacyShaderFileName + kShaderFileNameExtention))
                                            {
                                                return true;
                                            }
                        */
                        return true;

                    }
                }
                foreach (var shader in BuiltInRPShaders.tessShaders)
                {
                    if (guid == shader.m_Guid)
                    {
                        /*
                                            var filename = AssetDatabase.GUIDToAssetPath(guid);

                                            if (!filename.EndsWith(kLegacyShaderFileName + kShaderFileNameExtention))
                                            {
                                                return true;
                                            }
                        */
                        return true;

                    }
                }
            }
            return false;
        }
        static UTS2GUID FindUTS2GUID(string guid)
        {
            var ret = Array.Find<UTS2GUID>(BuiltInRPShaders.stdShaders, element => element.m_Guid == guid);
            foreach (var shader in BuiltInRPShaders.stdShaders)
            {
                if (shader.m_Guid == guid)
                {
                    return shader;
                }
            }
            foreach (var shader in BuiltInRPShaders.tessShaders)
            {
                if (shader.m_Guid == guid)
                {
                    return shader;
                }
            }
            return null;
        }
    }
}