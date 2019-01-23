using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{


    // Loading the game screen
    public void LoadGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("GameStart");
    }

    // Quitting the application
    public void QuitGame()
    {
        Debug.Log("GAME QUIT");
        Application.Quit();
    }
}
