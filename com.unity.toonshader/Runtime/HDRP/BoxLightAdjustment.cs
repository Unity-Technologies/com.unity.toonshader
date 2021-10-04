using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityObject = UnityEngine.Object;
using System.Linq;

#if HDRP_IS_INSTALLED_FOR_UTS

namespace Unity.Rendering.HighDefinition.Toon
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class BoxLightAdjustment : MonoBehaviour
    {


        // flags
        bool m_initialized = false;
        bool m_srpCallbackInitialized = false;

        [SerializeField]
        GameObject[] m_Objs;



        [SerializeField]
        internal Light m_targetBoxLight;

        [SerializeField]
        internal bool  m_followGameObjectPosition = true;

        [SerializeField]
        internal bool  m_followGameObjectRotation;

#if UNITY_EDITOR
#pragma warning restore CS0414
        bool m_isCompiling = false;
#endif

        void Reset()
        {
            OnDisable();
            OnEnable();

        }

        void OnValidate()
        {
            Release();
            Initialize();
        }

        private void Awake()
        {
            Initialize();

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {


            Initialize();



#if UNITY_EDITOR
            // handle script recompile
            if (EditorApplication.isCompiling && !m_isCompiling)
            {
                // on compile begin
                m_isCompiling = true;
                //                Release(); no need
                return; // 
            }
            else if (!EditorApplication.isCompiling && m_isCompiling)
            {
                // on compile end
                m_isCompiling = false;
            }
#endif

        }
        void EnableSrpCallbacks()
        {

            if (!m_srpCallbackInitialized)
            {
                m_srpCallbackInitialized = true;
            }
        }
        void DisableSrpCallbacks()
        {
            if (m_srpCallbackInitialized)
            {
                m_srpCallbackInitialized = false;
            }
        }

        void OnEnable()
        {

            Initialize();

            EnableSrpCallbacks();

        }

        void OnDisable()
        {
            DisableSrpCallbacks();

            Release();
        }

        void Initialize()
        {
            if (m_initialized)
            {
                return;
            }
#if UNITY_EDITOR
            // initializing renderer can interfere GI baking. so wait until it is completed.

            if (EditorApplication.isCompiling)
                return;
#endif
            // must be put to gameObject model chain.
            if (m_Objs == null || m_Objs.Length == 0)
            {
                m_Objs = new GameObject[1];
                m_Objs[0] = this.gameObject;
            }
            int objCount = m_Objs.Length;
            int rendererCount = 0;

            List<Renderer> rendererList = new List<Renderer>();
            for (int ii = 0; ii < objCount; ii++)
            {
                if (m_Objs[ii] == null )
                {
                    continue;
                }


                var renderer = m_Objs[ii].GetComponent<Renderer>();
                if (renderer != null)
                {
                    rendererCount++;
                    rendererList.Add(renderer);
                }
                GameObject[] childGameObjects = m_Objs[ii].GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToArray();
                int childCount = childGameObjects.Length;
                for (int jj = 0; jj < childCount; jj++)
                {
                    if (m_Objs[ii] == childGameObjects[jj])
                        continue;
                    var modelToonEvAdjustment = childGameObjects[jj].GetComponent<BoxLightAdjustment>();
                    if ( modelToonEvAdjustment != null )
                    {

                        break;
                    }
                    renderer = childGameObjects[jj].GetComponent<Renderer>();
                    if ( renderer != null )
                    {
                        rendererList.Add(renderer);
                        rendererCount++;
                    }
                }
            }

            m_initialized = true;
        }


        void Release()
        {
            if (m_initialized)
            {


            }

            m_initialized = false;

        }

    }
}
#endif  // HDRP_IS_INSTALLED_FOR_UTS