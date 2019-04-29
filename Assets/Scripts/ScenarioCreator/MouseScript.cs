using UnityEngine;

/// <summary>
/// Simple mouse script for handling some click functionality.
/// </summary>
public class MouseScript : MonoBehaviour
{
    [Header("Scripts")]
    public InformationColumn infoScript;

    [Header("Objects")]
    public GameObject currentSelection;

    /// <summary>
    /// Finds the <see cref="InformationColumn"/> script.
    /// </summary>
    public void Start()
    {
        infoScript = FindObjectOfType<InformationColumn>();
    }

    /// <summary>
    /// Called on mouse click.
    /// Sets selection and updates information.
    /// </summary>
    /// <param name="selected">The selected object</param>
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
                infoScript.ClearInformationColumn();
                infoScript.PopulateInformationColumn(
                    sysComp.componentName,
                    sysComp.componentVulnerabilities,
                    sysComp.securityLevel,
                    sysComp.isEntryPoint);
            }
        }
    }

    /// <summary>
    /// Sets selection to null and clears information column.
    /// </summary>
    public void DeselectObject()
    {
        currentSelection = null;
        infoScript.ClearInformationColumn();
    }
}
