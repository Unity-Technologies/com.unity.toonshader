// Original script by ricopin / SpringManager.cs
// https://twitter.com/ricopin416

using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityChan {
public class SpringManager : MonoBehaviour {

    void Start() {
        UpdateParameters();
    }

    void Update() {
#if UNITY_EDITOR
        if (m_dynamicRatio >= 1.0f)
            m_dynamicRatio = 1.0f;
        else if (m_dynamicRatio <= 0.0f)
            m_dynamicRatio = 0.0f;
        
        UpdateParameters();
#endif
    }

    private void LateUpdate() {
        if (m_dynamicRatio != 0.0f) {
            for (int i = 0; i < m_springBones.Length; i++) {
                if (m_dynamicRatio > m_springBones[i].threshold) {
                    m_springBones[i].UpdateSpring();
                }
            }
        }
    }

    private void UpdateParameters() {
        if (m_springBones.Length <= 1) {
            Debug.LogWarning("[UnityChan] SpringManager needs at least 2 SpringBones to apply scale.");
            return;
        } 
        
        int numSpringBonesMinusOne = m_springBones.Length - 1;
        for (int i = 0; i < m_springBones.Length; ++i) {
            SpringBone springBone = m_springBones[i];
            if (springBone.isUseEachBoneForceSettings) 
                continue;
            
            springBone.dragForce = m_baseDragForce * EvaluateScale(m_dragScaleCurve,i,numSpringBonesMinusOne);
            springBone.stiffnessForce = m_baseStiffnessForce * EvaluateScale(m_stiffnessScaleCurve,i, numSpringBonesMinusOne);
        }
    }

    private static float EvaluateScale(AnimationCurve curve, int index, int numSpringBonesMinusOne) {
        float start = curve.keys[0].time;
        float end = curve.keys[curve.length - 1].time;
        //var step	= (end - start) / (springBones.Length - 1);

        float t = start + (end - start) * index / (numSpringBonesMinusOne);

        return curve.Evaluate(t);
    }
    
    

//----------------------------------------------------------------------------------------------------------------------
    public float GetDynamicRatio() => m_dynamicRatio;
    public void SetDynamicRatio(float value) => m_dynamicRatio = value;

    public float GetBaseStiffnessForce() => m_baseStiffnessForce;
    public void SetBaseStiffnessForce(float value) => m_baseStiffnessForce = value;

    public AnimationCurve GetStiffnessScaleCurve() => m_stiffnessScaleCurve;
    public void SetStiffnessScaleCurve(AnimationCurve value) => m_stiffnessScaleCurve = value;

    public float GetBaseDragForce() => m_baseDragForce;
    public void SetBaseDragForce(float value) => m_baseDragForce = value;

    public AnimationCurve GetDragScaleCurve() => m_dragScaleCurve;
    public void SetDragScaleCurve(AnimationCurve value) => m_dragScaleCurve = value;

    public SpringBone[] GetSpringBones() => m_springBones;
    public void SetSpringBones(SpringBone[] value) => m_springBones = value;


//----------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    public string SpringBonesFieldName() => nameof(SpringManager.m_springBones);
#endif    
    
//----------------------------------------------------------------------------------------------------------------------
    // DynamicRatio is parameter for activated level of dynamic animation 
    [FormerlySerializedAs("dynamicRatio")] [SerializeField] private float m_dynamicRatio = 1.0f;

    [FormerlySerializedAs("stiffnessForce")] [SerializeField] private float m_baseStiffnessForce = 0.01f;
    [FormerlySerializedAs("stiffnessCurve")] [SerializeField] private AnimationCurve m_stiffnessScaleCurve 
        = AnimationCurve.Constant(0,1,1);
    [FormerlySerializedAs("dragForce")] [SerializeField] private float m_baseDragForce = 0.4f;
    [FormerlySerializedAs("dragCurve")] [SerializeField] private AnimationCurve m_dragScaleCurve
        = AnimationCurve.Constant(0,1,1);
    [FormerlySerializedAs("springBones")] [SerializeField] internal SpringBone[] m_springBones;
    
}
}