using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoBindGlobalSetting))]
public class AutoBindGlobalSettingInspector:Editor
{
    private SerializedProperty m_CodePath;
    private SerializedProperty m_Namespace;
    private SerializedProperty m_BaseUICodePath;

    private void OnEnable()
    {
        m_CodePath = serializedObject.FindProperty("m_CodePath");
        m_Namespace = serializedObject.FindProperty("m_Namespace");
        m_BaseUICodePath = serializedObject.FindProperty("m_BaseUICodePath");
    }

    public override void OnInspectorGUI()
    {
        m_Namespace.stringValue = EditorGUILayout.TextField(new GUIContent("默认命名空间"), m_Namespace.stringValue);
        
        EditorGUILayout.LabelField("BindUI默认代码保存路径");
        EditorGUILayout.LabelField(m_CodePath.stringValue);
        if (GUILayout.Button("选择BindUI路径", GUILayout.Width(204f)))
        {
            m_CodePath.stringValue = EditorUtility.OpenFolderPanel("选择代码路径", Application.dataPath, "");
        }
        
        EditorGUILayout.LabelField("BaseUIView默认代码保存路径");
        EditorGUILayout.LabelField(m_BaseUICodePath.stringValue);
        if (GUILayout.Button("选择BaseUIView路径", GUILayout.Width(204f)))
        {
            m_BaseUICodePath.stringValue = EditorUtility.OpenFolderPanel("选择代码路径", Application.dataPath, "");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
