using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//解决Dictionary不能使用泛型T
public interface IEventInfo { }

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件中心 单例模式对象
/// 1.Dictionary
/// 2.委托
/// 3.观察者设计模式
/// 4.泛型
/// </summary>
public class EventCenter
{
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventCenter();
            }
            return instance;
        }
    }
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    private EventCenter() { }


    public void AddEventListener<T>(string name, UnityAction<T> action)
    {

        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    public void AddEventListener(string name, UnityAction action)
    {

        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }


    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {

        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }

    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }

    public void EventTrigger<T>(string name, T obj)
    {

        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T>).actions != null)
            {
                (eventDic[name] as EventInfo<T>).actions.Invoke(obj);
            }
        }
    }

    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo).actions != null)
            {
                (eventDic[name] as EventInfo).actions.Invoke();
            }
        }
    }

    public void Clear()
    {
        eventDic.Clear();
    }
}
