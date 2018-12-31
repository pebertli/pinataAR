using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {

    private Dictionary<string, UnityEvent> eventDictionary;
    private Dictionary<string, UnityEvent<GameObject>> eventDictionaryGameObject;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if(!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
            }

            if (!eventManager)
                Debug.LogError("No event Manager");
            else
            {
                eventManager.Init();
            }

            return eventManager;
        }
    }

    void Init()
    {
        if(eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }

        if (eventDictionaryGameObject == null)
        {
            eventDictionaryGameObject = new Dictionary<string, UnityEvent<GameObject>>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        if(instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StartListening(string eventName, UnityAction<GameObject> listener)
    {
        UnityEvent<GameObject> thisEvent = null;

        if (instance.eventDictionaryGameObject.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new GameObjectUnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionaryGameObject.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<GameObject> listener)
    {
        if (eventManager == null)
            return;

        UnityEvent<GameObject> thisEvent = null;
        if (instance.eventDictionaryGameObject.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null)
            return;

        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    public static void TriggerEvent(string eventName, GameObject go)
    {
        UnityEvent<GameObject> thisEvent = null;
        if (instance.eventDictionaryGameObject.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(go);
        }
    }
}
