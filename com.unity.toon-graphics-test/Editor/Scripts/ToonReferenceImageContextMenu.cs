using System.Collections.Generic;
using System.IO;
using Unity.ToonShader.GraphicsTest;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.Rendering.Toon {
internal static class ToonReferenceImageContextMenu {
    [MenuItem("Assets/Toon/Copy to Toon Reference Images (RP)")]
    private static void CopyToReferenceImages() {
        string[] sourcePath = GetSelectedAssetPath();
        if (sourcePath == null || sourcePath.Length == 0) {
            Debug.LogWarning("[UTS Test] No asset selected.");
            return;
        }
        
        foreach (string path in sourcePath) {
            CopySingleImageToReferenceImages(path);
        }
    }

    private static void CopySingleImageToReferenceImages(string sourcePath) {
        
        if (string.IsNullOrEmpty(sourcePath)) {
            Debug.LogWarning("[UTS Test] Empty path.");
            return;
        }

        if (AssetDatabase.IsValidFolder(sourcePath)) {
            Debug.LogWarning("[UTS Test] Selected object is a folder. Please select a file.");
            return;
        }

        if (!sourcePath.Replace('\\', '/').StartsWith(ACTUAL_IMAGES_FOLDER)) {
            Debug.LogWarning($"[UTS Test] Selected asset must be under '{ACTUAL_IMAGES_FOLDER}'. Selected: {sourcePath}");
            return;
        }

        // Relative path under Assets/ActualImages
        string relativePath = sourcePath.Substring(ACTUAL_IMAGES_FOLDER.Length);
        relativePath = relativePath.Replace('\\', '/').TrimStart('/');

        string[] pathParts = relativePath.Split('/');

        int successCount = 0;
        foreach (KeyValuePair<string, HashSet<string>> kv in m_platformGraphicsAPIs) {
            string platform = kv.Key;
            pathParts[1] = platform;
            foreach(string gfxAPI in kv.Value) {
                pathParts[2] = gfxAPI;
                string targetPath = string.Join("/", pathParts);
                targetPath = Path.Combine(UTSGraphicsTestConstants.ReferenceImagePath, targetPath).Replace('\\', '/');
                
                // Ensure directory exists
                string targetDir = Path.GetDirectoryName(targetPath);
                if (string.IsNullOrEmpty(targetDir)) {
                    Debug.LogError($"[UTS Test] Failed to resolve target directory for: {targetPath}");
                    continue;
                }

                Directory.CreateDirectory(targetDir);
                
                
                try {
                    File.Copy(sourcePath, targetPath, true);
                    Debug.Log($"[UTS Test] copied to: {targetPath}");
                    successCount++;
                } catch (IOException ioEx) {
                    Debug.LogError($"[UTS Test] IO error copying to '{targetPath}':\n{ioEx}");
                } catch (System.Exception ex) {
                    Debug.LogError($"[UTS Test] Unexpected error copying to '{targetPath}':\n{ex}");
                }
                
            }

        }
        
        Debug.Log($"[UTS Test] Copy completed. Targets succeeded: {successCount}");
        AssetDatabase.Refresh();
    }

    private static string[] GetSelectedAssetPath() {
        Object[] objs = Selection.objects;
        if (objs == null)
            return null;
        int numObjects = objs.Length;
        string[] paths = new string[numObjects];
        for (int i = 0; i < numObjects; i++) {
            paths[i] = AssetDatabase.GetAssetPath(objs[i]);
        }

        return paths;
    }
    
//----------------------------------------------------------------------------------------------------------------------
    
    private static readonly Dictionary<string, HashSet<string>> m_platformGraphicsAPIs = new Dictionary<string, HashSet<string>>() {
        { "WindowsEditor", new HashSet<string> { "Direct3D11", "Direct3D12", "Vulkan" } },
        { "OSXEditor_AppleSilicon", new HashSet<string> { "Metal", } },
    };

//----------------------------------------------------------------------------------------------------------------------

    private const string ACTUAL_IMAGES_FOLDER = "Assets/ActualImages";
}
}