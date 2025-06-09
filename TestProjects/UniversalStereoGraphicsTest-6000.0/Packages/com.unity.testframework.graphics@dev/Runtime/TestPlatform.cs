using System;
using System.Runtime.InteropServices;
using Architecture = System.Runtime.InteropServices.Architecture;
using System.Globalization;
using UnityEngine;

namespace UnityEngine.TestTools.Graphics
{
    /// <summary>
    /// Represents the platform and architecture of the test environment.
    /// </summary>
    public class TestPlatform
    {
        private readonly Architecture architecture;
        private readonly RuntimePlatform platform;

        /// <summary>
        /// The OS architecture of the test environment.
        /// </summary>
        public Architecture Arch => architecture;

        /// <summary>
        /// The runtime platform value of the test environment.
        /// </summary>
        public RuntimePlatform Platform => platform;

        public TestPlatform(RuntimePlatform platform, Architecture architecture)
        {
            this.platform = platform;
            this.architecture = architecture;
        }

        /// <summary>
        /// Returns the current test environment's platform and architecture.
        /// </summary>
        /// <returns></returns>
        public static TestPlatform GetCurrent()
        {
            var currentPlatform = Application.platform;
            var currentArchitecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;

            #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            // Apple M1 processor is ARM64 but does not appear as such in RuntimeInformation.OSArchitecture on some testing environments
            if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(SystemInfo.processorType, "Apple M", CompareOptions.IgnoreCase) >= 0)
                currentArchitecture = System.Runtime.InteropServices.Architecture.Arm64;
            #endif

            return new TestPlatform(currentPlatform, currentArchitecture);
        }

        /// <summary>
        /// Converts the TestPlatform to a unique string value.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => platform.ToUniqueString(architecture);
    }
}
