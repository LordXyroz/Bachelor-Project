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
    public List<String> displayedSecurityLevels;
    private Dropdown componentSecurityLevelDropdown;
    private List<int> availableSecurityLevels;
    public SelectedObject selectedObject;

    [Header("Linking to the vulnerabilities menu")]
    private Canvas canvas;
    public VulnerabilityMenu vulnerabilityMenu;


    void Start()
    {
        componentSecurityLevelDropdown = GetComponentInChildren<Dropdown>();
        availableSecurityLevels = new List<int>();

        canvas = GetComponentInParent<Canvas>();
        vulnerabilityMenu = canvas.transform.Find("VulnerabilityMenu").GetComponent<VulnerabilityMenu>();
        selectedObject = canvas.transform.Find("Scripts").GetComponent<SelectedObject>();

        for (int i = 1; i <= 5; i++)
        {
            availableSecurityLevels.Add(i);
        }
        PopulateVulnerabilityDropdown(componentSecurityLevelDropdown, availableSecurityLevels);

        componentSecurityLevelDropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(componentSecurityLevelDropdown);
        });
    }


    private void PopulateVulnerabilityDropdown(Dropdown dropdown, List<int> OptionsArray)
    {
        displayedSecurityLevels = new List<string>();
        foreach (int option in OptionsArray)
        {
            displayedSecurityLevels.Add(option.ToString());
        }

        if (dropdown != null)
        {
            dropdown.ClearOptions();
        }
        dropdown.AddOptions(displayedSecurityLevels);
    }


    public void SetMenuInformation(string name, int security)
    {
        componentMenuName.text = name;
        componentSecurityLevelDropdown.value = security - 1;    // compensate for no security level 0 (lvl 1-5)
    }


    private void DropdownValueChanged(Dropdown dropdown)
    {
        componentSecurityLevelDropdown.value = dropdown.value;
        selectedObject.selected.gameObject.GetComponent<SystemComponent>().securityLevel = dropdown.value + 1;
    }


    public void UpdatePosition(Vector2 pos)
    {
        this.gameObject.GetComponent<RectTransform>().transform.position = pos;
    }


    public void OpenVulnerabilityMeny()
    {
        vulnerabilityMenu.gameObject.SetActive(true);
        vulnerabilityMenu.PopulateVulnerabilityMenu(componentMenuName.text);
    }
}
