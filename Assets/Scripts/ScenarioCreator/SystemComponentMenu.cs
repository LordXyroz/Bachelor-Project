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
    private SelectedObject selectedObject;
    private Toggle isEntryPointToggle;

    [Header("Linking to the vulnerabilities menu")]
    private Canvas canvas;
    private VulnerabilityMenu vulnerabilityMenu;


    void Start()
    {
        componentSecurityLevelDropdown = GetComponentInChildren<Dropdown>(true);
        availableSecurityLevels = new List<int>();

        canvas = GetComponentInParent<Canvas>();
        vulnerabilityMenu = canvas.transform.GetComponentInChildren<VulnerabilityMenu>(true);
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

        isEntryPointToggle = canvas.GetComponentInChildren<SystemComponentMenu>().GetComponentInChildren<Toggle>(true);
        if (isEntryPointToggle != null)
        {
            isEntryPointToggle.onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(isEntryPointToggle);
            });
        }
    }


    private void OnEnable()
    {

        isEntryPointToggle = GetComponentInChildren<Toggle>(true);
        if (selectedObject)
            isEntryPointToggle.isOn = selectedObject.selected.gameObject.transform.GetComponent<SystemComponent>().isEntryPoint;
    }


    private void ToggleValueChanged(Toggle entryPoint)
    {
        selectedObject.selected.gameObject.GetComponent<SystemComponent>().isEntryPoint = entryPoint.isOn;
    }


    /// <summary>
    /// Populates the dropdown menu of vulnerabilities
    /// </summary>
    /// <param name="dropdown">The dropdown menu to be updated</param>
    /// <param name="OptionsArray">The list of vulnerabilities</param>
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

        isEntryPointToggle = GetComponentInChildren<Toggle>(true);
        isEntryPointToggle.isOn = selectedObject.selected.gameObject.transform.GetComponent<SystemComponent>().isEntryPoint;
    }


    public void SetMenuInformation(string name, int security)
    {
        componentSecurityLevelDropdown = GetComponentInChildren<Dropdown>(true);
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
    }
}
