using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DefenderDefaultScreen : MonoBehaviour
{
    public void QuitDefender()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
