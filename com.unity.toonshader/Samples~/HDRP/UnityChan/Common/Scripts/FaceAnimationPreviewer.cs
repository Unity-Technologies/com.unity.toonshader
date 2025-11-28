using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UnityChan {

[ExecuteAlways]
[RequireComponent(typeof(Animator))]
public class FaceAnimationPreviewer : MonoBehaviour {
    
    void Start() {
        m_faceStateNameSet = new HashSet<string>(m_faceStateNames);
        
        InitUI();
    }

    void InitUI() {

        if (null == m_rootUI)
            return;

        VisualElement root = m_rootUI.rootVisualElement;
        
        VisualElement block = root.Q<VisualElement>("TopLeftBlock");
        VisualElement faceUIRoot = m_faceUITemplate.Instantiate(block);
        
        m_faceUIContainer = faceUIRoot.Q<VisualElement>("FaceUIContainer");

        Toggle faceUIToggle = faceUIRoot.Q<Toggle>("FaceUIToggle");
        faceUIToggle.SetValueWithoutNotify(true);
        faceUIToggle.RegisterValueChangedCallback( evt => {
            m_faceUIContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        });
        
        
        VisualElement buttonList = faceUIRoot.Q<VisualElement>("ButtonsContainer");
        
        int numBodyStates = m_faceStateNames.Count;
        for (int i=0;i<numBodyStates;++i) {
            string stateName = m_faceStateNames[i];
            Button btn = new Button {
                text = stateName.Split('@')[0], //state name contains suffix with '@'
                clickable = null //avoid buttons from NOT propagating PointerDownEvent and PointerUpEvent
            };
        
            btn.RegisterCallback<PointerDownEvent>( evt => {
                m_shouldResetFaceLayerWeight = true;
                m_prevLockFace = m_lockFace;
                m_lockFace = true;
                m_animator.Play(stateName,FACE_LAYER_INDEX);
                evt.target.CapturePointer(evt.pointerId); //ensures all pointer events are sent to the target
                
            });
        
            btn.RegisterCallback<PointerUpEvent>( evt => {
                if (!evt.target.HasPointerCapture(evt.pointerId)) 
                    return;
                
                evt.target.ReleasePointer(evt.pointerId);
                m_shouldResetFaceLayerWeight = false;
                m_lockFace = m_prevLockFace;
                
            });
            
            buttonList.Add(btn);
            int stateHash = Animator.StringToHash("face." + stateName);
            m_faceAnimationButtonUISelector.AddButton(btn, stateHash,i);
        }
        
        Toggle keepFaceToggle = faceUIRoot.Q<Toggle>("LockFaceToggle");
        keepFaceToggle.SetValueWithoutNotify(m_lockFace);
        keepFaceToggle.RegisterValueChangedCallback(evt => { m_lockFace = evt.newValue; });
    }

    void Update() {
        if (m_shouldResetFaceLayerWeight) {
            m_curFaceLayerWeight = 1;
        } else if (!m_lockFace) {
            m_curFaceLayerWeight = Mathf.Lerp(m_curFaceLayerWeight, 0, m_delayWeight * Time.deltaTime);
        }

        m_animator.SetLayerWeight(FACE_LAYER_INDEX, m_curFaceLayerWeight);
        
        UpdateAnimationState();
    }
    
    void UpdateAnimationState() {
        
        AnimatorStateInfo currentState = m_animator.GetCurrentAnimatorStateInfo(FACE_LAYER_INDEX);
        int currentStateHash = currentState.fullPathHash;
            
        if (m_curFaceStateHash == currentState.fullPathHash) 
            return;

        m_curFaceStateHash = currentStateHash;
        m_faceAnimationButtonUISelector.Select(m_curFaceStateHash);
    }
    

//----------------------------------------------------------------------------------------------------------------------
    
    //Called by Events set in the AnimationClip asset
    private void OnCallChangeFace(string str) {
        str = str.Split('@')[0]; //the previous state names contain suffix with '@'
        Assert.IsNotNull(m_faceStateNameSet);
        if (m_faceStateNameSet.Contains(str)) {
            TryOverrideFaceAnimation(str);
        } else {
            //fallback to default face animation 
            Assert.IsTrue(m_faceStateNames.Count > 0, "No face animation states found in the animator controller.");
            TryOverrideFaceAnimation(m_faceStateNames[m_defaultFaceAnimationIndex]);
            
        }
    }

    void TryOverrideFaceAnimation(string str) {
        if (m_lockFace)
            return;
        
        m_shouldResetFaceLayerWeight = true;
        m_animator.Play(str,FACE_LAYER_INDEX);
    }
//----------------------------------------------------------------------------------------------------------------------

    private void OnValidate() {
        m_animator = GetComponent<Animator>();
        
#if UNITY_EDITOR
        m_faceStateNames = AnimationEditorUtility.FindStateNames(m_animator,FACE_LAYER_INDEX);
        m_faceStateNames.Sort();
        m_defaultFaceAnimationIndex = m_faceStateNames.FindIndex(stateName => stateName.Contains("default"));
#endif
    }
    

//----------------------------------------------------------------------------------------------------------------------
    
    [FormerlySerializedAs("delayWeight")] [SerializeField] private float m_delayWeight;
    [SerializeField] private bool m_lockFace = false;

    [SerializeField] private UIDocument m_rootUI;
    [SerializeField] private StyledUITemplate m_faceUITemplate;
    
    [HideInInspector][SerializeField] private Animator m_animator;
    [HideInInspector][SerializeField] private List<string> m_faceStateNames;
    
    [HideInInspector][SerializeField] private int m_defaultFaceAnimationIndex = 0;    
    
//----------------------------------------------------------------------------------------------------------------------

    HashSet<string> m_faceStateNameSet = null;

    float m_curFaceLayerWeight = 0;
    bool m_shouldResetFaceLayerWeight = false;
    VisualElement m_faceUIContainer;
    private readonly AnimationButtonUISelector m_faceAnimationButtonUISelector = new AnimationButtonUISelector();
    private int m_curFaceStateHash; // full path hash

    
    bool m_prevLockFace = false;
    
    
    const int FACE_LAYER_INDEX = 1;

}
} //end namespace UnityChan