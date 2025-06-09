using System;
using System.IO;
using UnityEngine;

namespace UnityEngine.TestTools.Graphics
{
    /// <summary>
    /// A logger for the graphics tests. This class is used to log messages to a file.
    /// The default log file is created as <c>Logs/GraphicsTestLogs.log</c> in the project's root directory.
    /// </summary>
    public sealed class GraphicsTestLogger
    {
        const string k_DefaultLogPath = "../Logs/GraphicsTestLogs.log";
        internal string LogPath { get; set; }

        internal RuntimePlatform[] SupportedPlatforms { get; } = new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.LinuxEditor,
            RuntimePlatform.LinuxPlayer,
            RuntimePlatform.OSXEditor,
            RuntimePlatform.OSXPlayer
        };

        private GraphicsTestLogger(string logPath)
        {
            LogPath = logPath;
        }

        /// <summary>
        /// Logs a message to the log file. The message is also written to the Unity console.
        /// </summary>
        /// <param name="type">
        /// The type of message to log. This can be LogType.Error, LogType.Warning, or LogType.Log.
        /// </param>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="logPath">
        /// The path to the log file. If this is null or empty, the default log path is used.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the type is not a valid LogType.
        /// </exception>
        public static void Log(LogType type, string message, string logPath = k_DefaultLogPath)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            GraphicsTestLogger logger = new GraphicsTestLogger(logPath);
            string timestamp = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            message = message.Trim();

            switch (type)
            {
                case LogType.Error:
                    Debug.LogError($"Graphics Tests: {message}");
                    logger.LogToFile($"{timestamp} - [ERROR]:\t{message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"Graphics Tests: {message}");
                    logger.LogToFile($"{timestamp} - [WARN]:\t{message}");
                    break;
                case LogType.Log:
                    Debug.Log($"Graphics Tests: {message}");
                    logger.LogToFile($"{timestamp} - [INFO]:\t{message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        internal void LogToFile(string message)
        {
            if (Array.Exists(SupportedPlatforms, p => p == Application.platform))
            {
                using (StreamWriter writer = GetStreamWriterForPath(LogPath))
                {
                    writer.WriteLine(message);
                }
            }
        }

        internal StreamWriter GetStreamWriterForPath(string logPath)
        {
            if (string.IsNullOrEmpty(logPath))
            {
                Debug.LogWarning("Graphics Tests: Log path is empty. Using default log path.");
                logPath = Path.Combine(Application.dataPath, k_DefaultLogPath);
            }
            else if (!Path.IsPathRooted(logPath))
            {
                logPath = Path.Combine(Application.dataPath, logPath);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            return new StreamWriter(logPath, true);
        }
    }
}
