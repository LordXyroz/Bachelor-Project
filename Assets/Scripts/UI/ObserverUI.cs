using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObserverUI : BaseUI
{
    [Header("InfoPanel")]
    public GameObject infoPanelObject;
    public GameObject infoPanelArea;
    public GameObject infoPrefab;
    public ScrollRect scrollrect;
    private int counter = 0;

    /// <summary>
    /// Adds a log entry into the info panel for the observer
    /// </summary>
    /// <param name="msg">String to be displayed</param>
    public void AddLog(string msg)
    {
        GameObject go = Instantiate(infoPrefab, infoPanelArea.transform);
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

    public override void EnableActionPanel()
    {
        throw new System.NotImplementedException();
    }

    public override void EnableInfoPanel()
    {
        throw new System.NotImplementedException();
    }

    public override void EnableStatsPanel()
    {
        throw new System.NotImplementedException();
    }

    public override void PopulateInfoPanel(NodeInfo info)
    {
        throw new System.NotImplementedException();
    }

    public override void Start()
    {
        //throw new System.NotImplementedException();
    }

    public override void Update()
    {
        //throw new System.NotImplementedException();
    }
}
