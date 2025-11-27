using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace UnityChan {

[RequireComponent(typeof(Animator))]
public class BodyAnimationPreviewer : MonoBehaviour {
    void Start() {
        StartCoroutine(nameof(RandomChange));
        
        InitUI();
    }
    
    void InitUI() {
        if (null == m_rootUI)
            return;

        VisualElement root = m_rootUI.rootVisualElement;
        VisualElement block = root.Q<VisualElement>("TopRightBlock");
        VisualElement bodyUIRoot = m_bodyUITemplate.Instantiate(block);
        
        
        m_bodyUIContainer = bodyUIRoot.Q<VisualElement>("BodyUIContainer");
        
        Toggle bodyUIToggle = bodyUIRoot.Q<Toggle>("BodyUIToggle");
        bodyUIToggle.SetValueWithoutNotify(true);
        bodyUIToggle.RegisterValueChangedCallback( evt => {
            m_bodyUIContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        });
        
        VisualElement buttonList = bodyUIRoot.Q<VisualElement>("ButtonsContainer");
        int numBodyStates = m_bodyStateNames.Count;
        for (int i=0;i<numBodyStates;++i) {
            string stateName = m_bodyStateNames[i];
            Button btn = new Button {
                text = stateName, 
                clickable = null //avoid buttons from NOT propagating PointerDownEvent and PointerUpEvent
            };
        
            int stateHash = Animator.StringToHash("Base Layer." + stateName);
            btn.RegisterCallback<PointerDownEvent>( evt => {
                m_animator.Play(stateName);
            });
            
            buttonList.Add(btn);
            
            m_bodyAnimationButtonUISelector.AddButton(btn, stateHash,i);
        }

        //random
        Toggle keepFaceToggle = bodyUIRoot.Q<Toggle>("RandomBodyToggle");
        keepFaceToggle.SetValueWithoutNotify(m_randomAnimation);
        keepFaceToggle.RegisterValueChangedCallback(evt => { m_randomAnimation = evt.newValue; });
    }

    void Update() {
        
        if (m_inputReader.ReadButtonDown((int) SDUnityChanButtonInput.Next)) {
            m_animator.SetBool(UnityChanConstants.ANIMATOR_NEXT_PARAM, true);
        }

        if (m_inputReader.ReadButtonDown((int) SDUnityChanButtonInput.Prev)) {
            m_animator.SetBool(UnityChanConstants.ANIMATOR_PREV_PARAM, true);
        }

        if (m_inputReader.ReadButtonDown((int) SDUnityChanButtonInput.Reset)) {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        
        if (m_inputReader.ReadButtonDown((int) SDUnityChanButtonInput.Jump)) {
            m_animator.Play(m_curAnimatorStateHash, BODY_LAYER_INDEX, 0f);
        }
        
        UpdateAnimationState();
    }

    void UpdateAnimationState() {
        
        AnimatorStateInfo currentState = m_animator.GetCurrentAnimatorStateInfo(BODY_LAYER_INDEX);
        int currentStateHash = currentState.fullPathHash;
            
        if (m_curAnimatorStateHash == currentState.fullPathHash) 
            return;

        m_curAnimatorStateHash = currentStateHash;
        m_bodyAnimationButtonUISelector.Select(m_curAnimatorStateHash);
        
        //reset triggers
        m_animator.SetBool(UnityChanConstants.ANIMATOR_NEXT_PARAM, false);
        m_animator.SetBool(UnityChanConstants.ANIMATOR_PREV_PARAM, false);
    }

 
//----------------------------------------------------------------------------------------------------------------------

    IEnumerator RandomChange() {
        while (true) {
            if (m_randomAnimation) {
                int randomAnimationIndex = Random.Range(0, m_bodyStateNames.Count); // returns 0 or 1
                m_animator.Play(m_bodyStateNames[randomAnimationIndex]);
            }

            yield return new WaitForSeconds(m_randomInterval);
        }
    }

    private void OnValidate() {
        m_animator = GetComponent<Animator>();
        
#if UNITY_EDITOR
        m_bodyStateNames = AnimationEditorUtility.FindStateNames(m_animator,BODY_LAYER_INDEX);
#endif        
        
    }

//----------------------------------------------------------------------------------------------------------------------

    [FormerlySerializedAs("_random")] public bool m_randomAnimation = false; 
    [FormerlySerializedAs("_interval")] public float m_randomInterval = 3; 
    
    [SerializeField] private UIDocument m_rootUI;
    [SerializeField] private StyledUITemplate m_bodyUITemplate;

    [SerializeField] private SDUnityChanInputReader m_inputReader;
    
    [HideInInspector][SerializeField] private Animator m_animator; 
    [HideInInspector][SerializeField] private List<string> m_bodyStateNames;
//----------------------------------------------------------------------------------------------------------------------
    private int m_curAnimatorStateHash; // full path hash
    
    VisualElement m_bodyUIContainer;
    private readonly AnimationButtonUISelector m_bodyAnimationButtonUISelector = new AnimationButtonUISelector();
    
//----------------------------------------------------------------------------------------------------------------------
    
    const int BODY_LAYER_INDEX = 0;
    
}
}