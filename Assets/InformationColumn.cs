using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationColumn : MonoBehaviour
{

    private Canvas canvas;
    private TMP_Text currentObjectName;
    private TMP_Text vulnerabilityHeader;
    private TMP_Text currentVulnerabilities;


    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        currentObjectName = canvas.transform.Find("InformationColumnRight").transform.Find("ComponentNameText").GetComponent<TMP_Text>();
        vulnerabilityHeader = canvas.transform.Find("InformationColumnRight").transform.Find("VulnerabilityListText").GetComponent<TMP_Text>();
        currentVulnerabilities = canvas.transform.Find("InformationColumnRight").transform.Find("VulnerabilitiesText").GetComponent<TMP_Text>();
        ClearInformationColumn();
    }


    public void PopulateInformationColumn(string name, List<string> vulnerabilities)
    {
        currentObjectName.text = name;
        vulnerabilityHeader.text = "Vulnerabilities";

        foreach (string vulnerability in vulnerabilities)
        {
            currentVulnerabilities.text += vulnerability + "\n";
        }
    }


    public void ClearInformationColumn()
    {
        currentObjectName.text = "";
        vulnerabilityHeader.text = "";
        currentVulnerabilities.text = "";
    }
}
