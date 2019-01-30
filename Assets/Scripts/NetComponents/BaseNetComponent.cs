using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNetComponent : MonoBehaviour
{
    public List<BaseAttack> vulnerability;
    public List<BaseDefense> availableDefenses;
    public List<BaseDefense> implementedDefenses;

    public abstract void Start();
    public abstract void Update();

    public abstract void UnderAttack(Message message);
}
