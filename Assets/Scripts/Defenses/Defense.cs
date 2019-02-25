using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagingInterfaces;

/// <summary>
/// The class used for creating various defenses.
/// Implements the IDefenseResponse interface to listen to messages.
/// Add this to an empty GameObject and give it a name to create a new Defense.
/// Tweak the various variables in the Inspector tab in the Unity Editor to customize
/// the defense.
/// Should not be in a scene at start. Should be instatiated from player interraction.
/// </summary>
public class Defense : MonoBehaviour, IDefenseResponse
{
    [Header("Cost and duration")]
    public int cost;
    public int duration;

    [Header("Defense info")]
    public string description;

    [Header("Defense type")]
    public DefenseTypes defenseType;

    /// <summary>
    /// The target of where the defense should be implemented.
    /// Hidden in Inpsector because it should only be changed during gameplay.
    /// </summary>
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public float probability;

    private float timer;
    private bool triggered = false;

    private DefenderUI uiScript;

    public void Start()
    {
        uiScript = FindObjectOfType<DefenderUI>();
        uiScript.ToggleProgressbar(true, "Defending", description + " on: " + target.name);
        uiScript.UpdateProgressbar(timer, duration);
    }

    /// <summary>
    /// Automatically starts counting once the gameobject is created.
    /// Triggers an effect once the duration has been met.
    /// </summary>
    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration && !triggered)
            Effect();

        uiScript.UpdateProgressbar(timer, duration);
    }

    /// <summary>
    /// Toggles the trigger so that this doesn't fire every Update call.
    /// Broadcasts a message to potential listeners.
    /// </summary>
    public void Effect()
    {
        triggered = true;
        MessagingManager.BroadcastMessage(new DefenseMessage(target.name, name, MessageTypes.Game.Defense, defenseType));
    }

    /// <summary>
    /// From the IDefenseResponse interface.
    /// 
    /// Listens to a MessageTypes.Events.DEFENSE_RESPONSE
    /// Checks whether self is the target of the message.
    /// Destroys the gameobject once the function is done as the defense is done.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void DefenseResponse(SuccessMessage message)
    {
        if (message.targetName == name)
        {
            Destroy(gameObject);
        }
    }
}
