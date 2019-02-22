using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenderUI : BaseUI
{
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
    
    override public void PopulateInfoPanel(NodeInfo info)
    {
        throw new System.NotImplementedException();
    }

    override public void UpdateProgressbar(float value, float max)
    {
        throw new System.NotImplementedException();
    }

    override public void UpdateStats(int res, int defLvl, int analyzeLvl, int discoverLvl)
    {
        throw new System.NotImplementedException();
    }
    
}
