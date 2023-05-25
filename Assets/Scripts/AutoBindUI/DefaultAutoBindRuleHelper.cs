using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultAutoBindRuleHelper:IAutoBindRuleHelper
{
    private Dictionary<string, string> m_PrefixesDict = new Dictionary<string, string>()
    {
        {"Trans", "Transform"},
        {"OldAnim", "Animation"},
        {"NewAnim", "Animator"},
        
        {"Rect", "RectTransform"},
        {"Canvas", "Canvas"},
        {"Group", "CanvasGroup"},
        {"VGroup","VerticalLayoutGroup"},
        {"HGroup","HorizontalLayoutGroup"},
        {"GGroup","GridLayoutGroup"},
        {"TGroup","ToggleGroup"},

        {"Btn","Button"},
        {"Img","Image"},
        {"RImg","RawImage"},
        {"Txt","Text"},
        {"Input","InputField"},
        {"Slider","Slider"},
        {"Mask","Mask"},
        {"Mask2D","RectMask2D"},
        {"Tog","Toggle"},
        {"Sbar","Scrollbar"},
        {"SRect","ScrollRect"},
        {"Drop","Dropdown"},
        {"TMPTxt", "TMP_Text"},
    };
    
    public bool IsValidBind(Transform target, List<string> filedNames, List<string> componentTypeNames)
    {
        if (target.gameObject.IsPrefabInstance())
        {
            return false;
        }

        string[] strArray = target.name.Split("_");

        if (strArray.Length == 1)
        {
            return false;
        }

        string filedName = strArray[^1];
        for (int i = 0; i < strArray.Length - 1; i++)
        {
            string str = strArray[i];
            string comName;
            if (m_PrefixesDict.TryGetValue(str, out comName))
            {
                filedNames.Add($"{str}_{filedName}");
                componentTypeNames.Add(comName);
            }
            else
            {
                Debug.LogError($"{target.name}的命名中{str}不存在对应的组件类型，绑定失败");
            }
        }

        return true;
    }
}
