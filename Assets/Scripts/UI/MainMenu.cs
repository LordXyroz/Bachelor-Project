using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles UI for main menu scene.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public Slider bgmVolumeSlider;

    /// <summary>
    /// Sets up slider values for audio.
    /// </summary>
    public void Start()
    {
        bgmVolumeSlider.value = FindObjectOfType<AudioManager>().BgmMaxVolume;
        bgmVolumeSlider.onValueChanged.AddListener(FindObjectOfType<AudioManager>().SetBGMVolume);
    }

    /// <summary>
    /// Changes scene to matchmaking scene.
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("Matchmaking");
    }
    
    /// <summary>
    /// Changes scene to DragAndDrop Scene (Scenario creator).
    /// </summary>
    public void ScenarioCreator()
    {
        SceneManager.LoadScene("DragAndDropScene");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
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
