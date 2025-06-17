using UnityEngine;

namespace Unity.U2D.Animation.Sample.Dependency
{
    [ExecuteInEditMode]
    internal class AnimationSampleDependency : MonoBehaviour
    {
        enum Dependency
        {
            None,
            PsdImporter,
            AssetBundle
        }

        [SerializeField]
        GameObject errorUI = null;
        [SerializeField]
        Dependency dependency = Dependency.None;

        void Update()
        {
            var hasDependencyInstalled = HasDependencyInstalled();
            if (errorUI != null)
                errorUI.SetActive(!hasDependencyInstalled);
        }

        bool HasDependencyInstalled()
        {
            switch (dependency)
            {
                case Dependency.PsdImporter:
#if PSDIMPORTER_ENABLED
                    return true;
#else
                    return false;
#endif
                case Dependency.AssetBundle:
#if ASSETBUNDLE_ENABLED
                    return true;
#else
                    return false;
#endif
            }

            return true;
        }
    }
}
