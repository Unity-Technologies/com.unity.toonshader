using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools.Graphics;

namespace UnityEditor.TestTools.Graphics
{
    public static class CliArgumentsCheck
    {
        // Graphics API arguments, used to start the Editor with the selected api
        // List compiled from: https://docs.unity3d.com/6000.0/Documentation/Manual/EditorCommandLineArguments.html
        static List<GraphicsApiArguments> forceGfxApiArguments = new List<GraphicsApiArguments>
        {
            new("-force-d3d11", GraphicsDeviceType.Direct3D11),
            new("-force-d3d12", GraphicsDeviceType.Direct3D12),
            new("-force-glcore", GraphicsDeviceType.OpenGLCore),
            new("-force-gles", GraphicsDeviceType.OpenGLES3),
            new("-force-vulkan", GraphicsDeviceType.Vulkan),
            new("-force-metal", GraphicsDeviceType.Metal)
        };

        // List of custom cli arguments found in SetupProject.cs ApplySettings()
        // The Graphics Test Framework parses these
        static List<GraphicsApiArguments> graphicsTestFrameworkArguments = new List<GraphicsApiArguments>
        {
            new("d3d11", GraphicsDeviceType.Direct3D11),
            new("d3d12", GraphicsDeviceType.Direct3D12),
            new("glcore", GraphicsDeviceType.OpenGLCore),
#if !UNITY_2023_1_OR_NEWER
            new("gles2", GraphicsDeviceType.OpenGLES2),
#endif
            new("gles3", GraphicsDeviceType.OpenGLES3),
            new("vulkan", GraphicsDeviceType.Vulkan),
            new("metal", GraphicsDeviceType.Metal),
            new("ps4", GraphicsDeviceType.PlayStation4),
            new("ps5", GraphicsDeviceType.PlayStation5),
            new("ps5nggc", GraphicsDeviceType.PlayStation5NGGC),
            new("gamecorexboxone", GraphicsDeviceType.GameCoreXboxOne),
            new("gamecorexboxseries", GraphicsDeviceType.GameCoreXboxSeries),
            new("switch", GraphicsDeviceType.Switch)
        };

        static string gtfApplySettingsArg = "SetupProject.ApplySettings";
        static string testSettingsFileArg = "-testSettingsFile";

        /// <summary>
        /// Read the commandline arguments passed to the Editor, and if the argument is meant to run tests with a certain graphics API, confirm that this API was correctly switched to.
        /// In case of failure to do so, throw an error.
        /// </summary>
        public static void CheckGfxApiAgainstArguments(List<string> cliArguments, List<string> testSettings = null, GraphicsDeviceType? currentEditorGfxApi = null, GraphicsDeviceType? currentPlayerSettingsGfxApi = null)
        {
            // Make sure no duplicate graphics API CLI arguments were passed that would conflict with each other
            CheckForConflictingEditorCliArgs(cliArguments);
            CheckForConflictingPlayerCliArgs(cliArguments, testSettings);

            // Check the current Editor graphics API and compare it to the passed CLI arguments
            if (currentEditorGfxApi == null)
            {
                currentEditorGfxApi = SystemInfo.graphicsDeviceType;
            }
            CheckEditorGfxApiAgainstArgs(cliArguments, currentEditorGfxApi);


            // Check the first Graphics API inside the Player Settings and compare it to the passed CLI arguments
            if (currentPlayerSettingsGfxApi == null)
            {
                currentPlayerSettingsGfxApi = PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0];
            }
            CheckPlayerSettingsGfxApiAgainstGtfArgs(cliArguments, currentPlayerSettingsGfxApi);
            CheckPlayerSettingsGfxApiAgainstCliProjectSetupArgs(cliArguments, currentPlayerSettingsGfxApi);

            // Parse TestSettings.json. When using the flag '--playergraphicsapi=', the API to set in the Player Settings will be found in that file.
            if (cliArguments.Contains(testSettingsFileArg, StringComparer.CurrentCultureIgnoreCase))
            {
                if (testSettings == null)
                {
                    testSettings = File.ReadAllText(cliArguments[cliArguments.FindIndex(x => x.Equals(testSettingsFileArg, StringComparison.CurrentCultureIgnoreCase)) + 1].ToString()).Split('"').ToList();
                }

                CheckPlayerSettingsGfxApiAgainstUtrArgs(cliArguments, currentPlayerSettingsGfxApi, testSettings);
            }
        }

