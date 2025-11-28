using NUnit.Framework;
using Unity.Rendering.Toon;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine;

namespace Unity.ToonShader.Tests {

internal class ToonEnumUtilityTests {

    internal enum DummyEnum {
        [InspectorName(FIRST_VALUE)] First,
        Second
    }

    [Test]
    public void ToInspectorNamesAsGUIContentTest() {
        GUIContent[] contents = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(DummyEnum));
        Assert.AreEqual(2, contents.Length);
        Assert.AreEqual(FIRST_VALUE, contents[0].text);
        Assert.AreEqual("Second", contents[1].text);
    }

    [Test]
    public void ToIndicesTest() {
        int[] indices = ToonEnumUtility.ToIndices(typeof(DummyEnum));
        Assert.AreEqual(2, indices.Length);
        Assert.AreEqual(0, indices[0]);
        Assert.AreEqual(1, indices[1]);
    }

    const string FIRST_VALUE = "First Value";
}

} //end namespace


