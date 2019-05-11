using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEventManager
{
    #region Event Management
    static Dictionary<string, GlobalEvent> eventTable = new Dictionary<string, GlobalEvent>();
    public delegate void GlobalEventHandler();
    static Dictionary<string, GlobalEventHandler> immediateHandlersTable = new Dictionary<string, GlobalEventHandler>();


    public static void ResetGlobalEventManager()
    {
        immediateHandlersTable.Clear();
        eventTable.Clear();
    }

    public static void RegisterHandler(string evt, GlobalEventHandler handler)
    {
        if (string.IsNullOrEmpty(evt) || handler == null)
        {
            return;
        }

        if (!immediateHandlersTable.ContainsKey(evt))
        {
            immediateHandlersTable.Add(evt, handler);
        }
        else
        {
            immediateHandlersTable[evt] += handler;
        }
    }

    public static void UnregisterHandler(string evt, GlobalEventHandler handler)
    {
        if (string.IsNullOrEmpty(evt) || handler == null)
        {
            return;
        }

        if (immediateHandlersTable.ContainsKey(evt))
        {
            immediateHandlersTable[evt] -= handler;
        }
    }

    public static void UnregisterHandler(string evt)
    {
        if (string.IsNullOrEmpty(evt))
        {
            return;
        }

        if (immediateHandlersTable.ContainsKey(evt))
        {
            immediateHandlersTable.Remove(evt);
        }
    }

    public static void SendEvent(GlobalEvent globalEvent)
    {
        if (globalEvent == null || string.IsNullOrEmpty(globalEvent.evtName))
            return;

        if (immediateHandlersTable.ContainsKey(globalEvent.evtName) && immediateHandlersTable[globalEvent.evtName] != null)
        {
            immediateHandlersTable[globalEvent.evtName]();
        }
        else
        {
            if (!eventTable.ContainsKey(globalEvent.evtName))
                eventTable.Add(globalEvent.evtName, globalEvent);
        }
    }

    public static void SendEvent(string globalEvent)
    {
        if (string.IsNullOrEmpty(globalEvent))
            return;

        SendEvent(new GlobalEvent(globalEvent));
    }

    public static bool PeekEvent(string evt, bool consumed = true)
    {
        if (eventTable.ContainsKey(evt))
        {
            if (consumed)
            {
                eventTable.Remove(evt);
            }
            return true;
        }
        return false;
    }

    public static GlobalEvent AcquireEvent(string evt, bool consumed = true)
    {
        if (eventTable.ContainsKey(evt))
        {
            GlobalEvent e = eventTable[evt];
            if (consumed)
            {
                eventTable.Remove(evt);
            }
            return e;
        }
        return null;
    }

    public static string[] GetAllEvents()
    {
        string[] ret = new string[eventTable.Count];
        eventTable.Keys.CopyTo(ret, 0);
        return ret;
    }
    #endregion
}

public class GlobalEvent
{
    public string evtName;
    public object[] parameters;

    public GlobalEvent(string evtName)
    {
        this.evtName = evtName;
        parameters = null;
    }

    public GlobalEvent(string evtName, params object[] pars)
    {
        this.evtName = evtName;
        parameters = pars;
    }
}
