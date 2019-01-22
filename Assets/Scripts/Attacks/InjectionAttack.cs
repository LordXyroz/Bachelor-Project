using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectionAttack : BaseAttack
{
    // Start is called before the first frame update
    override
    public void Start()
    {
        EventManager.StartListening(EventTypes.Defenses.SANITIZE_INPUT, OnDefend);
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    public void OnDefend()
    {
        Debug.Log("OnDefend was called!!");
    }
}
