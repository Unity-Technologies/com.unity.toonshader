using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;
using UnityEngine.TestTools.Graphics;

namespace UnityEditor.TestTools.Graphics.Tests
{
    public class ReferenceImageUtilityTests
    {
        [Test]
        public void CopyImageToBasePath_ImageIsCopied()
        {
            Directory.CreateDirectory("Assets/ReferenceImagesBase");

            Selection.activeObject = AssetDatabase.LoadAssetAtPath("Packages/com.unity.testframework.graphics/Tests/TestImages/16pxDummyReference.png", typeof(Texture2D));

            EditorReferenceImageUtility.CopyImagesToBasePath();

            Assert.IsNotNull(AssetDatabase.LoadAssetAtPath("Assets/ReferenceImagesBase/16pxDummyReference.png", typeof(Texture2D)), null);

            AssetDatabase.DeleteAsset("Assets/ReferenceImagesBase/16pxDummyReference.png");
        }

        [Test]
        public void CopyImageRecursively_ImageIsCopied()
        {
            string[] directories = new string[]
            {
                "Assets/ReferenceImages/Linear/a/b/c",
                "Assets/ReferenceImages/Linear/a/b/d",
                "Assets/ReferenceImages/Gamma/a/b/c",
                "Assets/ReferenceImages/Gamma/a/b/d",
                "Assets/ReferenceImages/ShouldNotBeCopied"
            };

            foreach(var directory in directories)
            {
                Directory.CreateDirectory(directory);
            }

            Selection.activeObject = AssetDatabase.LoadAssetAtPath("Packages/com.unity.testframework.graphics/Tests/TestImages/16pxDummyReference.png", typeof(Texture2D));

            EditorReferenceImageUtility.CopyImagesToDefaultPaths();
            string[] leafFolders = EditorReferenceImageUtility.EnumerateLeafFolders("Assets/ReferenceImages").ToArray();

            Assert.That(leafFolders.Length, Is.EqualTo(directories.Length));
            foreach (var folder in leafFolders)
            {
                if (folder.Contains("ShouldNotBeCopied"))
                {
                    Assert.IsNull(AssetDatabase.LoadAssetAtPath(folder + "/16pxDummyReference.png", typeof(Texture2D)), null);
                    continue;
                }
                Assert.IsNotNull(AssetDatabase.LoadAssetAtPath(folder + "/16pxDummyReference.png", typeof(Texture2D)), null);
            }

            AssetDatabase.DeleteAsset("Assets/ReferenceImages/Linear/a/b/c/16pxDummyReference.png");
            AssetDatabase.DeleteAsset("Assets/ReferenceImages/Linear/a/b/d/16pxDummyReference.png");
            AssetDatabase.DeleteAsset("Assets/ReferenceImages/Gamma/a/b/c/16pxDummyReference.png");
            AssetDatabase.DeleteAsset("Assets/ReferenceImages/Gamma/a/b/d/16pxDummyReference.png");
        }
    }
}
