using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagingInterfaces;

/// <summary>
/// The class used for creating various attacks.
/// Implements the IAttackResponse interface to listen to messages.
/// Add this to an empty GameObject and give it a name to create a new Attack.
/// Tweak the various variables in the Inspector tab in the Unity Editor to customize
/// the attack.
/// Should not be in a scene at start. Should be instatiated from player interraction.
/// </summary>
public class Attack : MonoBehaviour, IAttackResponse
{
    [Header("Cost and duration")]
    public int cost;
    public int duration;

    [Header("Attack info")]
    public string description;
    
    [Header("Attack type")]
    public AttackTypes attackType;

    /// <summary>
    /// The target of where the defense should be implemented.
    /// Hidden in Inspector because it should only be changed during gameplay.
    /// </summary>
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public float probability;
    [HideInInspector]
    public string targetDisplayName;
    
    private float timer = 0f;
    private bool triggered = false;

    private AttackerUI uiScript;
    private NetworkingManager networking;

    /// <summary>
    /// Finds the UI script and toggles the progress bar.
    /// </summary>
    public void Start()
    {
        uiScript = FindObjectOfType<AttackerUI>();
        uiScript.ToggleProgressbar(true, "Attacking", description + " on: " + targetDisplayName);
        uiScript.UpdateProgressbar(timer, duration);

        networking = FindObjectOfType<NetworkingManager>();
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

        var rand = Random.Range(0f, 1f);

        networking.SendMessage(new AttackMessage(target.name, name, MessageTypes.Game.Attack, attackType, probability, rand));
    }

    /// <summary>
    /// From the IAttackResponse interface.
    /// 
    /// Listens to a MessageTypes.Events.ATTACK_RESPONSE
    /// Checks whether self is the target of the message.
    /// Does stuff based on the success or failure of the response.
    /// Destroys the gameobject once the function is done as the defense is done.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void AttackResponse(SuccessMessage message)
    {
        if (message.targetName == name)
        {
            Destroy(gameObject);
        }
    }
}
