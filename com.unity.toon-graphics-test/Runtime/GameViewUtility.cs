using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.ToonShader.GraphicsTest {
    
public static class GameViewUtility {

    //Copied from UnityEditor to provide access
    public enum GameViewSizeType {
        AspectRatio,
        FixedResolution,
    }
    
//----------------------------------------------------------------------------------------------------------------------
    static GameViewUtility() {
    }


    public static void SetSize(int index)
    {
        EditorWindow gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }

    public static void AddAndSelectCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
    {
        AddCustomSize(viewSizeType, sizeGroupType, width, height, text);
        int idx = GameViewUtility.FindSize(GameViewSizeGroupType.Standalone, width, height);
        GameViewUtility.SetSize(idx);
    }

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
    {
        object group = GetGroup(sizeGroupType);
        MethodInfo addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        
        object newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        // goal:
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
        // iterate through the sizes via group.GetGameViewSize(int index)

        object group = GetGroup(sizeGroupType);
        Type groupType = group.GetType();
        MethodInfo getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        MethodInfo getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        MethodInfo getGameViewSize = groupType.GetMethod("GetGameViewSize");
        Type gvsType = getGameViewSize.ReturnType;
        PropertyInfo widthProp = gvsType.GetProperty("width");
        PropertyInfo heightProp = gvsType.GetProperty("height");
        object[] indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            object size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }

//----------------------------------------------------------------------------------------------------------------------    
    
    static MethodInfo getGroup = sizesType.GetMethod("GetGroup");
    static Type sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
    static Type singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
    static PropertyInfo instanceProp = singleType.GetProperty("instance");
    static object gameViewSizesInstance = instanceProp.GetValue(null, null);

    static Assembly assembly = Assembly.Load("UnityEditor.dll");
    static Type gameViewSize = assembly.GetType("UnityEditor.GameViewSize");
    static Type gameViewSizeType = assembly.GetType("UnityEditor.GameViewSizeType");
    static ConstructorInfo ctor = gameViewSize.GetConstructor(new Type[]
    {
        gameViewSizeType,
        typeof(int),
        typeof(int),
        typeof(string)
    });

    static Type gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
    static PropertyInfo selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    
}    
    
} //end namespace

