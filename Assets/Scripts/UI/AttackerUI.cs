﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class AttackerUI : MonoBehaviour
{
    [Header("Progressbar")]
    public GameObject progressbarObject;
    public Image progressbar;
    public Text progressbarTitle;
    public Text progressbarText;

    [Header("OnClickMenu")]
    public GameObject onClickMenu;

    [Header("InfoPanel")]
    public GameObject infoPanelObject;
    public GameObject infoPanelArea;
    public Text targetText;
    public Text probeText;
    public Text analyzeText;
    public Text discoverText;
    public Text difficultyText;
    public Text nodesText;
    public Text numVulnText;
    public GameObject vulnPrefab;

    [Header("AttackPanel")]
    public GameObject attackPanelObject;
    public GameObject attackPanelArea;
    public GameObject attackButtonPrefab;

    [Header("Popup window")]
    public GameObject popupWindowObject;
    public Text popupWindowTitle;
    public Text popupWindowText;

    [Header("Stats window")]
    public GameObject statsPanelObject;
    public Text statsResourcesText;
    public Text statsAttackLvlText;
    public Text statsAnalyzeLvlText;
    public Text statsDiscoverLvlText;

    [Header("Actions window")]
    public GameObject actionsPanelObject;

    [Header("Tooltip")]
    public GameObject tooltipObject;
    public RectTransform tooltipFrameTransform;
    public Text tooltipText;
    private const float refHeight = 1080;
    private const float refWidth = 1920;

    /// <summary>
    /// Builds the list of attack buttons for the attack panel at start based on localization strings
    /// </summary>
    public void Start()
    {
        List<AttackTypes> attacks = VulnerabilityPairings.GetAllAttacks();
        foreach (var a in attacks)
        {
            GameObject go = Instantiate(attackButtonPrefab, attackPanelArea.transform);

            go.GetComponentInChildren<Text>().text = VulnerabilityPairings.GetAttackString(a);
            go.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<Attacker>().StartAttack((int)a - 1));
            go.GetComponent<Button>().onClick.AddListener(EnableInfoPanel);
            go.AddComponent<PointerHandler>().tooltipInfo = FindObjectOfType<Attacker>().attackPrefabs[(int)a - 1].GetComponent<Attack>().description;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onClickMenu.activeSelf)
                onClickMenu.SetActive(false);
            else if (popupWindowObject.activeSelf)
                popupWindowObject.SetActive(false);
            else if (infoPanelObject.activeSelf)
                EnableStatsPanel();
            else if (attackPanelObject.activeSelf)
                EnableInfoPanel();
        }
    }

    /// <summary>
    /// Fills the progressbar an amount based on current and max value
    /// </summary>
    /// <param name="value">Current value</param>
    /// <param name="max">Max value</param>
    public void UpdateProgressbar(float value, float max)
    {
        progressbar.fillAmount = value / max;
    }

    /// <summary>
    /// Enables/Disables the progressbar popup, and sets the texts to display.
    /// </summary>
    /// <param name="toggle">Whether to enable or disable</param>
    /// <param name="title">Title text</param>
    /// <param name="text">Info text</param>
    public void ToggleProgressbar(bool toggle, string title, string text)
    {
        progressbarTitle.text = title;
        progressbarText.text = text;

        progressbarObject.SetActive(toggle);
    }

    /// <summary>
    /// Toggles the OnClick menu, and moves it to a target position
    /// </summary>
    /// <param name="toggle">Whether to enable or disable</param>
    /// <param name="pos">Position to move to</param>
    public void ToggleOnClickMenu(bool toggle, Vector3 pos)
    {
        if (popupWindowObject.activeSelf || progressbarObject.activeSelf)
            return;

        onClickMenu.SetActive(toggle);
        onClickMenu.GetComponent<RectTransform>().localPosition = pos;
    }

    /// <summary>
    /// Disables the OnClick menu
    /// </summary>
    public void DisableOnClickMenu()
    {
        onClickMenu.SetActive(false);
    }
    
    /// <summary>
    /// Populates the info panel with new info.
    /// Removes old vulnerability entries with new ones.
    /// </summary>
    /// <param name="info">Class containing the info to be displayed</param>
    public void PopulateInfoPanel(NodeInfo info)
    {
        foreach (Transform child in infoPanelArea.transform)
            if (child.CompareTag("VulnBox"))
                Destroy(child.gameObject);

        targetText.text = info.component.name;
        probeText.text = (info.beenProbed) ? "Yes" : "No";
        analyzeText.text = (info.beenAnalyzed) ? "Yes" : "No";
        discoverText.text = (info.beenDiscoveredOn) ? "Yes" : "No";

        difficultyText.text = (info.difficulty == -1) ? "Unknown" : info.difficulty.ToString();
        nodesText.text = (info.numOfChildren == -1) ? "Unknown" : info.numOfChildren.ToString();
        numVulnText.text = (info.numOfVulnerabilities == -1) ? "Unknown" : info.numOfVulnerabilities.ToString();

        foreach (var v in info.vulnerabilities)
        {
            GameObject go = Instantiate(vulnPrefab, infoPanelArea.transform);
            Text t = go.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText"));
            t.text = VulnerabilityPairings.GetAttackString(v);
        }

        EnableInfoPanel();
    }

    public void TogglePopupWindow(bool toggle, string title, string text)
    {
        popupWindowTitle.text = title;
        popupWindowText.text = text;

        popupWindowObject.SetActive(toggle);
    }

    public void EnableStatsPanel()
    {
        DisableToolTip();
        statsPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        attackPanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    public void EnableInfoPanel()
    {
        DisableToolTip();
        infoPanelObject.SetActive(true);

        statsPanelObject.SetActive(false);
        attackPanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    /// <summary>
    /// Enables the attack panel and disables info panel.
    /// </summary>
    public void EnableAttackPanel()
    {
        DisableToolTip();
        attackPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        statsPanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    public void EnableActionPanel()
    {
        DisableToolTip();
        actionsPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        statsPanelObject.SetActive(false);
        attackPanelObject.SetActive(false);
    }

    public void DisablePopupWindow()
    {
        popupWindowObject.SetActive(false);
    }

    public void EnableToolTip(string text, Vector2 pos, Rect rect)
    {
        tooltipText.text = text;

        float xOffset = (rect.width / 2f) * (Screen.width /refWidth);
        float yOffset = rect.height * (Screen.height / refHeight);

        Debug.Log("Height: " + Screen.height + " , Width: " + Screen.width);
        Debug.Log("Height ratio: " + Screen.height / refHeight + " , Width ratio: " + Screen.width / refWidth);

        if (pos.x >= Screen.width / 2f)
        {
            xOffset = (xOffset * -1);
            tooltipFrameTransform.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            tooltipFrameTransform.pivot = new Vector2(0f, 0.5f);
        }
        
        Vector2 newPos = new Vector2(pos.x + xOffset, pos.y - yOffset);
        
        tooltipObject.GetComponent<RectTransform>().position = newPos;
        tooltipObject.SetActive(true);
    }

    public void DisableToolTip()
    {
        tooltipObject.SetActive(false);
    }

    public void UpdateStats(int res, int attackLvl, int analyzeLvl, int discoverLvl)
    {
        statsResourcesText.text = res + " GB";
        statsAttackLvlText.text = "Level - " + attackLvl;
        statsAnalyzeLvlText.text = "Level - " + analyzeLvl;
        statsDiscoverLvlText.text = "Level - " + discoverLvl;
    }
}
 