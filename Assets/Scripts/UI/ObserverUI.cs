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
    public Scrollbar scrollbar;
    private int counter = 0;

    public void AddLog(string msg)
    {
        GameObject go = Instantiate(infoPrefab, infoPanelArea.transform);
        go.GetComponentInChildren<Text>().text = msg;
        counter++;
        if (counter > 12)
            scrollbar.value = 0;
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
