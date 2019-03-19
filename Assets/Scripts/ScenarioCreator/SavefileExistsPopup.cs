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
    private SaveMenu saveMenu;


    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        saveMenu = canvas.transform.GetComponentInChildren<SaveMenu>(true);
        saveScenario = canvas.transform.GetComponentInChildren<SaveScenario>(true);
        overrideText = this.gameObject.transform.Find("OverrideQuestionText").GetComponent<TMP_Text>();
        overrideText.text = "Would you like to override file: \n" + saveScenario.fileName;// saveMenu.filename;
    }


    public void OverrideSaveFile()
    {
        saveScenario.overrideFile = true;
        saveScenario.SaveCurrentScenario();
    }
}
