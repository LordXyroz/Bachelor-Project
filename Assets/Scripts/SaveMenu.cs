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
    public int defenderDefenseLevel;
    public int defenderDiscoveryLevel;

    private TMP_Dropdown attackerAttackLevelDropdown;
    private TMP_Dropdown attackerDiscoveryLevelDropdown;
    private TMP_Dropdown attackerAnalysisLevelDropdown;
    private TMP_Dropdown defenderDefenseLevelDropdown;
    private TMP_Dropdown defenderDiscoveryLevelDropdown;
    private List<TMP_Dropdown> dropdownList = new List<TMP_Dropdown>();


    // Start is called before the first frame update
    void Start()
    {
        saveScenario = FindObjectOfType<SaveScenario>();

        inputFilename = GetComponentInChildren<TMP_InputField>();

        attackerAttackLevelDropdown = this.gameObject.transform.Find("AttackerAttackLevel").GetComponentInChildren<TMP_Dropdown>();
        attackerDiscoveryLevelDropdown = this.gameObject.transform.Find("AttackerDiscoveryLevel").GetComponentInChildren<TMP_Dropdown>();
        attackerAnalysisLevelDropdown = this.gameObject.transform.Find("AttackerAnalysisLevel").GetComponentInChildren<TMP_Dropdown>();
        defenderDefenseLevelDropdown = this.gameObject.transform.Find("DefenderDefenseLevel").GetComponentInChildren<TMP_Dropdown>();
        defenderDiscoveryLevelDropdown = this.gameObject.transform.Find("DefenderDiscoveryLevel").GetComponentInChildren<TMP_Dropdown>();

        dropdownList.Add(attackerAttackLevelDropdown);
        dropdownList.Add(attackerDiscoveryLevelDropdown);
        dropdownList.Add(attackerAnalysisLevelDropdown);
        dropdownList.Add(defenderDefenseLevelDropdown);
        dropdownList.Add(defenderDiscoveryLevelDropdown);

        PopulateDropdownMenu(dropdownList);
    }

    public void SaveButtonPressed()
    {
        currentDropdownValue = attackerAttackLevelDropdown.value;
        filename = inputFilename.text;

        attackerAttackLevel = attackerAttackLevelDropdown.value + 1;
        attackerDiscoveryLevel = attackerDiscoveryLevelDropdown.value + 1;
        attackerAnalysisLevel = attackerAnalysisLevelDropdown.value + 1;
        defenderDefenseLevel = defenderDefenseLevelDropdown.value + 1;
        defenderDiscoveryLevel = defenderDiscoveryLevelDropdown.value + 1;

        saveScenario.SaveCurrentScenario();
    }

    private void PopulateDropdownMenu(List<TMP_Dropdown> dropdownList)
    {
        if (filename != null)
        {
            inputFilename.text = filename;
        }

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
    }
}
