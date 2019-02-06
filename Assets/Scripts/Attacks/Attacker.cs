using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the attacker's controls and capabilities.
/// </summary>
public class Attacker : MonoBehaviour
{
    public GameObject[] attackPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var go = Instantiate(attackPrefabs[0]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var go = Instantiate(attackPrefabs[1]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var go = Instantiate(attackPrefabs[2]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var go = Instantiate(attackPrefabs[3]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            var go = Instantiate(attackPrefabs[4]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            var go = Instantiate(attackPrefabs[5]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            var go = Instantiate(attackPrefabs[6]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            var go = Instantiate(attackPrefabs[7]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            var go = Instantiate(attackPrefabs[8]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            var go = Instantiate(attackPrefabs[9]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
    }
}
