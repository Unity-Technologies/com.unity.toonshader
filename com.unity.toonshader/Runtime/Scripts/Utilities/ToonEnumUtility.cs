
using System;
using System.Reflection;
using UnityEngine;

namespace Unity.Rendering.Toon {

internal static class ToonEnumUtility {
    internal static GUIContent[] ToInspectorNamesAsGUIContent(Type t) {
        string[] names = Enum.GetNames(t);
        GUIContent[] ret = new GUIContent[names.Length];
        for (int i = 0; i < names.Length; i++) {
            FieldInfo field = t.GetField(names[i], BindingFlags.Public | BindingFlags.Static);
            InspectorNameAttribute attr = field != null
                ? (InspectorNameAttribute)Attribute.GetCustomAttribute(field, typeof(InspectorNameAttribute))
                : null;
            ret[i] = new GUIContent(attr != null ? attr.displayName : names[i]);
        }
        return ret;
    }

    internal static int[] ToIndices(Type t) {
        Array values = Enum.GetValues(t);
        int numValues = values.Length;
        int[] indices = new int[numValues];
        for (int i = 0; i < numValues; i++)
            indices[i] = (int)values.GetValue(i);
        return indices;
    }

}

}