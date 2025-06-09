using NUnit.Framework;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools.Graphics;

namespace UnityEditor.TestTools.Graphics.Tests
{
    public class GraphicsTestLoggerTests
    {
        [Test]
        [TestCase("CustomTestLogs", "CustomGraphicsTestLogs.log")]
        [TestCase("CustomTestLogs", "CustomGraphicsTestLogs.txt")]
        [TestCase("CustomTestLogs", "CustomGraphicsTestLogs")]
        public void Log_WithCustomLogDirectoryFilename_CreatesFileAndLogs(string directory, string filename)
        {
            string realLogDirectory = Path.Combine(Application.dataPath, directory);
            string logFilePath = Path.Combine(realLogDirectory, filename);

            if (Directory.Exists(realLogDirectory))
            {
                Directory.Delete(realLogDirectory);
            }

            GraphicsTestLogger.Log(LogType.Log, "Test message", logFilePath);

            Assert.That(Directory.Exists(realLogDirectory));
            Assert.That(Directory.GetFiles(realLogDirectory).Length > 0);

            string[] logFileContents = File.ReadAllLines(logFilePath);
            Assert.AreEqual(1, logFileContents.Length);
            Assert.AreEqual("2", logFileContents[0].Substring(0, 1));

            Directory.Delete(realLogDirectory, true);
        }

        [Test]
        public void Log_WithInvalidCustomLogDirectoryFilename_UsesDefaultLogDirectoryFilename()
        {
            string realLogDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(realLogDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            LogAssert.Expect(LogType.Log, "Graphics Tests: Test message");
            LogAssert.Expect(LogType.Warning, "Graphics Tests: Log path is empty. Using default log path.");
            GraphicsTestLogger.Log(LogType.Log, "Test message", null);

            Assert.That(Directory.Exists(realLogDirectory));
            Assert.That(Directory.GetFiles(realLogDirectory).Length > 0);

            string[] logFileContents = File.ReadAllLines(logFilePath);
            Assert.AreEqual(1, logFileContents.Length);
            Assert.AreEqual("2", logFileContents[0].Substring(0, 1));
        }

        [Test]
        public void Log_CreatesLogFile()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            GraphicsTestLogger.Log(LogType.Log, "Test message");

            Assert.IsTrue(File.Exists(logFilePath));
        }

        [Test]
        public void Log_WritesMessageToLogFile()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            GraphicsTestLogger.Log(LogType.Log, "Test message");

            string[] logFileContents = File.ReadAllLines(logFilePath);

            Assert.AreEqual(1, logFileContents.Length);
            Assert.AreEqual("2", logFileContents[0].Substring(0, 1));
        }

        [Test]
        public void Log_WritesMessageToLogFileWithCorrectTimestamp()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            GraphicsTestLogger.Log(LogType.Log, "Test message");

            string[] logFileContents = File.ReadAllLines(logFilePath);

            string timestamp = logFileContents[0].Substring(0, 19);
            TestContext.Out.WriteLine(timestamp);
            DateTime parsedTimestamp;
            Assert.IsTrue(DateTime.TryParse(timestamp, out parsedTimestamp));
        }

        [Test]
        public void LogError_WritesMessageToLogFileWithCorrectType()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            LogAssert.Expect(LogType.Error, "Graphics Tests: Test message");
            GraphicsTestLogger.Log(LogType.Error, "Test message");

            string[] logFileContents = File.ReadAllLines(logFilePath);

            Assert.AreEqual("[ERROR]:\tTest message", logFileContents[0].Substring(22));
        }

        [Test]
        public void LogWarning_WritesMessageToLogFileWithCorrectType()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            LogAssert.Expect(LogType.Warning, "Graphics Tests: Test message");
            GraphicsTestLogger.Log(LogType.Warning, "Test message");

            string[] logFileContents = File.ReadAllLines(logFilePath);

            Assert.AreEqual("[WARN]:\tTest message", logFileContents[0].Substring(22));
        }

        [Test]
        public void LogLog_WritesMessageToLogFileWithCorrectType()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            LogAssert.Expect(LogType.Log, "Graphics Tests: Test message");
            GraphicsTestLogger.Log(LogType.Log, "Test message");

            string[] logFileContents = File.ReadAllLines(logFilePath);

            Assert.AreEqual("[INFO]:\tTest message", logFileContents[0].Substring(22));
        }

        [Test]
        public void Log_MultipleTimes_WritesMessagesToLogFileInCorrectOrder()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            LogAssert.Expect(LogType.Log, "Graphics Tests: Test message 1");
            LogAssert.Expect(LogType.Log, "Graphics Tests: Test message 2");
            GraphicsTestLogger.Log(LogType.Log, "Test message 1");
            GraphicsTestLogger.Log(LogType.Log, "Test message 2");

            string[] logFileContents = File.ReadAllLines(logFilePath);

            Assert.AreEqual(2, logFileContents.Length);
            Assert.AreEqual("[INFO]:\tTest message 1", logFileContents[0].Substring(22));
            Assert.AreEqual("[INFO]:\tTest message 2", logFileContents[1].Substring(22));
        }

        [Test]
        public void Log_WithDifferentTypes_WritesMessagesToLogFileInCorrectOrder()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            LogAssert.Expect(LogType.Error, "Graphics Tests: Test message 1");
            LogAssert.Expect(LogType.Warning, "Graphics Tests: Test message 2");
            LogAssert.Expect(LogType.Log, "Graphics Tests: Test message 3");
            GraphicsTestLogger.Log(LogType.Error, "Test message 1");
            GraphicsTestLogger.Log(LogType.Warning, "Test message 2");
            GraphicsTestLogger.Log(LogType.Log, "Test message 3");

            string[] logFileContents = File.ReadAllLines(logFilePath);

            Assert.AreEqual(3, logFileContents.Length);
            Assert.AreEqual("[ERROR]:\tTest message 1", logFileContents[0].Substring(22));
            Assert.AreEqual("[WARN]:\tTest message 2", logFileContents[1].Substring(22));
            Assert.AreEqual("[INFO]:\tTest message 3", logFileContents[2].Substring(22));
        }

        [Test]
        public void Log_WithEmptyMessage_DoesNotWriteToLogFile()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            GraphicsTestLogger.Log(LogType.Log, "");
            GraphicsTestLogger.Log(LogType.Log, null);

            Assert.IsFalse(File.Exists(logFilePath));
        }

        [Test]
        public void Log_WithRootedPath_CreatesFileAndLogs()
        {
            string logFilePath = Path.Combine(Application.dataPath, "TestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            GraphicsTestLogger.Log(LogType.Log, "Test message", logFilePath);

            Assert.IsTrue(File.Exists(logFilePath));

            string[] logFileContents = File.ReadAllLines(logFilePath);
            Assert.AreEqual(1, logFileContents.Length);
            Assert.AreEqual("2", logFileContents[0].Substring(0, 1));

            File.Delete(logFilePath);
        }

        [Test]
        public void Log_WithIncorrectLogType_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GraphicsTestLogger.Log((LogType)4, "Test message"));
        }

        [TearDown]
        public void TearDown()
        {
            string logDirectory = Path.Combine(Application.dataPath, "../Logs");
            string logFilePath = Path.Combine(logDirectory, "GraphicsTestLogs.log");

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
        }
    }
}
