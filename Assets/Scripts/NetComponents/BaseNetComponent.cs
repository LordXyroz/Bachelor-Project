using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNetComponent : MonoBehaviour
{
    public List<AttackEnum> vulnerability;
    public List<DefenseEnum> availableDefenses;
    public List<DefenseEnum> implementedDefenses;

    public abstract void Start();
    public abstract void Update();
}
