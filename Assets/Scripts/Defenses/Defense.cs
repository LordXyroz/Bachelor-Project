using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour, IDefenseResponse
{
    [Header("Cost and duration")]
    public int cost;
    public int duration;

    [Header("Defense info")]
    public string description;

    [Header("Defense type")]
    public DefenseTypes defenseType;

    [HideInInspector]
    public GameObject target;

    private float timer;
    private bool triggered = false;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration && !triggered)
            Effect();
    }

    public void Effect()
    {
        triggered = true;
        MessagingManager.BroadcastMessage(new Message(target.name, name, MessageTypes.Events.DEFENSE, defenseType));
    }

    public void DefenseResponse(Message message)
    {
        if (message.targetName == name)
        {
            if (message.success)
                Debug.Log("Defense implemented!");
            else
                Debug.Log("Defense failed");

            Destroy(gameObject);
        }
    }
}
