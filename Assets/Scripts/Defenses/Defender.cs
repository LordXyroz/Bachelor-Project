﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the defender's controls and capabilities.
/// </summary>
public class Defender : MonoBehaviour, IAnalyzeResponse
{
    public GameObject[] defensePrefabs;

    public GameObject target;

    private int analyzeDuration = 10;
    private float analyzeTimer = 0f;
    private bool analyzeCount = false;

    private bool workInProgress = false;

    // Update is called once per frame
    void Update()
    {
        if (analyzeCount)
            analyzeTimer += Time.deltaTime;

        if (analyzeTimer >= analyzeDuration)
            Analyze();

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartDefense(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartDefense(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartDefense(2);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartDefense(3);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartDefense(4);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartDefense(5);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartDefense(6);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartDefense(7);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartDefense(8);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartDefense(9);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartDefense(10);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartDefense(11);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartDefense(12);
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
    /// Function to be hooked up to a button. Starts timer for Analyzing
    /// </summary>
    public void StartAnalyze()
    {
        if (!workInProgress)
        {
            workInProgress = true;
            analyzeCount = true;
        }
    }

    /// <summary>
    /// Function to be hooked up to a button. Starts a Defense implementation.
    /// </summary>
    /// <param name="id"></param>
    public void StartDefense(int id)
    {
        if (!workInProgress)
        {
            workInProgress = true;

            var go = Instantiate(defensePrefabs[id]);
            go.GetComponent<Defense>().target = target;
        }
    }

    /// <summary>
    /// Sends an Analyze message.
    /// Resets timers/bools.
    /// </summary>
    public void Analyze()
    {
        analyzeCount = false;
        analyzeTimer = 0f;

        MessagingManager.BroadcastMessage(new Message(target.name, name, MessageTypes.Game.ANALYZE));
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

            workInProgress = false;
        }
    }
}
