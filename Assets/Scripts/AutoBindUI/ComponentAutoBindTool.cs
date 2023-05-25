using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 组件自动绑定工具
/// </summary>
public class ComponentAutoBindTool : MonoBehaviour
{
#if UNITY_EDITOR
    [Serializable]
    public class BindData
    {
        public BindData()
        {
            
        }

        public BindData(string name, Component bindComponent)
        {
            Name = name;
            BindComponent = bindComponent;
        }

        public string Name;
        public Component BindComponent;
    }

    public List<BindData> BindDatas = new List<BindData>();

    [SerializeField]
    private string m_ClassName;

    [SerializeField]
    private string m_Namespace;

    [SerializeField]
    private string m_CodePath;

    [SerializeField]
    private string m_BaseUICodePath;

    public string ClassName
    {
        get
        {
            return m_ClassName;
        }
    }

    public string Namespace
    {
        get
        {
            return m_Namespace;
        }
    }

    public string CodePath
    {
        get
        {
            return m_CodePath;
        }
    }

    public string BaseUICodePath
    {
        get
        {
            return m_BaseUICodePath;
        }
    }

    public IAutoBindRuleHelper RuleHelper
    {
        get;
        set;
    }
#endif

    [SerializeField]
    public List<Component> m_BindComponents = new List<Component>();

    public T GetBindComponent<T>(int index) where T : Component
    {
        if (index >= m_BindComponents.Count)
        {
            Debug.LogError("索引超出下标!!!");
            return null;
        }

        T bindCom = m_BindComponents[index] as T;

        if (bindCom == null)
        {
            Debug.LogError("类型无效!!!");
            return null;
        }

        return bindCom;
    }
}
