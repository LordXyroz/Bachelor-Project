using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IAttackResponse
{
    [Header("Cost and duration")]
    public int cost;
    public int duration;

    [Header("Attack info")]
    public string description;
    
    [Header("Attack type")]
    public AttackTypes attackType;

    [HideInInspector]
    public GameObject target;

    private float timer = 0f;
    private bool triggered = false;

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration && !triggered)
            Effect();
    }

    public void Effect()
    {
        triggered = true;
        MessagingManager.BroadcastMessage(new Message(target.name, name, MessageTypes.Events.ATTACK, attackType));
    }
    
    public void AttackResponse(Message message)
    {
        if (message.targetName == name)
        {
            if (message.success)
                Debug.Log("Attack was successful!");
            else
                Debug.Log("Attack failed!");

            Destroy(gameObject);
        }
    }
}
