//#define USE_GITHUB_DOC_LINK
#define USE_UTS_DOC_LINK
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Rendering.Toon
{
    /// <summary>
    /// Attribute to define the help url
    /// </summary>
    /// <example>
    /// [CoreRPHelpURLAttribute("Volume")]
    /// public class Volume : MonoBehaviour
    /// </example>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false)]
    internal class UTS3InspectorHelpURLAttribute : HelpURLAttribute
    {
        /// <summary>
        /// The constructor of the attribute
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="packageName"></param>
        internal UTS3InspectorHelpURLAttribute(string pageName, string packageName = "com.unity.toonshader")
            : base(UTS3DocumentationInfo.GetPageLink(packageName, pageName))
        {
        }
    }

    //We need to have only one version number amongst packages (so public)
    /// <summary>
    /// Documentation Info class.
    /// </summary>
    internal class UTS3DocumentationInfo
    {
        internal const string fallbackVersion = "0.7";
#if USE_GITHUB_DOC_LINK
        const string fallbackVersion = "";
        const string url = "https://github.com/Unity-Technologies/{0}/blob/development/v1/{0}/Documentation~/";
#elif USE_UTS_DOC_LINK
        const string url = "https://docs.unity3d.com/Packages/{0}@{1}/manual/";
#else
        
        const string url = "https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html";
#endif
        /// <summary>
        /// Current version of the documentation.
        /// </summary>
        internal static string version
        {
            get
            {
                return fallbackVersion;
            }
        }

        /// <summary>
        /// Generates a help url for the given package and page name
        /// </summary>
        /// <param name="packageName">The package name</param>
        /// <param name="pageName">The page name</param>
        /// <returns>The full url page</returns>
        internal static string GetPageLink(string packageName, string pageName) => string.Format(url, packageName, version, pageName);
    }

    /// <summary>
    /// Set of utils for documentation
    /// </summary>
    internal static class UTS3DocumentationUtils
    {
        /// <summary>
        /// Obtains the help url from an enum
        /// </summary>
        /// <typeparam name="TEnum">The enum with a <see cref="HelpURLAttribute"/></typeparam>
        /// <param name="mask">[Optional] The current value of the enum</param>
        /// <returns>The full url</returns>
        internal static string GetHelpURL<TEnum>(TEnum mask = default)
            where TEnum : struct, IConvertible
        {
            var type = mask.GetType();
            var attribute = type.GetCustomAttributes(typeof(HelpURLAttribute), false);


            var helpURLAttribute = (HelpURLAttribute)mask
                .GetType()
                .GetCustomAttributes(typeof(HelpURLAttribute), false)
                .FirstOrDefault();
#if USE_GITHUB_DOC_LINK
            return helpURLAttribute == null ? string.Empty : $"{helpURLAttribute.URL}{mask}.md";

#elif USE_UTS_DOC_LINK
            return helpURLAttribute == null ? string.Empty : $"{helpURLAttribute.URL}{mask}.html";
#else

            return helpURLAttribute == null ? string.Empty : $"{helpURLAttribute.URL}#{mask}";
#endif
        }
    }
}
