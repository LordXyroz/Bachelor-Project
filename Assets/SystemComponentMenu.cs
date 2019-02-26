using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script put on the right-click menu for system components
/// </summary>

public class SystemComponentMenu : MonoBehaviour
{
    [Header("Populating the component menu")]
    public TMP_Text componentMenuName;
    public List<String> displayedVulnerabilities;
    private Dropdown availableVulnerabilitiesDropdown;
    private List<int> availableVulnerabilities;


    void Start()
    {
        availableVulnerabilitiesDropdown = GetComponentInChildren<Dropdown>();
        availableVulnerabilities = new List<int>();
        availableVulnerabilities.Add(123);
        availableVulnerabilities.Add(2);
        availableVulnerabilities.Add(4563);
        availableVulnerabilities.Add(8989898);

        PopulateVulnerabilityDropdown(availableVulnerabilitiesDropdown, availableVulnerabilities);
    }


    private void PopulateVulnerabilityDropdown(Dropdown dropdown, List<int> OptionsArray)
    {
        displayedVulnerabilities = new List<string>();
        foreach (int option in OptionsArray)
        {
            displayedVulnerabilities.Add(option.ToString());
        }

        if (dropdown != null)
        {
            dropdown.ClearOptions();
        }
        dropdown.AddOptions(displayedVulnerabilities);
    }


    public void SetMenuName(string name)
    {
        Debug.Log("Component name: " + name);
        componentMenuName.text = name;
    }


    public void UpdatePosition(Vector2 pos)
    {
        this.gameObject.GetComponent<RectTransform>().transform.position = pos;
    }
}
