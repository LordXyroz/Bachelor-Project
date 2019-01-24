using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectionAttack : BaseAttack
{
    // Start is called before the first frame update
    public void Start()
    {
        cost = 100;
        duration = 6;

        description = "Do a code-injection attack";

        EventManager.StartListening(EventTypes.Defenses.SANITIZE_INPUT, OnDefense);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
            timer += Time.deltaTime;

        if (timer >= duration)
            Effect();
    }
    
    public override void Effect()
    {
        if (!stopped)
            Debug.Log("WIN!");
        else
            Debug.Log("LOOSE!");
        gameObject.SetActive(false);    
    }

    public override void OnAttack()
    {
        throw new System.NotImplementedException();
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
