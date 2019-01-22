using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseAttack : MonoBehaviour
{
    public int a;
    public float b;
    
    
    virtual public void Start()
    {
        Debug.Log("Base start called!");
    }

    
}
