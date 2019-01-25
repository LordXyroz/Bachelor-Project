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

    public BaseNetComponent target;

    public abstract void Start();
    public abstract void Update();

    public abstract void Effect();

    public abstract void OnAttack();
    public abstract void OnDefense();

    public abstract void StartAttack();
}
