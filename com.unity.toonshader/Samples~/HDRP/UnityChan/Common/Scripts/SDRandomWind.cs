// Original script by ricopin / RandomWind.cs
// https://twitter.com/ricopin416

using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UnityChan {
public class SDRandomWind : MonoBehaviour {
    
    void Start() {
        springBones = GetComponent<SpringManager>().springBones;
        StartCoroutine("RandomChange");
        
        InitUI();
        m_isGUIShown = m_showUI;
    }

    void InitUI() {
        if (null == m_rootUI)
            return;
        
        VisualElement root = m_rootUI.rootVisualElement;
        
        VisualElement block = root.Q<VisualElement>("BottomLeftBlock");
        m_windUIRoot = m_windUITemplate.Instantiate(block);
        
        m_windToggle = m_windUIRoot.Q<Toggle>(UnityChanConstants.TOGGLE_BLOCK_UI_ELEMENT_NAME);
        m_windToggle.text = "Wind";
        m_windToggle.RegisterValueChangedCallback(evt => { isWindActive = evt.newValue; });
        
        
        m_windUIRoot.style.display = m_showUI ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    void Update() {
        if (isWindActive) {
            Vector3 force = Vector3.zero;
            force.x = Mathf.PerlinNoise(Time.time, 0.0f) * windPower * 0.001f;
            force.y = gravity * -0.001f;
            if (m_applyMinusForce) {
                force.x *= -1;
            } 

            UpdateSpringBonesForce(force);
        } else {
            UpdateSpringBonesForce(Vector3.zero);
        }

        if (m_isGUIShown != m_showUI) {
            m_windUIRoot.style.display = m_showUI ? DisplayStyle.Flex : DisplayStyle.None;
            m_isGUIShown = m_showUI;
        }
    }

    void UpdateSpringBonesForce(Vector3 force) {
        for (int i = 0; i < springBones.Length; i++) {
            springBones[i].springForce = force;
        }
    }
    
    void OnGUI() {
        // if (isGUI) {
        //     Rect rect1 = new Rect(10, Screen.height - 40, 400, 30);
        //     isWindActive = GUI.Toggle(rect1, isWindActive, "Random Wind");
        // }
    }

    // ランダム判定用関数.
    IEnumerator RandomChange() {
        // 無限ループ開始.
        while (true) {
            //ランダム判定用シード発生.
            float _seed = Random.Range(0.0f, 1.0f);

            if (_seed > threshold) {
                //_seedがthreshold以上の時、符号を反転する.
                m_applyMinusForce = true;
            } else {
                m_applyMinusForce = false;
            }

            // 次の判定までインターバルを置く.
            yield return new WaitForSeconds(interval);
        }
    }

    void OnValidate() {
        springBones = GetComponent<SpringManager>().springBones;
    }
 

    public bool isWindActive = false;
    public float threshold = 0.5f; // ランダム判定の閾値.
    public float interval = 5.0f; // ランダム判定のインターバル.
    public float windPower = 1.0f; //風の強さ.
    public float gravity = 0.98f; //重力の強さ.
    
    [SerializeField] private bool m_showUI = true;
    
    
    [SerializeField] private UIDocument m_rootUI;
    [SerializeField] private StyledUITemplate m_windUITemplate;

//----------------------------------------------------------------------------------------------------------------------    
    private SpringBone[] springBones;

    private bool m_applyMinusForce = false; 
    private bool m_isGUIShown = false;
    
    VisualElement m_windUIRoot;
    Toggle m_windToggle;
    

}
}