using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI script for the scenario creator.
/// TODO: Move all UI scripting here.
/// </summary>
public class ScenarioCreatorUI : MonoBehaviour
{
    public Button mainMenuButton;
    
    /// <summary>
    /// Sets up button listener.
    /// </summary>
    void Start()
    {
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
    }
}
