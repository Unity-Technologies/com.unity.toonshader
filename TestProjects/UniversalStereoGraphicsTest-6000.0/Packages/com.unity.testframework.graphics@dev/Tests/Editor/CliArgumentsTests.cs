using System.Linq;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.TestTools.Graphics;
using System;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.Graphics.Tests
{
    public class CliArgumentsData
    {
        static List<GraphicsApiArguments> forceGfxApiArguments = CliArgumentsCheck.GetForceGfxApiArguments();

        public static IEnumerable ForceGraphicsApiTestCases
        {
            get
            {
                foreach (var arg in forceGfxApiArguments)
                {
                    yield return new TestCaseData(arg.Argument, arg.GraphicsApi);
                }
            }
        }

        static List<GraphicsApiArguments> graphicsTestFrameworkArguments = CliArgumentsCheck.GetGraphicsTestFrameworkArguments();

        public static IEnumerable GraphicsTestFrameworkTestCases
        {
            get
            {
                foreach (var arg in graphicsTestFrameworkArguments)
                {
                    yield return new TestCaseData(new List<string> { "SetupProject.ApplySettings", arg.Argument }, arg.GraphicsApi);
                }
            }
        }

        static List<GraphicsDeviceType> graphicsDeviceTypes = Enum.GetValues(typeof(GraphicsDeviceType)).Cast<GraphicsDeviceType>().ToList();
        public static IEnumerable PlayerGraphicsApiTestCases
        {
            get
            {
                foreach (var graphicsDeviceType in graphicsDeviceTypes)
                {
                    yield return new TestCaseData($"-playerGraphicsAPI={graphicsDeviceType}", graphicsDeviceType);
                }
            }
        }

        public static IEnumerable UtrTestCases
        {
            get
            {
                foreach (var graphicsDeviceType in graphicsDeviceTypes)
                {
                    yield return new TestCaseData("-testSettingsFile", $"{ graphicsDeviceType }", graphicsDeviceType);
                }
            }
        }
    }

    public class CliArgumentsTests
    {
        [Test]
        [TestCaseSource(typeof(CliArgumentsData), nameof(CliArgumentsData.ForceGraphicsApiTestCases))]
        public void CheckGfxApiAgainstArguments_EditorGfxApiMatchesForceArg_LogIsLogged(string arg, GraphicsDeviceType api)
        {
            CliArgumentsCheck.CheckGfxApiAgainstArguments(new List<string> { arg }, null, api);
            LogAssert.Expect(LogType.Log, $"Graphics Tests: {arg} argument was passed to the Editor. Running with {api}");
        }

        [Test]
        public void CheckGfxApiAgainstArguments_EditorGfxApiDoesntMatchForceArg_ErrorIsLogged()
        {
            string cliArgument = "-force-vulkan";
            GraphicsDeviceType gfxApi = GraphicsDeviceType.Direct3D11;

            CliArgumentsCheck.CheckGfxApiAgainstArguments(new List<string> { cliArgument }, null, gfxApi);
            LogAssert.Expect(LogType.Error, $"Graphics Tests: {cliArgument} argument was passed to the Editor, but the Editor is running with {gfxApi}");
        }

        [Test]
        [TestCaseSource(typeof(CliArgumentsData), nameof(CliArgumentsData.GraphicsTestFrameworkTestCases))]
        public void CheckGfxApiAgainstArguments_PlayerGfxApiMatchesGtfArg_LogIsLogged(List<string> args, GraphicsDeviceType api)
        {
            CliArgumentsCheck.CheckGfxApiAgainstArguments(args, null, null, api);
            LogAssert.Expect(LogType.Log, $"Graphics Tests: {args[1]} argument was passed to the Editor, and the Player Settings Graphics API list starts with {api}");
        }

        [Test]
        public void CheckGfxApiAgainstArguments_PlayerGfxApiDoesntMatchGtfArg_ErrorIsLogged()
        {
            List<string> cliArguments = new List<string>{ "SetupProject.ApplySettings" , "vulkan" };
            GraphicsDeviceType gfxApi = GraphicsDeviceType.Direct3D11;

            CliArgumentsCheck.CheckGfxApiAgainstArguments(cliArguments, null, null, gfxApi);
            LogAssert.Expect(LogType.Error, $"Graphics Tests: {cliArguments[1]} argument was passed to the Editor, but the Player Settings Graphics API list starts with {gfxApi}");
        }

        [Test]
        public void CheckGfxApiAgainstArguments_NonGtfArgumentsAreIgnored_LogIsLogged()
        {
            List<string> cliArguments = new List<string> { "-buildTarget", "PS5", "SetupProject.ApplySettings", "ps5nggc" };
            GraphicsDeviceType gfxApi = GraphicsDeviceType.PlayStation5NGGC;

            CliArgumentsCheck.CheckGfxApiAgainstArguments(cliArguments, null, null, gfxApi);
            LogAssert.Expect(LogType.Log, $"Graphics Tests: {cliArguments[3]} argument was passed to the Editor, and the Player Settings Graphics API list starts with {gfxApi}");
        }

        [Test]
        [TestCaseSource(typeof(CliArgumentsData), nameof(CliArgumentsData.PlayerGraphicsApiTestCases))]
        public void CheckGfxApiAgainstArguments_PlayerGfxApiMatchesCliProjectSetupArg_LogIsLogged(string arg, GraphicsDeviceType api)
        {
            CliArgumentsCheck.CheckGfxApiAgainstArguments(new List<string> { arg }, null, null, api);
            LogAssert.Expect(LogType.Log, $"Graphics Tests: {arg} argument was passed to the Editor, and the Player Settings Graphics API list starts with {api}");
        }

        [Test]
        public void CheckGfxApiAgainstArguments_PlayerGfxApiDoesntMatchCliProjectSetupArg_ErrorIsLogged()
        {
            string cliArgument = "-playergraphicsapi=vulkan";
            GraphicsDeviceType gfxApi =  GraphicsDeviceType.Direct3D11;

            CliArgumentsCheck.CheckGfxApiAgainstArguments(new List<string> { cliArgument }, null, null, gfxApi);
            LogAssert.Expect(LogType.Error, $"Graphics Tests: {cliArgument} argument was passed to the Editor, but the Player Settings Graphics API list starts with {gfxApi}");
        }

        [Test]
        [TestCaseSource(typeof(CliArgumentsData), nameof(CliArgumentsData.UtrTestCases))]
        public void CheckGfxApiAgainstArguments_PlayerGfxApiMatchesUtrArg_LogIsLogged(string testSettingsArg, string gfxApiArg, GraphicsDeviceType api)
        {
            CliArgumentsCheck.CheckGfxApiAgainstArguments(new List<string> { testSettingsArg }, new List<string> { "playerGraphicsAPI", gfxApiArg }, null, api);
            LogAssert.Expect(LogType.Log, $"Graphics Tests: \"playerGraphicsAPI\":\"{gfxApiArg}\" was found in TestSettings.json, and the Player Settings Graphics API list starts with {api}");
        }

        [Test]
        public void CheckGfxApiAgainstArguments_PlayerGfxApiDoesntMatchUtrArg_ErrorIsLogged()
        {
            string cliArgument = "Vulkan";
            GraphicsDeviceType gfxApi = GraphicsDeviceType.Direct3D11;

            CliArgumentsCheck.CheckGfxApiAgainstArguments(new List<string> { "-testSettingsFile" }, new List<string> { "playerGraphicsAPI", cliArgument }, null, gfxApi);
            LogAssert.Expect(LogType.Error, $"Graphics Tests: \"playerGraphicsAPI\":\"{cliArgument}\" was found in TestSettings.json, but the Player Settings Graphics API list starts with {gfxApi}");
        }

        [Test]
        public void CheckForConflictingEditorCliArgs_EditorGfxApiDoesntMatchArgs_ErrorIsLogged()
        {
            List<string> cliArguments = new List<string>{ "-force-vulkan", "-force-d3d11", "-force-vulkan", "-force-d3d12" };

            var foundArgs = CliArgumentsCheck.CheckForConflictingEditorCliArgs(cliArguments);
            LogAssert.Expect(LogType.Error, $"Graphics Tests: Conflicting commandline arguments were found: {string.Join(" ", foundArgs)}. Only one argument should be passed to force the Editor graphics API.");
        }

        [Test]
        public void CheckForConflictingEditorCliArgs_PlayerGfxApiDoesntMatchArgs_ErrorIsLogged()
        {
            List<string> cliArguments = new List<string> { "-PlayerGraphicsAPI=Direct3D11", "-playergraphicsapi=metal", "d3d12", "-testSettingsFile" };
            List<string> testSettings = new List<string> { "playerGraphicsAPI", "Vulkan" };

            var foundArgs = CliArgumentsCheck.CheckForConflictingPlayerCliArgs(cliArguments, testSettings);
            LogAssert.Expect(LogType.Error, $"Graphics Tests: Conflicting commandline arguments are forcing multiple graphics APIs for the Player Settings: {string.Join(" ", foundArgs)}. Only one argument should be passed to force the Player Settings graphics API.");
        }
    }
}
