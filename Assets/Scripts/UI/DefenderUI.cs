using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DefenderUI : BaseUI
{
    [Header("InfoPanel")]
    public GameObject infoPanelObject;
    public GameObject infoPanelArea;
    public Text targetText;
    public Text probeText;
    public Text analyzeText;
    public Text difficultyText;
    public Text nodesText;
    public Text numVulnText;
    public GameObject vulnPrefab;

    [Header("DefensePanel")]
    public GameObject defensePanelObject;
    public GameObject defensePanelArea;
    public GameObject defensButtonPrefab;

    // Start is called before the first frame update
    override public void Start()
    {
        List<DefenseTypes> defenses = VulnerabilityPairings.GetAllDefenses();
        foreach (var d in defenses)
        {
            GameObject go = Instantiate(defensButtonPrefab, defensePanelArea.transform);

            go.GetComponentInChildren<Text>().text = VulnerabilityPairings.GetDefenseString(d);
            go.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<Defender>().StartDefense((int)d - 1));
            go.GetComponent<Button>().onClick.AddListener(EnableInfoPanel);
            go.AddComponent<PointerHandler>().tooltipInfo = FindObjectOfType<Defender>().defensePrefabs[(int)d - 1].GetComponent<Defense>().description;
        }
    }

    // Update is called once per frame
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
            else if (defensePanelObject.activeSelf)
                EnableInfoPanel();
        }
    }

    override public void EnableStatsPanel()
    {
        DisableToolTip();
        statsPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        defensePanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    override public void EnableInfoPanel()
    {
        DisableToolTip();
        infoPanelObject.SetActive(true);

        statsPanelObject.SetActive(false);
        defensePanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    public void EnableDefensePanel()
    {
        DisableToolTip();
        defensePanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        statsPanelObject.SetActive(false);
        actionsPanelObject.SetActive(false);
    }

    override public void EnableActionPanel()
    {
        DisableToolTip();
        actionsPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
        statsPanelObject.SetActive(false);
        defensePanelObject.SetActive(false);
    }
    
    public void UpdateStats(int res, int defLvl, int analyzeLvl)
    {
        statsResourcesText.text = res + " GB";
        statsAtkDefLvlText.text = "Level - " + defLvl;
        statsAnalyzeLvlText.text = "Level - " + analyzeLvl;
    }

    public override void PopulateInfoPanel(NodeInfo info)
    {
        foreach (Transform child in infoPanelArea.transform)
            if (child.CompareTag("VulnBox"))
                Destroy(child.gameObject);

        targetText.text = info.component.name;
        probeText.text = (info.beenProbed) ? "Yes" : "No";
        analyzeText.text = (info.beenAnalyzed) ? "Yes" : "No";

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
}
