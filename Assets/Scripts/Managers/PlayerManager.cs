using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private enum PlayerType
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

    // Start is called before the first frame update
    void Awake()
    {
        if (playerType == PlayerType.Attacker)
        {
            uiScript = FindObjectOfType<AttackerUI>();
            defenderUI.SetActive(false);
        }
        else if (playerType == PlayerType.Defender)
        {
            uiScript = FindObjectOfType<DefenderUI>();
            attackerUI.SetActive(false);
        }
    }

    public BaseUI GetUIScript()
    {
        return uiScript;
    }
}
