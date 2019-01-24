using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseAttack : MonoBehaviour
{
    public int cost;
    public int duration;

    public bool stopped = false;
    public bool triggered = false;

    public float timer = 0f;

    public string description;

    public abstract void Effect();

    public abstract void OnAttack();
    public abstract void OnDefense();

    public abstract void StartAttack();
}
