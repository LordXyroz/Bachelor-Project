using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for quiting current scene, used in scenario creator.
/// </summary>
public class Quit : MonoBehaviour
{
    /// <summary>
    /// Quits current scene and returns to menu.
    /// </summary>
    public void QuitScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
