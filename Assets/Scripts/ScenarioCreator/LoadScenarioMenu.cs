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
    private Canvas canvas;
    private SaveMenu saveMenu;
    private TMP_Dropdown scenarioDropdown;
    private string file;

    private string _SaveFileDirectory;


    void Start()
    {
        _SaveFileDirectory = Application.dataPath + "/Savefiles";

        canvas = FindObjectOfType<Canvas>();
        saveMenu = canvas.transform.GetComponentInChildren<SaveMenu>(true);
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
        string[] info = Directory.GetFiles(_SaveFileDirectory, "*.json");

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
        string filepath = Path.Combine(_SaveFileDirectory, file);
        if (loadScenario != null)
        {
            loadScenario.fileName = file;
            saveMenu.filename = file;
            loadScenario.LoadGame(filepath);
        }
        else
        {
            FindObjectOfType<NetworkingManager>().LoadSaveFile(filepath);
        }
    }
}

