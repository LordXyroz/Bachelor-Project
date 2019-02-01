using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the defender's controls and capabilities.
/// </summary>
public class Defender : MonoBehaviour
{
    public GameObject[] defensePrefabs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var go = Instantiate(defensePrefabs[0]);
            go.GetComponent<Defense>().target = GameObject.Find("GoogleAPI");
        }
    }
}
