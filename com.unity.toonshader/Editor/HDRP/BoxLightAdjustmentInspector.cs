using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if HDRP_IS_INSTALLED_FOR_UTS
using Unity.Rendering.HighDefinition.Toon;
using UnityEngine.Rendering.HighDefinition;


namespace UnityEditor.Rendering.HighDefinition.Toon
{
    [CustomEditor(typeof(BoxLightAdjustment))]

    public class BoxLightAdjustmentInspector : Editor
    {
        SerializedObject m_SerializedObject;

        public override void OnInspectorGUI()
        {
            const string labelBoxLight = "Box Light";
            const string labelFollowPostion = "Follow Position";
            const string labelFollowRotation = "Follow Rotation";
            const string labelLightLeyer = "Light Layer";

            bool isChanged = false;

            var obj = target as BoxLightAdjustment;

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField(labelBoxLight);
            Light targetLight = EditorGUILayout.ObjectField(obj.m_targetBoxLight, typeof(Light), true) as Light;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed the target box lihgt");
                obj.m_targetBoxLight = targetLight;
                isChanged = true;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(targetLight == null);
            {
                EditorGUI.indentLevel++;
                var layer = targetLight.gameObject.layer;
                List<string> layerNames = new List<string>();
                List<int> layerIndecies = new List<int>();

                for (int ii = 0; ii < 32; ii++)
                {
                    string layerName = LayerMask.LayerToName(ii);
                    if (!string.IsNullOrEmpty(layerName))
                    {
                        layerNames.Add(layerName);
                        layerIndecies.Add(ii);
                    }
                }
                

                EditorGUI.BeginChangeCheck();

                bool followPosition = EditorGUILayout.Toggle(labelFollowPostion, obj.m_followGameObjectPosition);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed Light Hi Cut Filter");
                    obj.m_followGameObjectPosition = followPosition;
                    isChanged = true;
                }

                EditorGUI.BeginChangeCheck();
                bool followRotation = EditorGUILayout.Toggle(labelFollowRotation, obj.m_followGameObjectRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed Expsure Adjustment");
                    obj.m_followGameObjectRotation = followRotation;
                    isChanged = true;
                }
                EditorGUI.indentLevel--;
            }
            if (isChanged)
            {
                // at leaset 2020.3.12f1, not neccessary. but, from which version??
                EditorApplication.QueuePlayerLoopUpdate();
            }

        }


        float ConvertFromEV100(float EV100)
        {

            float val = Mathf.Pow(2, EV100) * 2.5f;
            return val;

        }

        float ConvertToEV100(float val)
        {

            return Mathf.Log(val * 0.4f, 2.0f);

        }





        [MenuItem("GameObject/Toon Shader/Create Box Light", false, 9999)]
        static void CreateBoxLight()
        {

            var go = Selection.activeGameObject;
            if (go != null)
            {
                GameObject lightGameObject = new GameObject("Box Light for" + go.name);
                Light lightComp = lightGameObject.AddComponent<Light>();
                lightComp.type = LightType.Spot;

                var boxLightAdjustment = go.AddComponent<BoxLightAdjustment>();
                boxLightAdjustment.m_targetBoxLight = lightComp;
            }
            else
            {
                Debug.LogError("Please, select a GameObject you want a Box Light to follow.");
            }
        }

    }
}
#endif