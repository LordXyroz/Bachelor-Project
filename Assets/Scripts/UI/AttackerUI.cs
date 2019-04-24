using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class AttackerUI : BaseUI
{
    [Header("InfoPanel")]
    public GameObject infoPanelObject;
    public GameObject infoPanelArea;
    public Text targetText;
    public Text probeText;
    public Text analyzeText;
    public Text discoverText;
    public Text exploitableText;
    public Text exploitedText;
    public Text difficultyText;
    public Text nodesText;
    public Text numVulnText;
    public GameObject vulnPrefab;

    [Header("AttackPanel")]
    public GameObject attackPanelObject;
    public GameObject attackPanelArea;
    public GameObject attackButtonPrefab;

    /// <summary>
    /// Builds the list of attack buttons for the attack panel at start based on localization strings
    /// </summary>
    override public void Start()
    {
        List<AttackTypes> attacks = VulnerabilityLogic.GetAllAttacks();
        foreach (var a in attacks)
        {
            GameObject go = Instantiate(attackButtonPrefab, attackPanelArea.transform);

            go.GetComponentInChildren<Text>().text = VulnerabilityLogic.GetAttackString(a);
            go.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<Attacker>().StartAttack((int)a - 1));
            go.GetComponent<Button>().onClick.AddListener(EnableInfoPanel);
            go.AddComponent<PointerHandler>().tooltipInfo = FindObjectOfType<Attacker>().attackPrefabs[(int)a - 1].GetComponent<Attack>().description;
        }

        quitButton.onClick.AddListener(FindObjectOfType<NetworkingManager>().DisconnectFromServer);

        finishScreenButton.onClick.AddListener(FindObjectOfType<NetworkingManager>().DisconnectFromServer);
        finishScreenButton.onClick.AddListener(FinishGame);
    }

    /// <summary>
    /// Checks for escape key, and closes panels/popups based on what is currently open.
    /// </summary>
    override public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onClickMenu.activeSelf)
                onClickMenu.SetActive(false);
            else if (popupWindowObject.activeSelf)
                popupWindowObject.SetActive(false);
            else if (actionsPanelObject.activeSelf)
                EnableStatsPanel();
            else if (infoPanelObject.activeSelf)
                EnableStatsPanel();
            else if (attackPanelObject.activeSelf)
                EnableInfoPanel();
        }
    }

    /// <summary>
    /// Enables the stats panel and disables all other panels
    /// </summary>
    override public void EnableStatsPanel()
    {
        DisableToolTip();
        statsPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        attackPanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    /// <summary>
    /// Enables the info panel and disables all other panels
    /// </summary>
    override public void EnableInfoPanel()
    {
        DisableToolTip();
        infoPanelObject.SetActive(true);

        statsPanelObject.SetActive(false);
        attackPanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    /// <summary>
    /// Enables the attack panel and disables all other panels.
    /// </summary>
    public void EnableAttackPanel()
    {
        DisableToolTip();
        attackPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        statsPanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    /// <summary>
    /// Enables the action panel and disables all other panels.
    /// </summary>
    override public void EnableActionPanel()
    {
        DisableToolTip();
        actionsPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        statsPanelObject.SetActive(false);
        attackPanelObject.SetActive(false);
    }

    /// <summary>
    /// Populates the info panel with new info.
    /// Removes old vulnerability entries with new ones.
    /// </summary>
    /// <param name="info">Class containing the info to be displayed</param>
    override public void PopulateInfoPanel(NodeInfo info)
    {
        foreach (Transform child in infoPanelArea.transform)
            if (child.CompareTag("VulnBox"))
                Destroy(child.gameObject);

        targetText.text = info.displayName;
        probeText.text = (info.beenProbed) ? "Yes" : "No";
        analyzeText.text = (info.beenAnalyzed) ? "Yes" : "No";
        discoverText.text = (info.beenDiscoveredOn) ? "Yes" : "No";

        exploitableText.text = (info.exploitable) ? "Yes" : "No";
        
        exploitedText.transform.parent.gameObject.SetActive(info.exploitable);
        exploitedText.text = (info.exploited) ? "Yes" : "No";

        difficultyText.text = (info.difficulty == -1) ? "Unknown" : info.difficulty.ToString();
        nodesText.text = (info.numOfChildren == -1) ? "Unknown" : info.numOfChildren.ToString();
        numVulnText.text = (info.numOfVulnerabilities == -1) ? "Unknown" : info.numOfVulnerabilities.ToString();

        foreach (var v in info.vulnerabilities)
        {
            GameObject go = Instantiate(vulnPrefab, infoPanelArea.transform);
            Text t = go.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText"));
            t.text = VulnerabilityLogic.GetAttackString(v);
        }

        EnableInfoPanel();
    }

    /// <summary>
    /// Updates all the text for the stats panel
    /// </summary>
    /// <param name="res">Amount of resources</param>
    /// <param name="attackLvl">Current level of attack</param>
    /// <param name="analyzeLvl">Current level of analysis</param>
    /// <param name="discoverLvl">Current level of discovery</param>
    public void UpdateStats(int res, int attackLvl, int analyzeLvl, int discoverLvl)
    {
        statsResourcesText.text = res + " GB";
        statsAtkDefLvlText.text = "Level - " + attackLvl;
        statsAnalyzeLvlText.text = "Level - " + analyzeLvl;
        statsDiscoverLvlText.text = "Level - " + discoverLvl;
    }
}
 