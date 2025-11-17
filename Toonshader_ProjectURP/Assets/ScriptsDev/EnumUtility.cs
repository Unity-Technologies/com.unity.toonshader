
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal static class EnumUtility {
    internal static List<GUIContent> ToInspectorNames(Type t) {
        List<GUIContent> ret = new List<GUIContent>();
        foreach (MemberInfo mi in t.GetMembers( BindingFlags.Static | BindingFlags.Public)) {
            InspectorNameAttribute inspectorNameAttribute = (InspectorNameAttribute) Attribute.GetCustomAttribute(mi, typeof(InspectorNameAttribute));
            if (null == inspectorNameAttribute) {
                ret.Add(new GUIContent(mi.Name));
                continue;
            }

            ret.Add(new GUIContent(inspectorNameAttribute.displayName));			
        }

        return ret;
    }
}
