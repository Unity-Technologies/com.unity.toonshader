using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

namespace Unity.ToonShader.GraphicsTest {
    
public static class GameViewUtility {

    //Copied from UnityEditor to provide access
    public enum GameViewSizeType {
        AspectRatio,
        FixedResolution,
    }
    
//----------------------------------------------------------------------------------------------------------------------

    public static void SetSize(int index)
    {
        EditorWindow gvWnd = EditorWindow.GetWindow(GameViewReflection.ASSEMBLY_TYPE);
        GameViewReflection.SELECTED_SIZE_INDEX_PROP.SetValue(gvWnd, index, null);
    }

    public static void AddAndSelectCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, 
        int width, int height, string text)
    {
        AddCustomSize(viewSizeType, sizeGroupType, width, height, text);
        int idx = GameViewUtility.FindSize(GameViewSizeGroupType.Standalone, width, height);
        GameViewUtility.SetSize(idx);
    }

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
    {
        object group = GetGroup(sizeGroupType);
        
        object newSize = GameViewSizeReflection.CTOR.Invoke(new object[] { (int)viewSizeType, width, height, text });
        GameViewSizesReflection.ADD_CUSTOM_SIZE.Invoke(group, new object[] { newSize });
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
        for (int i = 0; i < sizesCount; i++) {
            indexValue[0] = i;
            object size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)    {
        return GameViewSizesReflection.GET_GROUP.Invoke(GameViewSizesReflection.GAME_VIEW_SIZES_INSTANCE, new object[] { (int)type });
    }
    
    public static bool IsInitialized() {
        bool ret = GameViewSizesReflection.ASSEMBLY_TYPE != null
            && GameViewSizesReflection.GET_GROUP != null
            && GameViewSizesReflection.SINGLE_TYPE != null
            && GameViewSizesReflection.INSTANCE_PROP != null
            && GameViewSizesReflection.GAME_VIEW_SIZES_INSTANCE != null
            && GameViewSizesReflection.ADD_CUSTOM_SIZE != null
            && GameViewSizeReflection.ASSEMBLY != null
            && GameViewSizeReflection.GAME_VIEW_SIZE != null
            && GameViewSizeReflection.GAME_VIEW_SIZE_TYPE != null
            && GameViewSizeReflection.CTOR != null
            && GameViewReflection.ASSEMBLY_TYPE != null
            && GameViewReflection.SELECTED_SIZE_INDEX_PROP != null;

        FindSize( GameViewSizeGroupType.Standalone, 1920, 1080); // check execution
        
        return ret;
    }
    
//----------------------------------------------------------------------------------------------------------------------    

    private struct GameViewSizesReflection {
        internal static readonly Type ASSEMBLY_TYPE = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        internal static readonly MethodInfo GET_GROUP = ASSEMBLY_TYPE.GetMethod("GetGroup");
        internal static readonly Type SINGLE_TYPE = typeof(ScriptableSingleton<>).MakeGenericType(ASSEMBLY_TYPE);
        internal static readonly PropertyInfo INSTANCE_PROP = SINGLE_TYPE.GetProperty("instance");
        internal static readonly object GAME_VIEW_SIZES_INSTANCE = INSTANCE_PROP.GetValue(null, null);
        internal static readonly MethodInfo ADD_CUSTOM_SIZE = GET_GROUP.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
    }
    
    struct GameViewSizeReflection {
        internal static readonly Assembly ASSEMBLY = Assembly.Load("UnityEditor.dll");
        internal static readonly Type GAME_VIEW_SIZE = ASSEMBLY.GetType("UnityEditor.GameViewSize");
        internal static readonly Type GAME_VIEW_SIZE_TYPE = ASSEMBLY.GetType("UnityEditor.GameViewSizeType");
        internal static readonly ConstructorInfo CTOR = GAME_VIEW_SIZE.GetConstructor(new Type[]
        {
            GAME_VIEW_SIZE_TYPE,
            typeof(int),
            typeof(int),
            typeof(string)
        });
    }

    
    struct GameViewReflection {
        internal static readonly Type ASSEMBLY_TYPE = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        internal static readonly PropertyInfo SELECTED_SIZE_INDEX_PROP = ASSEMBLY_TYPE.GetProperty("selectedSizeIndex",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
    }
    
}    
    
} //end namespace


#endif
