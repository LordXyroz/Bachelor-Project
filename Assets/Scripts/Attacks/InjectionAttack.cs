using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectionAttack : BaseAttack
{
    // Start is called before the first frame update
    public void Start()
    {
        cost = 100;
        duration = 60;

        description = "Do a code-injection attack";

        EventManager.StartListening(EventTypes.Defenses.SANITIZE_INPUT, OnDefense);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public override void Effect()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDefense()
    {
        throw new System.NotImplementedException();
    }
}
