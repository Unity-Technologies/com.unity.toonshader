using NUnit.Framework;
using Unity.Rendering.Toon;
using System.Collections;
using Unity.FilmInternalUtilities.Editor;
using UnityEngine.TestTools;

namespace Unity.ToonShader.EditorTests {

internal class UTSHelpURLAttributeTests {
    
    [UnityTest]
    public IEnumerator VerifyVersion() {
        
        //check package version
        bool done = false;
        string installedPackageVersionStr = null;
        PackageUtility.FindInstalledPackageVersion(
            ToonConstants.PACKAGE_NAME,
            onVersionFound: (version) => {
                installedPackageVersionStr = version;
                done = true;
            },
            onVersionNotFound: () => {
                done = true;
            }
        );
        while (!done) {
            yield return null;
        }
        Assert.IsFalse(string.IsNullOrEmpty(installedPackageVersionStr));

        bool parseResult = PackageVersion.TryParse(installedPackageVersionStr, out PackageVersion packageVersion);
        Assert.IsTrue(parseResult, $"Failed to parse package version string: {installedPackageVersionStr}");
        
        string installedPackageVersionMajorMinor = packageVersion.GetMajor() + "." + packageVersion.GetMinor();
        
        Assert.IsTrue(ToonConstants.PACKAGE_VERSION_MAJOR_MINOR == installedPackageVersionMajorMinor, 
            $"Incorrect package version in {nameof(UTSHelpURLAttribute)}. " +
            $"Expected: " + packageVersion + " Actual: " + ToonConstants.PACKAGE_VERSION_MAJOR_MINOR);

    }

}
} //end namespace


