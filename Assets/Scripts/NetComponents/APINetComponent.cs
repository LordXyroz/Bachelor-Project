using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APINetComponent : BaseNetComponent
{
    public override void Start()
    {
        vulnerability = new List<BaseAttack>();
        availableDefenses = new List<BaseDefense>();
        implementedDefenses = new List<BaseDefense>();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
