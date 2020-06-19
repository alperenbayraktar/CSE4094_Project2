using System.Collections.Generic;
using UnityEngine;
public interface IEventManagerListener
{
    void OnEventRecieved(string eventName);
}//
public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    private List<IEventManagerListener> listenerList = new List<IEventManagerListener>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Debug.Log("Instance(EventManager) is assigned!");
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }

    public static void Subscribe(IEventManagerListener listener)
    {
        if (instance == null) return;

        if (instance.listenerList.Contains(listener)) return;

        instance.listenerList.Add(listener);
    }

    public static void Unsubscribe(IEventManagerListener listener)
    {
        if (instance == null) return;

        if (!instance.listenerList.Contains(listener)) return;

        instance.listenerList.Remove(listener);
    }

    public static void SendEvent(string eventName)
    {
        //Debug.Log(eventName + " was Raised!");
        foreach(IEventManagerListener listener in instance.listenerList)
        {
            listener.OnEventRecieved(eventName);
        }
    }
}
