using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenarioCreatorUI : MonoBehaviour
{
    public Button mainMenuButton;
    
    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
