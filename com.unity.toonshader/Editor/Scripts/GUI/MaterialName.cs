using UnityEngine;

namespace UnityEditor.Rendering.Toon {

internal class MaterialName {
    internal readonly string name;
    internal readonly int nameID;

    internal MaterialName(string s) {
        name = s;
        nameID = Shader.PropertyToID(s);
    }
}
}

