using System.IO;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.Graphics
{
    internal class CreateSceneListFileFromBuildSettings : IPrebuildSetup
    {
        private readonly string assetsResourcesDirectory = "Assets/Resources";
        private readonly string sceneListFileName = "SceneList.txt";

        public void Setup()
        {
            if (!Directory.Exists(assetsResourcesDirectory))
            {
                Directory.CreateDirectory(assetsResourcesDirectory);
            }

            File.WriteAllLines($"{assetsResourcesDirectory}/{sceneListFileName}", EditorGraphicsTestCaseProvider.GetTestScenePaths());
        }
    }
}