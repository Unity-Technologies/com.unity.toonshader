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

    // Declared out of numeric order to verify results are sorted by value, not declaration order.
    internal enum NonSequentialEnum {
        High = 20,
        Low = 5,
        Mid = 10,
    }

    [Test]
    public void ToInspectorNamesAsGUIContentTest() {
        GUIContent[] contents = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(DummyEnum));
        Assert.AreEqual(2, contents.Length);
        Assert.AreEqual(FIRST_VALUE, contents[0].text);
        Assert.AreEqual("Second", contents[1].text);
    }

    [Test]
    public void ToIntValuesTest() {
        int[] values = ToonEnumUtility.ToIntValues(typeof(DummyEnum));
        Assert.AreEqual(2, values.Length);
        Assert.AreEqual(0, values[0]);
        Assert.AreEqual(1, values[1]);
    }

    [Test]
    public void NonSequentialEnumNamesAndValuesAreAlignedAndSortedTest() {
        GUIContent[] contents = ToonEnumUtility.ToInspectorNamesAsGUIContent(typeof(NonSequentialEnum));
        int[] values = ToonEnumUtility.ToIntValues(typeof(NonSequentialEnum));

        Assert.AreEqual(3, contents.Length);
        Assert.AreEqual(3, values.Length);

        Assert.AreEqual("Low", contents[0].text);
        Assert.AreEqual("Mid", contents[1].text);
        Assert.AreEqual("High", contents[2].text);

        Assert.AreEqual(5, values[0]);
        Assert.AreEqual(10, values[1]);
        Assert.AreEqual(20, values[2]);
    }

    const string FIRST_VALUE = "First Value";
}

} //end namespace


