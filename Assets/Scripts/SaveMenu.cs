﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// This is the script for the save menu
/// </summary>

public class SaveMenu : MonoBehaviour
{

    [Header("Data for saving scenario")]
    public string filename;
    private TMP_InputField inputFilename;
    public int currentDropdownValue;
    private SaveScenario saveScenario;

    [Header("The stats for attacker and defender")]
    public int maxLevelOfStat = 3;
    public int attackerAttackLevel;
    public int attackerDiscoveryLevel;
    public int attackerAnalysisLevel;
    public int attackerResources;

    public int defenderDefenseLevel;
    public int defenderDiscoveryLevel;
    public int defenderResources;

    [Header("Getting the save data from the running instance")]
    private TMP_Dropdown attackerAttackLevelDropdown;
    private TMP_Dropdown attackerDiscoveryLevelDropdown;
    private TMP_Dropdown attackerAnalysisLevelDropdown;
    private TMP_InputField attackerResourcesInput;

    private TMP_Dropdown defenderDefenseLevelDropdown;
    private TMP_Dropdown defenderDiscoveryLevelDropdown;
    private TMP_InputField defenderResourcesInput;
    private List<TMP_Dropdown> dropdownList = new List<TMP_Dropdown>();


    // Start is called before the first frame update
    void Start()
    {
        saveScenario = FindObjectOfType<SaveScenario>();

        inputFilename = GetComponentInChildren<TMP_InputField>();

        attackerAttackLevelDropdown = this.gameObject.transform.Find("AttackerAttackLevel").GetComponentInChildren<TMP_Dropdown>();
        attackerDiscoveryLevelDropdown = this.gameObject.transform.Find("AttackerDiscoveryLevel").GetComponentInChildren<TMP_Dropdown>();
        attackerAnalysisLevelDropdown = this.gameObject.transform.Find("AttackerAnalysisLevel").GetComponentInChildren<TMP_Dropdown>();
        attackerResourcesInput = this.gameObject.transform.Find("AttackerResources").GetComponentInChildren<TMP_InputField>();

        defenderDefenseLevelDropdown = this.gameObject.transform.Find("DefenderDefenseLevel").GetComponentInChildren<TMP_Dropdown>();
        defenderDiscoveryLevelDropdown = this.gameObject.transform.Find("DefenderDiscoveryLevel").GetComponentInChildren<TMP_Dropdown>();
        defenderResourcesInput = this.gameObject.transform.Find("DefenderResources").GetComponentInChildren<TMP_InputField>();

        dropdownList.Add(attackerAttackLevelDropdown);
        dropdownList.Add(attackerDiscoveryLevelDropdown);
        dropdownList.Add(attackerAnalysisLevelDropdown);
        dropdownList.Add(defenderDefenseLevelDropdown);
        dropdownList.Add(defenderDiscoveryLevelDropdown);

        PopulateSaveMenu(dropdownList);
    }


    public void SaveButtonPressed()
    {
        //attackerResources = int.Parse(attackerResourcesInput.text);

        try
        {
            attackerResources = int.Parse(attackerResourcesInput.text);
        }
        catch (FormatException)
        {
            Console.WriteLine("{0}: Bad Format for attacker resources", attackerResources);
        }
        catch (OverflowException)
        {
            Console.WriteLine("{0}: Overflow for attacker resources", attackerResources);
        }

        try
        {
            defenderResources = int.Parse(defenderResourcesInput.text);
        }
        catch (FormatException)
        {
            Console.WriteLine("{0}: Bad Format for defender resources", defenderResources);
        }
        catch (OverflowException)
        {
            Console.WriteLine("{0}: Overflow for defender resources", defenderResources);
        }


        currentDropdownValue = attackerAttackLevelDropdown.value;
        filename = inputFilename.text;

        attackerAttackLevel = attackerAttackLevelDropdown.value + 1;
        attackerDiscoveryLevel = attackerDiscoveryLevelDropdown.value + 1;
        attackerAnalysisLevel = attackerAnalysisLevelDropdown.value + 1;
        defenderDefenseLevel = defenderDefenseLevelDropdown.value + 1;
        defenderDiscoveryLevel = defenderDiscoveryLevelDropdown.value + 1;

        saveScenario.SaveCurrentScenario();
    }

    private void PopulateSaveMenu(List<TMP_Dropdown> dropdownList)
    {
        if (filename != null)
        {
            inputFilename.text = filename;
        }

        attackerResourcesInput.text = attackerResources.ToString();
        defenderResourcesInput.text = defenderResources.ToString();

        foreach (TMP_Dropdown dropdown in dropdownList)
        {
            List<string> levels = new List<string>();

            for (int i = 1; i <= maxLevelOfStat; i++)
            {
                levels.Add(i.ToString());
            }

            if (dropdown != null)
            {
                dropdown.ClearOptions();
            }
            dropdown.AddOptions(levels);
        }
        attackerAttackLevelDropdown.value = attackerAttackLevel - 1;
        attackerDiscoveryLevelDropdown.value = attackerDiscoveryLevel - 1;
        attackerAnalysisLevelDropdown.value = attackerAnalysisLevel - 1;
        defenderDefenseLevelDropdown.value = defenderDefenseLevel - 1;
        defenderDiscoveryLevelDropdown.value = defenderDiscoveryLevel - 1;
    }
}