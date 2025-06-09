using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Custom Resolutions", menuName = "Graphics Test Framework/Custom Resolutions", order = 100)]
public class CustomResolutionSettings : ScriptableObject
{
    public CustomResolutionFields[] fields;
}
