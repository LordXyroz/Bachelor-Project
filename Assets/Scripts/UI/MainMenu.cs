using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loading the game screen
    public void LoadGame()
    {
        SceneManager.LoadScene("Matchmaking");
    }
    
    public void ScenarioCreator()
    {
        // Load scenario creator
    }

    // Quitting the application
    public void QuitGame()
    {
        Debug.Log("GAME QUIT");
        Application.Quit();
    }
}
