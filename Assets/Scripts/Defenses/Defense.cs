using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float timer;
    private bool triggered = false;

    /// <summary>
    /// Automatically starts counting once the gameobject is created.
    /// Triggers an effect once the duration has been met.
    /// </summary>
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration && !triggered)
            Effect();
    }

    /// <summary>
    /// Toggles the trigger so that this doesn't fire every Update call.
    /// Broadcasts a message to potential listeners.
    /// </summary>
    public void Effect()
    {
        triggered = true;
        MessagingManager.BroadcastMessage(new DefenseMessage(target.name, name, MessageTypes.Game.DEFENSE, defenseType));
    }

    /// <summary>
    /// From the IDefenseResponse interface.
    /// 
    /// Listens to a MessageTypes.Events.DEFENSE_RESPONSE
    /// Checks whether self is the target of the message.
    /// Does stuff based on the success or failure of the response.
    /// Destroys the gameobject once the function is done as the defense is done.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void DefenseResponse(SuccessMessage message)
    {
        if (message.targetName == name)
        {
            if (message.success)
                Debug.Log("Defense implemented!");
            else
                Debug.Log("Defense failed");

            Destroy(gameObject);
        }
    }
}
