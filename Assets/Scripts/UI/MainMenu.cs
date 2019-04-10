using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Slider bgmVolumeSlider;

    public void Start()
    {
        bgmVolumeSlider.value = FindObjectOfType<AudioManager>().BgmMaxVolume;
        bgmVolumeSlider.onValueChanged.AddListener(FindObjectOfType<AudioManager>().SetBGMVolume);
    }

    // Loading the game screen
    public void LoadGame()
    {
        SceneManager.LoadScene("Matchmaking");
    }
    
    public void ScenarioCreator()
    {
        SceneManager.LoadScene("DragAndDropScene");
    }

    // Quitting the application
    public void QuitGame()
    {
        Debug.Log("GAME QUIT");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    
}
