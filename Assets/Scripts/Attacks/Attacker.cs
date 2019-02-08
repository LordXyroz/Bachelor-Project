using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// TODO!!!!!!!
/// Maybe use coroutines for handling discover/analyze/attack??

/// <summary>
/// Handles the attacker's controls and capabilities.
/// </summary>
public class Attacker : MonoBehaviour, IDiscoverResponse, IAnalyzeResponse
{
    public GameObject[] attackPrefabs;

    public GameObject target;

    private int discoverDuration = 3;
    private float discoverTimer = 0f;
    private bool discoverCount = false;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeCount = false;

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

    /// <summary>
    /// Changes the target for the next attack/analyze event.
    /// </summary>
    /// <param name="go">Gameobject set to be new target</param>
    public void SetTarget(GameObject go)
    {
        target = go;
    }

    /// <summary>
    /// Sends a discover message.
    /// Resets timers/bools.
    /// </summary>
    public void Discover()
    {
        discoverCount = false;
        discoverTimer = 0f;

        int currentDepth = -1;
        string targetName = "";
        if (target == null)
        {
            currentDepth = 0;
        }
        else
        {
            currentDepth = target.GetComponent<GameNetworkComponent>().graphDepth;
            targetName = target.GetComponent<GameNetworkComponent>().name;
        }

        MessagingManager.BroadcastMessage(new DiscoverMessage(targetName, name, MessageTypes.Game.DISCOVER, currentDepth));
    }

    /// <summary>
    /// Sends an analyze message.
    /// Rests timers/bools.
    /// </summary>
    public void Analyze()
    {
        analyzeCount = false;
        analyzeTimer = 0f;
        
        MessagingManager.BroadcastMessage(new Message(target.name, name, MessageTypes.Game.ANALYZE));
    }

    /// <summary>
    /// TODO: Implement function
    /// 
    /// From the IOnDiscoverResponse interface.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function.</param>
    public void OnDiscoverResponse(DiscoverResponseMessage message)
    {

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
