using System;
using System.Collections.Generic;
using UnityEngine;

namespace SQDFC
{
    public class EventManager : SingletonBase<EventManager>
    {
        /// <summary>
        /// 存放UI窗体注册的消息监听事件
        /// </summary>
        public Dictionary<string, List<Action<object, string>>> m_AllListenerDict = new Dictionary<string, List<Action<object, string>>>();

        public Dictionary<string, List<Action>> m_AllListenerDict1 = new Dictionary<string, List<Action>>();

        public Dictionary<string, List<Action<object>>> m_AllListenerDict2 =
            new Dictionary<string, List<Action<object>>>();

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventHandler"></param>

        public void AddListener(string eventName, Action eventHandler)
        {
            if (m_AllListenerDict1.ContainsKey(eventName) && m_AllListenerDict1[eventName].Contains(eventHandler))
            {
                Debug.LogError("重复监听的事件!!!");
            }
            else
            {
                if (!m_AllListenerDict1.ContainsKey(eventName))
                {
                    m_AllListenerDict1.Add(eventName, new List<Action>(){eventHandler});
                }
                else
                {
                    m_AllListenerDict1[eventName].Add(eventHandler);
                }
            }
        }
        
        public void AddListener(string eventName, Action<object> eventHandler)
        {
            if (m_AllListenerDict2.ContainsKey(eventName) && m_AllListenerDict2[eventName].Contains(eventHandler))
            {
                Debug.LogError("重复监听的事件!!!");
            }
            else
            {
                if (!m_AllListenerDict2.ContainsKey(eventName))
                {
                    m_AllListenerDict2.Add(eventName, new List<Action<object>>(){eventHandler});
                }
                else
                {
                    m_AllListenerDict2[eventName].Add(eventHandler);
                }
            }
        }
        
        /// <summary>
        /// 移除指定的事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handler"></param>
        public void RemoveListener(string eventName, Action handler)
        {
            var keys = m_AllListenerDict1.Keys;
            foreach (var key in keys)
            {
                if (eventName == key)
                {
                    m_AllListenerDict1[key].Remove(handler);
                    if (m_AllListenerDict1[key].Count == 0)
                    {
                        m_AllListenerDict1.Remove(key);
                    }
                    break;
                }
            }
        }
        
        public void RemoveListener(string eventName, Action<object> handler)
        {
            var keys = m_AllListenerDict2.Keys;
            foreach (var key in keys)
            {
                if (eventName == key)
                {
                    m_AllListenerDict2[key].Remove(handler);
                    if (m_AllListenerDict2[key].Count == 0)
                    {
                        m_AllListenerDict2.Remove(key);
                    }
                    break;
                }
            }
        }
        
        /// <summary>
        /// 移除所有监听事件
        /// </summary>
        public void RemoveAllListener()
        {
            m_AllListenerDict1.Clear();
            m_AllListenerDict1 = new Dictionary<string, List<Action>>();
            m_AllListenerDict2.Clear();
            m_AllListenerDict2 = new Dictionary<string, List<Action<object>>>();
        }
        
        /// <summary>
        /// 广播事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="publishUIName"></param>
        // public void TriggerEvent(string eventName, object data = null, string publishUIName = "None")
        // {
        //     foreach (var key in m_AllListenerDict.Keys)
        //     {
        //         if (key == eventName)
        //         {
        //             foreach (var handler in m_AllListenerDict[key])
        //             {
        //                 handler.Invoke(data, publishUIName);
        //             }
        //         }
        //     }
        // }
        
        public void TriggerEvent(string eventName)
        {
            foreach (var key in m_AllListenerDict1.Keys)
            {
                if (key == eventName)
                {
                    foreach (var handler in m_AllListenerDict1[key])
                    {
                        handler.Invoke();
                    }
                }
            }
        }
        
        public void TriggerEvent(string eventName, object data = null)
        {
            foreach (var key in m_AllListenerDict2.Keys)
            {
                if (key == eventName)
                {
                    foreach (var handler in m_AllListenerDict2[key])
                    {
                        handler.Invoke(data);
                    }
                }
            }
        }
        
    }
}