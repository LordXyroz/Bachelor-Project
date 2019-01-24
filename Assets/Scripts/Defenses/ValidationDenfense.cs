using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidationDenfense : BaseDefense
{
    // Start is called before the first frame update
    void Start()
    {
        cost = 100;
        duration = 10;

        description = "Sanitize and validate user input";

        EventManager.StartListening(EventTypes.Attacks.INJECTION, OnAttack);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
            timer += Time.deltaTime;

        if (timer >= duration)
        {
            Effect();
            triggered = false;
            timer = 0;
        }
    }

    public override void Effect()
    {
        if (stopped)
            EventManager.TriggerEvent(EventTypes.Defenses.SANITIZE_INPUT);
        else
            Debug.Log("Defender lost!");
        //gameObject.SetActive(false);
    }

    public override void OnAttack()
    {
        stopped = false;
    }

    public override void OnDefense()
    {
        throw new System.NotImplementedException();
    }

    public override void StartDefense()
    {
        triggered = true;
    }
}
