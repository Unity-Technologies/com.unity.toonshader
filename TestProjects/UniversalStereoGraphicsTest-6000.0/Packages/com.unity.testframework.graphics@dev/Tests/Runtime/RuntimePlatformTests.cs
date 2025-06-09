using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System;

namespace UnityEngine.TestTools.Graphics.Tests
{
    [TestFixture(System.Runtime.InteropServices.Architecture.Arm64)]
    [TestFixture(System.Runtime.InteropServices.Architecture.X64)]
    public class RuntimePlatformTests
    {
        System.Runtime.InteropServices.Architecture m_Architecture;
        static RuntimePlatform[] runtimePlatforms = Enum.GetValues(typeof(RuntimePlatform)) as RuntimePlatform[];

        public RuntimePlatformTests(System.Runtime.InteropServices.Architecture architecture)
        {
            m_Architecture = architecture;
        }

        [SetUp]
        public void SetUp()
        {
            TestContext.Out.WriteLine($"Test Platform OS Architecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
            TestContext.Out.WriteLine($"Test Platform Process Architecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
        }

        [Test]
        [TestCaseSource("runtimePlatforms")]
        public void ToUniqueString_ReturnsUniqueStringForPlatform(RuntimePlatform platform)
        {
            string expected  = platform switch
            {
                RuntimePlatform.WSAPlayerX86 => "MetroPlayerX86",
                RuntimePlatform.WSAPlayerX64 => "MetroPlayerX64",
                RuntimePlatform.WSAPlayerARM => "MetroPlayerARM",
                _ => platform.ToString(),
            };

            if (m_Architecture is System.Runtime.InteropServices.Architecture.Arm64)
            {
                switch (platform)
                {
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.OSXEditor:
                        expected += "_AppleSilicon";
                        break;
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        expected += "_ARM64";
                        break;
                    default:
                        break;
                }
            }

            Assert.That(platform.ToUniqueString(m_Architecture), Is.EqualTo(expected));
        }
    }
}
