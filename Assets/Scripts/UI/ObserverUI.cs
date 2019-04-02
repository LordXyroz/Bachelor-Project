using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ObserverUI : BaseUI
{
    [Header("LogPanel")]
    public GameObject logPanelObject;
    public GameObject logPanelArea;
    public GameObject logPrefab;
    public ScrollRect scrollrect;
    private int counter = 0;

    [Header("InfoPanel")]
    public GameObject infoPanelObject;
    public GameObject infoPanelArea;
    public Text targetText;
    public Text difficultyText;
    public Text nodesText;

    public GameObject vulnPrefab;
    public GameObject availDefPrefab;
    public GameObject implDefPrefab;
    public GameObject numVulnPrefab;
    public GameObject numAvailDefPrefab;
    public GameObject numImplDefPrefab;

    /// <summary>
    /// Adds a log entry into the info panel for the observer
    /// </summary>
    /// <param name="msg">String to be displayed</param>
    public void AddLog(string msg)
    {
        GameObject go = Instantiate(logPrefab, logPanelArea.transform);
        go.GetComponentInChildren<Text>().text = msg;

        counter++;
        if (counter > 12)
        {
            /// Force the cavnas to update in order for the scroll rect to update position values
            Canvas.ForceUpdateCanvases();
            /// Sets the scrollrect to the bottom of the list
            scrollrect.verticalNormalizedPosition = 0;
        }
    }

    /// <summary>
    /// Enables the log panel and disables node info panel.
    /// </summary>
    public void EnableLogPanel()
    {
        DisableToolTip();
        logPanelObject.SetActive(true);

        infoPanelObject.SetActive(false);
    }
    
    /// <summary>
    /// Not implemented for observer
    /// </summary>
    public override void EnableActionPanel()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Enables the node info panel, disables log panel.
    /// </summary>
    public override void EnableInfoPanel()
    {
        DisableToolTip();
        infoPanelObject.SetActive(true);

        logPanelObject.SetActive(false);
    }

    /// <summary>
    /// Not implemented for observer
    /// </summary>
    public override void EnableStatsPanel()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Not implemented for observer
    /// </summary>
    /// <param name="info"></param>
    public override void PopulateInfoPanel(NodeInfo info)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Populates the info panel with new info.
    /// Removes old vulnerability entries with new ones.
    /// </summary>
    /// <param name="info">Class containing the info to be displayed</param>
    public void PopulateInfoPanel(ObserverNodeInfo info)
    {
        foreach (Transform child in infoPanelArea.transform)
            if (child.CompareTag("VulnBox"))
                Destroy(child.gameObject);

        targetText.text = info.displayName;

        difficultyText.text = (info.difficulty == -1) ? "Unknown" : info.difficulty.ToString();
        nodesText.text = (info.numOfChildren == -1) ? "Unknown" : info.numOfChildren.ToString();

        GameObject numVuln = Instantiate(numVulnPrefab, infoPanelArea.transform);
        numVuln.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText")).text = info.numOfVulnerabilities.ToString();

        foreach (var v in info.vulnerabilities)
        {
            GameObject go = Instantiate(vulnPrefab, infoPanelArea.transform);
            Text t = go.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText"));
            t.text = VulnerabilityLogic.GetAttackString(v);
        }

        GameObject numAvailDef = Instantiate(numAvailDefPrefab, infoPanelArea.transform);
        numAvailDef.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText")).text = info.availableDefenses.Count.ToString();

        foreach (var def in info.availableDefenses)
        {
            GameObject go = Instantiate(availDefPrefab, infoPanelArea.transform);
            Text t = go.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText"));
            t.text = VulnerabilityLogic.GetDefenseString(def);
        }

        GameObject numImplDef = Instantiate(numImplDefPrefab, infoPanelArea.transform);
        numImplDef.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText")).text = info.implementedDefenses.Count.ToString();

        foreach (var def in info.implementedDefenses)
        {
            GameObject go = Instantiate(availDefPrefab, infoPanelArea.transform);
            Text t = go.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText"));
            t.text = VulnerabilityLogic.GetDefenseString(def);
        }
        
        EnableInfoPanel();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Start()
    {
        quitButton.onClick.AddListener(FindObjectOfType<NetworkingManager>().DisconnectFromServer); ;
    }

    /// <summary>
    /// Unused
    /// </summary>
    public override void Update()
    {
        //throw new System.NotImplementedException();
    }
}
