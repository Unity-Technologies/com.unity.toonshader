using UnityEditor;
using UnityEngine;

namespace UnityChan {
[CustomEditor(typeof(SpringManager))]
public class SpringManagerInspector : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SpringManager manager = (SpringManager)target;

        if (GUILayout.Button("Init From Children")) {
            SpringBone[] springBones = manager.GetComponentsInChildren<SpringBone>(true);
            SerializedProperty springBonesProp = serializedObject.FindProperty(nameof(SpringManager.springBones));

            springBonesProp.arraySize = springBones.Length;
            for (int i = 0; i < springBones.Length; i++) {
                springBonesProp.GetArrayElementAtIndex(i).objectReferenceValue = springBones[i];
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

}