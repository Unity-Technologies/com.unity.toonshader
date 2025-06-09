using System;
using System.Runtime.InteropServices;
using System.Globalization;
using UnityEngine;

namespace UnityEngine.TestTools.Graphics
{
    /// <summary>
    /// Extension methods for the RuntimePlatform enum.
    /// </summary>
    public static class RuntimePlatformExtensions
    {
        /// <summary>
        /// Converts the RuntimePlatform to a unique string value.
        /// </summary>
        /// <param name="platform">The RuntimePlatform to convert.</param>
        /// <param name="architecture">The architecture of the platform.</param>
        /// <returns>A unique string value for the RuntimePlatform.</returns>
        /// <remarks>
        /// This method is required to generate backward compatible unique string values for duplicated RuntimePlatform enum values.
        /// </remarks>
        public static string ToUniqueString(this RuntimePlatform platform, System.Runtime.InteropServices.Architecture architecture)
        {
            string platformUniqueString = platform switch
            {
                RuntimePlatform.WSAPlayerX86 => "MetroPlayerX86", //duplicate RuntimePlatform.MetroPlayerX86
                RuntimePlatform.WSAPlayerX64 => "MetroPlayerX64", //duplicate RuntimePlatform.MetroPlayerX64
                RuntimePlatform.WSAPlayerARM => "MetroPlayerARM", //duplicate RuntimePlatform.MetroPlayerARM
                _ => platform.ToString(), // Use the default enum value
            };

            if (architecture is System.Runtime.InteropServices.Architecture.Arm64)
            {
                switch (platform)
                {
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.OSXEditor:
                        platformUniqueString += "_AppleSilicon";
                        break;
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        platformUniqueString += "_ARM64";
                        break;
                    default:
                        break;
                }
            }

            return platformUniqueString;
        }
    }
}
