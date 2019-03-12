using TMPro;
using UnityEngine;

/// <summary>
/// This script is for the OverrideSavefilePopup, meant to appear to double-check if user wants to override an existing save file
/// </summary>
public class SavefileExistsPopup : MonoBehaviour
{
    private TMP_Text overrideText;
    private Canvas canvas;
    private SaveScenario saveScenario;


    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        saveScenario = canvas.transform.GetComponentInChildren<SaveScenario>(true);
        overrideText = this.gameObject.transform.Find("OverrideQuestionText").GetComponent<TMP_Text>();
        overrideText.text = "Would you like to override file: \n" + saveScenario.fileName;
    }


    public void OverrideSaveFile()
    {
        saveScenario.overrideFile = true;
        saveScenario.SaveCurrentScenario();
    }
}
