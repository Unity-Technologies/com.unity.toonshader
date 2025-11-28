
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unity.Rendering.Toon {

internal static class ToonEnumUtility {
    internal static GUIContent[] ToInspectorNamesAsGUIContent(Type t) {
        MemberInfo[] members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);

        int numMembers = members.Length;
        GUIContent[] ret = new GUIContent[numMembers];
        for (int i = 0; i < numMembers; i++) {
            InspectorNameAttribute inspectorNameAttribute = (InspectorNameAttribute)Attribute.GetCustomAttribute(
                members[i], typeof(InspectorNameAttribute));
            if (inspectorNameAttribute == null) {
                ret[i] = new GUIContent(members[i].Name);
            } else {
                ret[i] = new GUIContent(inspectorNameAttribute.displayName);
            }
        }

        return ret;
    }

    internal static int[] ToIndices(Type t) {

        MemberInfo[] members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);
        int numMembers = members.Length;
        int[] indices = new int[numMembers];
        for (int i = 0; i < numMembers; ++i) {
            indices[i] = i;
        }

        return indices;

    }

}

}