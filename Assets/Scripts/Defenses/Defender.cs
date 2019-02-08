using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the defender's controls and capabilities.
/// </summary>
public class Defender : MonoBehaviour, IAnalyzeResponse
{
    public GameObject[] defensePrefabs;

    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var go = Instantiate(defensePrefabs[0]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            var go = Instantiate(defensePrefabs[1]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var go = Instantiate(defensePrefabs[2]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            var go = Instantiate(defensePrefabs[3]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            var go = Instantiate(defensePrefabs[4]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            var go = Instantiate(defensePrefabs[5]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var go = Instantiate(defensePrefabs[6]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            var go = Instantiate(defensePrefabs[7]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            var go = Instantiate(defensePrefabs[8]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            var go = Instantiate(defensePrefabs[9]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            var go = Instantiate(defensePrefabs[10]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            var go = Instantiate(defensePrefabs[11]);
            go.GetComponent<Defense>().target = target;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            var go = Instantiate(defensePrefabs[12]);
            go.GetComponent<Defense>().target = target;
        }
    }

    /// <summary>
    /// Changes the target for the next attack/analyze event.
    /// </summary>
    /// <param name="go">Gameobject set to be new target</param>
    public void SetTarget(GameObject go)
    {
        target = go;
    }

    /// <summary>
    /// From the IOnAnalyzeResponse interface.
    /// 
    /// Gets a list of vulnerabilities from the sender.
    /// TODO: Use the list for game logic.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function.</param>
    public void OnAnalyzeResponse(AnalyzeResponeMessage message)
    {
        if (message.targetName == name)
        {
            Debug.Log("Analyze response from: " + message.senderName + " to me: " + message.targetName);
            if (message.attacks.Count == 0)
                Debug.Log(message.senderName + " has no vulnerabilities");
            foreach (var a in message.attacks)
                Debug.Log(message.senderName + " is vulnerable vs: " + a);
        }
    }
}
