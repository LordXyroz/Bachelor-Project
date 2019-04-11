using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    [Header("Scripts")]
    public InformationColumn infoScript;

    [Header("Objects")]
    public GameObject currentSelection;

    public void Start()
    {
        infoScript = FindObjectOfType<InformationColumn>();
    }

    public void SelectObject(GameObject selected)
    {
        if (currentSelection == selected)
        {
            currentSelection = null;
            infoScript.ClearInformationColumn();
        }
        else
        {
            currentSelection = selected;
            SystemComponent sysComp = currentSelection.GetComponent<SystemComponent>();
            if (sysComp != null)
            {
                infoScript.PopulateInformationColumn(
                    sysComp.componentName,
                    sysComp.componentVulnerabilities,
                    sysComp.securityLevel,
                    sysComp.isEntryPoint);
            }
        }
    }

    public void DeselectObject()
    {
        currentSelection = null;
        infoScript.ClearInformationColumn();
    }
}
