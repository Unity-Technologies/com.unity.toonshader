using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.U2D.Animation.Sample
{
    internal class PressPlayUI : MonoBehaviour
    {
        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.enabled = false;
        }
    }
}
