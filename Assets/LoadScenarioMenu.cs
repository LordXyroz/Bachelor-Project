using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// Scipt for the scenario loading menu (LoadScenarioMenu)
/// </summary>
public class LoadScenarioMenu : MonoBehaviour
{
    private LoadScenario loadScenario;
    private TMP_Dropdown scenarioDropdown;
    private string file = "savefile.json";


    void Start()
    {
        loadScenario = FindObjectOfType<LoadScenario>();
        scenarioDropdown = GetComponentInChildren<TMP_Dropdown>(true);
        PopulatescenarioDropdown(scenarioDropdown);

        scenarioDropdown.onValueChanged.AddListener(delegate
            {
                DropdownValueChanged(scenarioDropdown);
            });
    }


    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        scenarioDropdown.value = dropdown.value;
    }


    private void PopulatescenarioDropdown(TMP_Dropdown dropdown)
    {
        List<string> loadfiles = new List<string>();
        string[] info = Directory.GetFiles(loadScenario.directoryPath, "*.json");

        foreach (string f in info)
        {
            loadfiles.Add(Path.GetFileName(f));
        }
        if (dropdown != null)
        {
            dropdown.ClearOptions();
        }
        dropdown.AddOptions(loadfiles);
    }


    public void LoadScenario()
    {
        file = scenarioDropdown.options[scenarioDropdown.value].text;
        string filepath = Path.Combine(loadScenario.directoryPath, file);
        Debug.Log("Loading data from file: " + file);
        loadScenario.LoadGame(filepath);
    }
}

