using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChan {

[RequireComponent(typeof(Animator))]  
public class IKLookAt : MonoBehaviour {

    void Start() {
        
        InitUI();
        UpdateTargetVisibility();
    }

    void InitUI() {
        if (null == m_rootUI)
            return;
        
        VisualElement root = m_rootUI.rootVisualElement;
        
        VisualElement block = root.Q<VisualElement>("BottomRightBlock");
        m_lookAtUIRoot = m_lookAtUITemplate.Instantiate(block);
        
        m_lookAtToggle = m_lookAtUIRoot.Q<Toggle>(UnityChanConstants.TOGGLE_BLOCK_UI_ELEMENT_NAME);
        m_lookAtToggle.text = "Look at IK";
        m_lookAtToggle.RegisterValueChangedCallback(evt => {
            ikActive = evt.newValue;
            UpdateTargetVisibility();
        });
        
        m_lookAtUIRoot.style.display = m_showUI ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void UpdateTargetVisibility() {
        if (m_target) {
            m_target.enabled = ikActive;
        }
    }

    void OnAnimatorIK(int layerIndex) {
        if (!m_animator) 
            return;
        
        if (ikActive) {
            m_animator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
            if (m_target != null) {
                m_animator.SetLookAtPosition(m_target.transform.position);
            } else {
                m_animator.SetLookAtWeight(0.0f);
            }
        } else {
            m_animator.SetLookAtWeight(0.0f);
        }
    }

    void OnValidate() {
        m_animator = GetComponent<Animator>();
    }

//----------------------------------------------------------------------------------------------------------------------    

    public bool ikActive = false;
    [SerializeField] private Renderer m_target = null;

    public float lookAtWeight = 1.0f;
    public float bodyWeight = 0.3f;
    public float headWeight = 0.8f;
    public float eyesWeight = 1.0f;
    public float clampWeight = 0.5f;

    [SerializeField] private bool m_showUI = true;

    [SerializeField] private UIDocument m_rootUI;
    [SerializeField] private StyledUITemplate m_lookAtUITemplate;

    [SerializeField] private Animator m_animator;

//----------------------------------------------------------------------------------------------------------------------

    VisualElement m_lookAtUIRoot;
    Toggle m_lookAtToggle;
    
    
}
}