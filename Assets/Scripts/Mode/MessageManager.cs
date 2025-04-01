using System;
using System.Collections.Generic;



public delegate void Callback();
public delegate void Callback<T>(T t);
public delegate void Callback<T1, T2>(T1 t1, T2 t2);
public delegate void Callback<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
public delegate void Callback<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);

/// <summary>
/// 消息分发中心
/// </summary>
public static class MessageManager
{
    private static readonly Dictionary<string, Delegate> _allEvent = new Dictionary<string, Delegate>();

    #region AddListener

    private static void OnListenerAdding(string eventType)
    {
        if (string.IsNullOrEmpty(eventType))
        {
            //ColorLog.LogCyan("AddListener eventType is null or empty !!!");
            return;
        }

        if (!_allEvent.ContainsKey(eventType))
        {
            _allEvent.Add(eventType, null);
        }
    }

    public static void AddListener(string eventType, Callback callback)
    {
        OnListenerAdding(eventType);
        _allEvent[eventType] = (Callback)_allEvent[eventType] + callback;
    }

    public static void AddListener<T>(string eventType, Callback<T> callback)
    {
        OnListenerAdding(eventType);
        _allEvent[eventType] = (Callback<T>)_allEvent[eventType] + callback;
    }

    public static void AddListener<T1, T2>(string eventType, Callback<T1, T2> callback)
    {
        OnListenerAdding(eventType);
        _allEvent[eventType] = (Callback<T1, T2>)_allEvent[eventType] + callback;
    }

    public static void AddListener<T1, T2, T3>(string eventType, Callback<T1, T2, T3> callback)
    {
        OnListenerAdding(eventType);
        _allEvent[eventType] = (Callback<T1, T2, T3>)_allEvent[eventType] + callback;
    }

    public static void AddListener<T1, T2, T3, T4>(string eventType, Callback<T1, T2, T3, T4> callback)
    {
        OnListenerAdding(eventType);
        _allEvent[eventType] = (Callback<T1, T2, T3, T4>)_allEvent[eventType] + callback;
    }

    #endregion

    #region RemoveListener 

    private static bool OnListenerRemoving(string eventType)
    {
        if (string.IsNullOrEmpty(eventType))
        {
            //ColorLog.LogCyan("RemoveListener eventType is null or empty !!!");
            return false;
        }

        if (_allEvent.ContainsKey(eventType))
        {
            return true;
        }

        return false;
    }

    private static void OnListenerRemoved(string eventType)
    {
        if (_allEvent[eventType] == null)
        {
            _allEvent.Remove(eventType);
        }
    }

    public static void RemoveListener(string eventType, Callback callback)
    {
        var isHasEvent = OnListenerRemoving(eventType);
        if (isHasEvent)
        {
            _allEvent[eventType] = (Callback)_allEvent[eventType] - callback;
            OnListenerRemoved(eventType);
        }
    }

    public static void RemoveListener<T>(string eventType, Callback<T> callback)
    {
        var isHasEvent = OnListenerRemoving(eventType);
        if (isHasEvent)
        {
            _allEvent[eventType] = (Callback<T>)_allEvent[eventType] - callback;
            OnListenerRemoved(eventType);
        }
    }

    public static void RemoveListener<T1, T2>(string eventType, Callback<T1, T2> callback)
    {
        var isHasEvent = OnListenerRemoving(eventType);
        if (isHasEvent)
        {
            _allEvent[eventType] = (Callback<T1, T2>)_allEvent[eventType] - callback;
            OnListenerRemoved(eventType);
        }
    }

    public static void RemoveListener<T1, T2, T3>(string eventType, Callback<T1, T2, T3> callback)
    {
        var isHasEvent = OnListenerRemoving(eventType);
        if (isHasEvent)
        {
            _allEvent[eventType] = (Callback<T1, T2, T3>)_allEvent[eventType] - callback;
            OnListenerRemoved(eventType);
        }
    }

    public static void RemoveListener<T1, T2, T3, T4>(string eventType, Callback<T1, T2, T3, T4> callback)
    {
        var isHasEvent = OnListenerRemoving(eventType);
        if (isHasEvent)
        {
            _allEvent[eventType] = (Callback<T1, T2, T3, T4>)_allEvent[eventType] - callback;
            OnListenerRemoved(eventType);
        }
    }

    #endregion

    #region Broadcast

    private static void OnBroadcasting(string eventType)
    {
        if (string.IsNullOrEmpty(eventType))
        {
            //ColorLog.LogCyan("OnBroadcasting eventType is null or empty !!!");
        }
    }

    public static void Broadcast(string eventType)
    {
        OnBroadcasting(eventType);
        if (_allEvent.TryGetValue(eventType, out var outValue))
        {
            var callback = outValue as Callback;
            callback?.Invoke();
        }
    }

    public static void Broadcast<T>(string eventType, T t)
    {
        OnBroadcasting(eventType);
        if (_allEvent.TryGetValue(eventType, out var outValue))
        {
            var callback = outValue as Callback<T>;
            callback?.Invoke(t);
        }
    }

    public static void Broadcast<T1, T2>(string eventType, T1 t1, T2 t2)
    {
        OnBroadcasting(eventType);
        if (_allEvent.TryGetValue(eventType, out var outValue))
        {
            var callback = outValue as Callback<T1, T2>;
            callback?.Invoke(t1, t2);
        }
    }

    public static void Broadcast<T1, T2, T3>(string eventType, T1 t1, T2 t2, T3 t3)
    {
        OnBroadcasting(eventType);
        if (_allEvent.TryGetValue(eventType, out var outValue))
        {
            var callback = outValue as Callback<T1, T2, T3>;
            callback?.Invoke(t1, t2, t3);
        }
    }

    public static void Broadcast<T1, T2, T3, T4>(string eventType, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        OnBroadcasting(eventType);
        if (_allEvent.TryGetValue(eventType, out var outValue))
        {
            var callback = outValue as Callback<T1, T2, T3, T4>;
            callback?.Invoke(t1, t2, t3, t4);
        }
    }

    #endregion
}   