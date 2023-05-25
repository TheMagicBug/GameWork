using UnityEditor;
using UnityEngine;

public class AutoBindGlobalSetting:ScriptableObject
{
    [SerializeField]
    private string m_CodePath;

    [SerializeField]
    private string m_Namespace;
    
    [SerializeField]
    private string m_BaseUICodePath;
    
    public string CodePath
    {
        get => m_CodePath;
    }

    public string Namespace
    {
        get => m_Namespace;
    }

    public string BaseUICodePath
    {
        get => m_BaseUICodePath;
    }

    [MenuItem("LazyDog/CreateAutoBindGlobalSetting")]
    public static void CreateAutoBindGlobalSetting()
    {
        string[] paths = AssetDatabase.FindAssets("t:AutoBindGlobalSetting");
        if (paths.Length >= 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(paths[0]);
            EditorUtility.DisplayDialog("警告", $"已经存在AutoBindGlobalSetting, 路径:{path}", "确认");
            return;
        }

        AutoBindGlobalSetting setting = CreateInstance<AutoBindGlobalSetting>();
        AssetDatabase.CreateAsset(setting, "Assets/AutoBindGlobalSetting.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