        /// <summary>
        /// Throw an error if multiples of the same type of CLI argument for forcing the Editor graphics API were passed.
        /// </summary>
        public static List<string> CheckForConflictingEditorCliArgs(List<string> cliArguments)
        {
            var possibleArguments = forceGfxApiArguments.Select(x => x.Argument).ToList();
            var foundArguments = new List<string>();

            foreach (var arg in cliArguments.Where(arg => possibleArguments.Contains(arg)))
            {
                foundArguments.Add(arg);
            }

            foundArguments = foundArguments.Distinct().ToList();
            if (foundArguments.Count > 1)
            {
                GraphicsTestLogger.Log(LogType.Error, $"Conflicting commandline arguments were found: {string.Join(" ", foundArguments)}. Only one argument should be passed to force the Editor graphics API.");
            }

            return foundArguments;
        }

        /// <summary>
        /// Throw an error if multiple CLI argument for forcing different Player graphics APIs were passed.
        /// </summary>
        public static List<string> CheckForConflictingPlayerCliArgs(List<string> cliArguments, List<string> testSettings)
        {
            var graphicsDeviceTypes = Enum.GetValues(typeof(GraphicsDeviceType)).Cast<GraphicsDeviceType>().ToList().Select(x => x.ToString()).ToList();

            var gtfArgs = graphicsTestFrameworkArguments.Select(x => x.Argument).ToList();
            var gtfApis = graphicsTestFrameworkArguments.Select(x => x.GraphicsApi).ToList();

            var foundApis = new List<string>();

            // Look through cli-project-setup arguments
            foreach (var arg in cliArguments.Where(arg => arg.StartsWith("-playergraphicsapi=", StringComparison.CurrentCultureIgnoreCase)))
            {
                foundApis.Add(arg.Split("=")[1]);
            }

            // Look through GraphicsTestFramework arguments
            var foundGtfArgs = FilterNonGtfArguments(cliArguments);

            foreach (var arg in foundGtfArgs.Where(arg => gtfArgs.Contains(arg, StringComparer.CurrentCultureIgnoreCase)))
            {
                foundApis.Add(gtfApis[gtfArgs.FindIndex(x => x.Equals(arg, StringComparison.CurrentCultureIgnoreCase))].ToString());
            }

            // Look through UTR arguments
            if (cliArguments.Contains(testSettingsFileArg, StringComparer.CurrentCultureIgnoreCase))
            {
                if (testSettings == null)
                {
                    testSettings = File.ReadAllText(cliArguments[cliArguments.FindIndex(x => x.Equals(testSettingsFileArg, StringComparison.CurrentCultureIgnoreCase)) + 1].ToString()).Split('"').ToList();
                }

                foreach (var arg in testSettings.Where(arg => graphicsDeviceTypes.Contains(arg, StringComparer.CurrentCultureIgnoreCase)))
                {
                    foundApis.Add(arg);
                }
            }

            foundApis = foundApis.Distinct().ToList();
            if (foundApis.Count > 1)
            {
                GraphicsTestLogger.Log(LogType.Error, $"Conflicting commandline arguments are forcing multiple graphics APIs for the Player Settings: {string.Join(" ", foundApis)}. Only one argument should be passed to force the Player Settings graphics API.");
            }

            return foundApis;
        }

        /// <summary>
        /// Check if Editor was forced to a certain graphics API (for Editor Playmode runs)
        /// </summary>
        internal static void CheckEditorGfxApiAgainstArgs(List<string> cliArguments, GraphicsDeviceType? currentEditorGfxApi)
        {
            var editorForceGfxApiArgs = forceGfxApiArguments.Select(x => x.Argument).ToList();
            var editorApis = forceGfxApiArguments.Select(x => x.GraphicsApi).ToList();

            foreach (var arg in editorForceGfxApiArgs.Where(arg => (cliArguments.Contains(arg, StringComparer.CurrentCultureIgnoreCase))))
            {
                if (currentEditorGfxApi == editorApis[editorForceGfxApiArgs.FindIndex(x => x.Equals(arg, StringComparison.CurrentCultureIgnoreCase))])
                {
                    GraphicsTestLogger.Log(LogType.Log, $"{arg} argument was passed to the Editor. Running with {currentEditorGfxApi}");
                }
                else
                {
                    GraphicsTestLogger.Log(LogType.Error, $"{arg} argument was passed to the Editor, but the Editor is running with {currentEditorGfxApi}");
                }
            }
        }

