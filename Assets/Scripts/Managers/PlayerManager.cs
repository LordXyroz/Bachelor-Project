using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// Enum for all playertypes possible in the game
    /// </summary>
    public enum PlayerType
    {
        Observer,
        Attacker,
        Defender
    }

    [Header("Game and player")]
    [SerializeField]
    private PlayerType playerType;
    [SerializeField]
    private BaseUI uiScript;

    [Header("GameObjects")]
    public GameObject attackerUI;
    public GameObject defenderUI;
    public GameObject observerUI;
    public GameObject attacker;
    public GameObject defender;
    public GameObject observer;

    /// <summary>
    /// Initializes values and enables gameobjects based on playertype
    /// </summary>
    void Awake()
    {
        playerType = FindObjectOfType<NetworkingManager>().playerType;

        if (playerType == PlayerType.Attacker)
        {
            attackerUI.SetActive(true);
            defenderUI.SetActive(false);
            observerUI.SetActive(false);

            attacker.SetActive(true);
            defender.SetActive(false);
            observer.SetActive(false);

            uiScript = FindObjectOfType<AttackerUI>();
        }
        else if (playerType == PlayerType.Defender)
        {
            attackerUI.SetActive(false);
            defenderUI.SetActive(true);
            observerUI.SetActive(false);

            attacker.SetActive(false);
            defender.SetActive(true);
            observer.SetActive(false);

            uiScript = FindObjectOfType<DefenderUI>();
        }
        else if (playerType == PlayerType.Observer)
        {
            attackerUI.SetActive(false);
            defenderUI.SetActive(false);
            observerUI.SetActive(true);

            attacker.SetActive(false);
            defender.SetActive(false);
            observer.SetActive(true);

            uiScript = FindObjectOfType<ObserverUI>();
        }
    }

    /// <summary>
    /// Gets which uiscript is active for the player
    /// </summary>
    /// <returns>UI script which is active</returns>
    public BaseUI GetUIScript()
    {
        return uiScript;
    }
    
    /// <summary>
    /// Gets the player type currently playing.
    /// </summary>
    /// <returns>Playertpe currently playing</returns>
    public PlayerType GetPlayerType()
    {
        return playerType;
    }
}
