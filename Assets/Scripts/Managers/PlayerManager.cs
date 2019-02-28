using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
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
    public GameObject attacker;
    public GameObject defender;
    public GameObject observer;

    // Start is called before the first frame update
    void Awake()
    {
        playerType = FindObjectOfType<NetworkingManager>().playerType;

        if (playerType == PlayerType.Attacker)
        {
            uiScript = FindObjectOfType<AttackerUI>();
            defenderUI.SetActive(false);

            attacker.SetActive(true);
            defender.SetActive(false);
            observer.SetActive(false);
        }
        else if (playerType == PlayerType.Defender)
        {
            uiScript = FindObjectOfType<DefenderUI>();
            attackerUI.SetActive(false);

            attacker.SetActive(false);
            defender.SetActive(true);
            observer.SetActive(false);
        }
        else if (playerType == PlayerType.Observer)
        {
            attackerUI.SetActive(false);
            defenderUI.SetActive(false);

            attacker.SetActive(false);
            defender.SetActive(false);
            observer.SetActive(true);
        }
    }

    public BaseUI GetUIScript()
    {
        return uiScript;
    }

    public bool IsAttacker()
    {
        return playerType == PlayerType.Attacker;
    }
}