        /// <summary>
        /// Parse TestSettings.json. When using the flag '--playergraphicsapi=', the API to set in the Player Settings will be found in that file.
        /// </summary>
        internal static void CheckPlayerSettingsGfxApiAgainstUtrArgs(List<string> cliArguments, GraphicsDeviceType? playerSettingsGfxApi, List<string> testSettings = null)
        {
            var graphicsDeviceTypes = Enum.GetValues(typeof(GraphicsDeviceType)).Cast<GraphicsDeviceType>().ToList();

            if (testSettings.Contains("playerGraphicsAPI", StringComparer.CurrentCultureIgnoreCase))
            {
                foreach (var gfxDevice in graphicsDeviceTypes.Where(gfxDevice => (testSettings.Contains(gfxDevice.ToString(), StringComparer.CurrentCultureIgnoreCase))))
                {
                    if (playerSettingsGfxApi == gfxDevice)
                    {
                        GraphicsTestLogger.Log(LogType.Log, $"\"playerGraphicsAPI\":\"{gfxDevice}\" was found in TestSettings.json, and the Player Settings Graphics API list starts with {playerSettingsGfxApi}");
                    }
                    else
                    {
                        GraphicsTestLogger.Log(LogType.Error, $"\"playerGraphicsAPI\":\"{gfxDevice}\" was found in TestSettings.json, but the Player Settings Graphics API list starts with {playerSettingsGfxApi}");
                    }
                }
            }
        }

        /// <summary>
        /// Check the custom arguments supported in SetupProject.cs
        /// </summary>
        internal static void CheckPlayerSettingsGfxApiAgainstGtfArgs(List<string> cliArguments, GraphicsDeviceType? playerSettingsGfxApi)
        {
            var gtfGfxApiArgs = graphicsTestFrameworkArguments.Select(x => x.Argument).ToList();
            var gtfApis = graphicsTestFrameworkArguments.Select(x => x.GraphicsApi).ToList();

            var foundGtfArgs = FilterNonGtfArguments(cliArguments);

            foreach (var arg in gtfGfxApiArgs.Where(arg => foundGtfArgs.Contains(arg, StringComparer.CurrentCultureIgnoreCase)))
            {
                if (playerSettingsGfxApi == gtfApis[gtfGfxApiArgs.FindIndex(x => x.Equals(arg, StringComparison.CurrentCultureIgnoreCase))])
                {
                    GraphicsTestLogger.Log(LogType.Log, $"{arg} argument was passed to the Editor, and the Player Settings Graphics API list starts with {playerSettingsGfxApi}");
                }
                else
                {
                    GraphicsTestLogger.Log(LogType.Error, $"{arg} argument was passed to the Editor, but the Player Settings Graphics API list starts with {playerSettingsGfxApi}");
                }
            }
        }

        /// <summary>
        /// Find all arguments that start from "SetupProject.ApplySettings", and stop when we hit the first argument that doesn't belong to possibleGtfArgs
        /// </summary>
        internal static List<string> FilterNonGtfArguments(List<string> cliArguments)
        {
            var possibleGtfArgs = SetupProject.Options.Select(x => x.Key).ToList();
            var foundGtfArgs = new List<string>();

            if (cliArguments.Contains(gtfApplySettingsArg, StringComparer.CurrentCultureIgnoreCase))
            {
                for (int i = cliArguments.FindIndex(x => x.Equals(gtfApplySettingsArg, StringComparison.CurrentCultureIgnoreCase)) + 1; i < cliArguments.Count; i++)
                {
                    if (possibleGtfArgs.Contains(cliArguments[i], StringComparer.CurrentCultureIgnoreCase))
                    {
                        foundGtfArgs.Add(cliArguments[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return foundGtfArgs;
        }

        /// <summary>
        /// Check the '-playergraphicsapi=' Editor argument. Projects using cli-project-setup can parse this.
        /// </summary>
        internal static void CheckPlayerSettingsGfxApiAgainstCliProjectSetupArgs(List<string> cliArguments, GraphicsDeviceType? playerSettingsGfxApi)
        {
            foreach (var arg in cliArguments.Where(arg => arg.StartsWith("-playergraphicsapi=", StringComparison.CurrentCultureIgnoreCase)))
            {
                var parsedArg = arg.Split("=")[1];

                if (playerSettingsGfxApi.ToString().ToLower() == parsedArg.ToLower())
                {
                    GraphicsTestLogger.Log(LogType.Log, $"{arg} argument was passed to the Editor, and the Player Settings Graphics API list starts with {playerSettingsGfxApi}");
                }
                else
                {
                    GraphicsTestLogger.Log(LogType.Error, $"{arg} argument was passed to the Editor, but the Player Settings Graphics API list starts with {playerSettingsGfxApi}");
                }
            }
        }

        public static List<GraphicsApiArguments> GetForceGfxApiArguments()
        {
            return forceGfxApiArguments;
        }

        public static List<GraphicsApiArguments> GetGraphicsTestFrameworkArguments()
        {
            return graphicsTestFrameworkArguments;
        }
    }

    public class GraphicsApiArguments
    {
        public string Argument { get; set; }
        public GraphicsDeviceType GraphicsApi { get; set; }

        public GraphicsApiArguments(string argument, GraphicsDeviceType graphicsApi)
        {
            Argument = argument;
            GraphicsApi = graphicsApi;
        }
    }
}
