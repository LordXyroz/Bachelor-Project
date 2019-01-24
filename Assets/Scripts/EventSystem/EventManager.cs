using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<ushort, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType<EventManager>();

                if (!eventManager)
                    Debug.LogError("Needs an active EventManager in the scene!");
                else
                    eventManager.Init();
            }
            return eventManager;
        }
    }

    private void Init()
    {
        if (eventDictionary == null)
            eventDictionary = new Dictionary<ushort, UnityEvent>();
    }

    public static void StartListening(ushort type, UnityAction action)
    {
        UnityEvent thisEvent = null;

        if (Instance.eventDictionary.TryGetValue(type, out thisEvent))
            thisEvent.AddListener(action);
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(action);

            Instance.eventDictionary.Add(type, thisEvent);
        }
    }

    public static void StopListening(ushort type, System.Action action)
    {

    }

    public static void TriggerEvent(ushort type)
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(type, out thisEvent))
            thisEvent.Invoke();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            EventManager.TriggerEvent(EventTypes.Defenses.SANITIZE_INPUT);
        if (Input.GetKeyDown(KeyCode.S))
            FindObjectOfType<InjectionAttack>().StartAttack();
    }

    public void SomeFunc()
    {
        
    }
}

