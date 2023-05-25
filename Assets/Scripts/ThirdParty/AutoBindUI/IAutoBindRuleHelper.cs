using System.Collections.Generic;
using UnityEngine;

public interface IAutoBindRuleHelper
{
    /// <summary>
    /// 是否是有效绑定
    /// </summary>
    /// <param name="target"></param>
    /// <param name="filedNames"></param>
    /// <param name="componentTypeNames"></param>
    /// <returns></returns>
    bool IsValidBind(Transform target, List<string> filedNames, List<string> componentTypeNames);
}
