using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChan {

[CreateAssetMenu(fileName = "StyledUITemplate", menuName = "UnityChan/StyledUITemplate", order = 1)]
[Serializable]
public class StyledUITemplate : ScriptableObject{

    public VisualElement Instantiate(VisualElement parent) {
        VisualElement instance = m_uiTemplate.Instantiate();
        parent.styleSheets.Add(m_uiStyle);
        parent.Add(instance);
        return instance;
    }
    
//----------------------------------------------------------------------------------------------------------------------  
    
    [SerializeField] private VisualTreeAsset m_uiTemplate;
    [SerializeField] private StyleSheet m_uiStyle;
    
}

}
