using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the attacker's controls and capabilities.
/// </summary>
public class Attacker : MonoBehaviour
{
    public GameObject[] attackPrefabs;

    public GameObject target;

    private int discoverDuration = 10;
    private float discoverTimer = 0f;
    private bool discoverCount = false;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeCount = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (discoverCount)
            discoverTimer += Time.deltaTime;

        if (discoverTimer >= discoverDuration)
            Discover();

        if (analyzeCount)
            analyzeTimer += Time.deltaTime;

        if (analyzeTimer >= analyzeDuration)
            Analyze();

        if (Input.GetKeyDown(KeyCode.Q))
            discoverCount = true;

        if (Input.GetKeyDown(KeyCode.W))
            analyzeCount = true;

        if (target)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var go = Instantiate(attackPrefabs[0]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var go = Instantiate(attackPrefabs[1]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                var go = Instantiate(attackPrefabs[2]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                var go = Instantiate(attackPrefabs[3]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                var go = Instantiate(attackPrefabs[4]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                var go = Instantiate(attackPrefabs[5]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                var go = Instantiate(attackPrefabs[6]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                var go = Instantiate(attackPrefabs[7]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                var go = Instantiate(attackPrefabs[8]);
                go.GetComponent<Attack>().target = target;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                var go = Instantiate(attackPrefabs[9]);
                go.GetComponent<Attack>().target = target;
            }
        }
    }

    public void SetTarget(GameObject go)
    {
        target = go;
    }

    public void Discover()
    {
        discoverCount = false;
        discoverTimer = 0f;
        MessagingManager.BroadcastMessage(new Message("", name, MessageTypes.Game.DISCOVER));
    }

    public void Analyze()
    {
        analyzeCount = false;
        analyzeTimer = 0f;
    }
}
