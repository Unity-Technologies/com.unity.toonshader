using UnityEditor;
using UnityEngine;

namespace UnityChan.Editor {
[CustomEditor(typeof(SpringManager))]
public class SpringManagerInspector : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SpringManager manager = (SpringManager)target;

        if (GUILayout.Button("Init From Children")) {
            SpringBone[] springBones = manager.GetComponentsInChildren<SpringBone>(true);
            SerializedProperty springBonesProp = serializedObject.FindProperty(nameof(SpringManager.m_springBones));

            springBonesProp.arraySize = springBones.Length;
            for (int i = 0; i < springBones.Length; i++) {
                springBonesProp.GetArrayElementAtIndex(i).objectReferenceValue = springBones[i];
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

}