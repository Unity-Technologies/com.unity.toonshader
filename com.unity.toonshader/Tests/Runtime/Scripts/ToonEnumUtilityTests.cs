using NUnit.Framework;
using Unity.Rendering.Toon;
using System.Collections;
//using Unity.FilmInternalUtilities.Editor;
using UnityEngine.TestTools;
using UnityEngine;

namespace Unity.ToonShader.Tests {

internal class ToonEnumUtilityTests {

    internal enum DummyEnum {
        [InspectorName("First Value")] First,
        Second
    }

    [Test]
    internal void ToInspectorNamesAsGUIContent_ReturnsCorrectNames() {
        var contents = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(DummyEnum));
        Assert.AreEqual(2, contents.Length);
        Assert.AreEqual("First Value", contents[0].text);
        Assert.AreEqual("Second", contents[1].text);
    }

    [Test]
    internal void ToIndices_ReturnsCorrectIndices() {
        var indices = ToonEnumUtility.ToIndices(typeof(DummyEnum));
        Assert.AreEqual(2, indices.Length);
        Assert.AreEqual(0, indices[0]);
        Assert.AreEqual(1, indices[1]);
    }

}

} //end namespace


