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
        
        Assert.IsTrue(ToonConstants.PACKAGE_VERSION_MAJOR_MINOR == packageVersion, 
            $"Incorrect package version in {nameof(UTSHelpURLAttribute)}. " +
            $"Expected: " + packageVersion + " Actual: " + ToonConstants.PACKAGE_VERSION_MAJOR_MINOR);

    }

}
} //end namespace


