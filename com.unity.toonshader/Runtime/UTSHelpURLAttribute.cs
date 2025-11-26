using UnityEngine;

namespace Unity.Rendering.Toon {
internal class UTSHelpURLAttribute : HelpURLAttribute {

    internal UTSHelpURLAttribute(string pageName)
        : base(GetPageLink(pageName)) {
    }

    static string GetPageLink(string pageName) => string.Format(DOC_URL, 
        ToonConstants.PACKAGE_NAME, ToonConstants.PACKAGE_VERSION_MAJOR_MINOR, pageName);

//----------------------------------------------------------------------------------------------------------------------
    const string DOC_URL = "https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html";
    
}
}