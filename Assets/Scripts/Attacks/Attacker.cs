using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.A))
        {
            var go = Instantiate(attackPrefabs[0]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            var go = Instantiate(attackPrefabs[1]);
            go.GetComponent<Attack>().target = GameObject.Find("GoogleAPI");
        }
    }
}
