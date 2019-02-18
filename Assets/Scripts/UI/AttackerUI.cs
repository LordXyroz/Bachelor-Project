using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackerUI : MonoBehaviour
{
    [Header("Target field")]
    public Text targetText;

    [Header("Info field")]
    public GameObject infoStaticText;
    public GameObject infoTextObject;
    public Text infoText;

    [Header("Progressbar")]
    public GameObject progressbarObject;
    public Image progressbar;
    public Text progressbarTitle;
    public Text progressbarText;

    public void ChangeTarget(GameObject go)
    {
        targetText.text = go.name;
    }

    public void UpdateInfo(string text)
    {
        infoStaticText.SetActive(true);
        infoTextObject.SetActive(true);
        infoText.text = text;
    }

    public void UpdateProgressbar(float value, float max, string title, string text)
    {
        progressbarTitle.text = title;
        progressbarText.text = text;
        progressbar.fillAmount = value / max;
    }

    public void ToggleProgressbar(bool toggle)
    {
        progressbarObject.SetActive(toggle);
    }
    
}
 