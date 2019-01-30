using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectionAttack : BaseAttack
{
    // Start is called before the first frame update
    public override void Start()
    {
        cost = 100;
        duration = 6;

        description = "Do a code-injection attack";

        //EventManager.StartListening(EventTypes.Defenses.SANITIZE_INPUT, OnDefense);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (triggered)
            timer += Time.deltaTime;

        if (timer >= duration)
        {
            Effect();
            triggered = false;
            timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            EventManager.BroadcastMessage(new Message("GoogleAPI", name, MessageTypes.Events.ATTACK));
        }
    }
    
    public override void Effect()
    {
        //foreach (var def in target.implementedDefenses)
        //{
        //    
        //}

        //if (!stopped)
        //    EventManager.TriggerEvent(EventTypes.Attacks.INJECTION);
        //else
        //    Debug.Log("Attacker lost!");
        //gameObject.SetActive(false);    
    }

    public override void AttackResponse(Message message)
    {
        if (message.targetName == name)
        {
            Debug.Log("Message recieved from: " + message.senderName + " to me: " + name);
        }
    }

    public override void OnDefense()
    {
        stopped = true;
    }

    public override void StartAttack()
    {
        triggered = true;
    }
    
}
