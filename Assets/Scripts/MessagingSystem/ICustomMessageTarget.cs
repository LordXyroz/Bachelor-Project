using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICustomMessageTarget : IEventSystemHandler
{

    void Attack();
    void Defense();
}
