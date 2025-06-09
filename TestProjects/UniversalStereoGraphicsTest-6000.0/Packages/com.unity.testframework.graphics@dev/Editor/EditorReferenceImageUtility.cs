using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine.TestTools.Graphics;
using Object = UnityEngine.Object;

namespace UnityEditor.TestTools.Graphics
{
    static class EditorReferenceImageUtility
    {
        const string k_ReferenceImagesRoot = "Assets/ReferenceImages";
        const string k_ReferenceImagesBaseRoot = "Assets/ReferenceImagesBase";

        [MenuItem("Assets/Graphics Test Framework/Copy Image(s) To.../Reference Images Base", priority = 1)]
        internal static void CopyImagesToBasePath()
        {
            CopyImages(k_ReferenceImagesBaseRoot);
        }

        [MenuItem("Assets/Graphics Test Framework/Copy Image(s) To.../Reference Images", priority = 2)]
        internal static void CopyImagesToDefaultPaths()
        {
            CopyImages(k_ReferenceImagesRoot + "/Linear");
            CopyImages(k_ReferenceImagesRoot + "/Gamma");
        }

        [MenuItem("Assets/Graphics Test Framework/Copy Image(s) To.../Custom Folder", priority = 3)]
        internal static void CopyImagesToCustomFolder()
        {
            string defaultPath = Directory.Exists(k_ReferenceImagesRoot) ? k_ReferenceImagesRoot : "Assets";

            string path = EditorUtility.OpenFolderPanel("Destination", defaultPath, "");
            if (string.IsNullOrEmpty(path))
                return;

            CopyImages(path);
        }

        [MenuItem("Assets/Graphics Test Framework/Copy Image(s) To.../Reference Images Base", true)]
        [MenuItem("Assets/Graphics Test Framework/Copy Image(s) To.../Reference Images", true)]
        [MenuItem("Assets/Graphics Test Framework/Copy Image(s) To.../Custom Folder", true)]
        public static bool LabelSceneForBake_Test()
        {
            return IsTextureAssetSelected();
        }

        static bool IsTextureAssetSelected()
        {
            UnityEngine.Object[] textureAssets = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

            return textureAssets.Length != 0;
        }

        internal static void CopyImages(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            string[] leafFolders = EnumerateLeafFolders(path).ToArray();
            int numOfLeafFolders = leafFolders.Length;

            Object[] selectedObjects = Selection.objects;
            int numOfCopies = numOfLeafFolders * selectedObjects.Length;

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                Texture2D selected = selectedObjects[i] as Texture2D;
                if (selected != null)
                {
                    string pathToOriginalImage = AssetDatabase.GetAssetPath(selected);
                    string extension = Path.GetExtension(pathToOriginalImage);
                    string imageName = selected.name + extension;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Copied \"" + imageName + "\" to...");

                    for (int j = 0; j < numOfLeafFolders; j++)
                    {
                        string leafFolder = leafFolders[j];
                        if (EditorUtility.DisplayCancelableProgressBar(
                                "Copy " + imageName + " to ReferenceImages",
                                string.Format("({0} of {1}) {2}", j, numOfCopies, leafFolder),
                                (float)j / numOfLeafFolders))
                        {
                            break;
                        }
                        AssetDatabase.CopyAsset(pathToOriginalImage, Path.Combine(leafFolder, imageName));
                        sb.AppendLine($"-> {leafFolder}");
                    }
                    EditorUtility.ClearProgressBar();
                    GraphicsTestLogger.Log(LogType.Log, sb.ToString());
                }
            }

            AssetDatabase.Refresh();
        }

        internal static IEnumerable<string> EnumerateLeafFolders(string root)
        {
            Stack<string> dir = new Stack<string>();
            dir.Push(root);

            while (dir.Count != 0)
            {
                bool anySubfolders = false;
                root = dir.Pop();

                foreach (var subfolder in Directory.EnumerateDirectories(root))
                {
                    dir.Push(subfolder);
                    anySubfolders = true;
                }

                if (!anySubfolders)
                {
                    yield return root.Replace('\\', '/');
                }
            }
        }
    }
}
