using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMockup : MonoBehaviour
{
    // loading the attacker mockup scene
    public void SelectAttackerSide()
    {
        SceneManager.LoadScene("AttackerScene");
    }

    // loading the defender mockup scene
    public void SelectDefenderSide()
    {
        SceneManager.LoadScene("DefenderScene");
    }

    // // loading the menu scene
    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

}
