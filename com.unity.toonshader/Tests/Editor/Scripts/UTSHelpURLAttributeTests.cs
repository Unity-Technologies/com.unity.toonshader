using NUnit.Framework;
using Unity.Rendering.Toon;
using System.Collections;
using UnityEngine.TestTools;

namespace Unity.ToonShader.EditorTests {

internal class UTSHelpURLAttributeTests {
    
    [UnityTest]
    public IEnumerator VerifyVersion() {
        
        //check package version
        bool done = false;
        string packageVersion = null;
        FilmInternalUtilities.Editor.PackageUtility.FindInstalledPackageVersion(
            ToonConstants.PACKAGE_NAME,
            onVersionFound: (version) => {
                packageVersion = version;
                done = true;
            },
            onVersionNotFound: () => {
                done = true;
            }
        );
        while (!done) {
            yield return null;
        }
        Assert.IsFalse(string.IsNullOrEmpty(packageVersion));
        
        Assert.IsTrue(UTSHelpURLAttribute.version == packageVersion, 
            $"Incorrect package version in {nameof(UTSHelpURLAttribute)}. " +
            $"Expected: " + packageVersion + " Actual: " + UTSHelpURLAttribute.version);

    }

}
} //end namespace


