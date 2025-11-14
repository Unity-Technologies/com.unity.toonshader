using UnityEngine;

namespace Unity.Rendering.Toon
{
    internal class UTSHelpURLAttribute : HelpURLAttribute
    {
        //[TODO-sin: 2025-10-20] Return the actual version
        private const string fallbackVersion = "0.7";

        private static string version
        {
            get
            {
                return fallbackVersion;
            }
        }
        const string url = "https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html";

        internal UTSHelpURLAttribute(string pageName, string packageName = "com.unity.toonshader")
            : base(GetPageLink(packageName, pageName))
        {
        }

        internal static string GetPageLink(string packageName, string pageName) => string.Format(url, packageName, version, pageName);

    }
}
