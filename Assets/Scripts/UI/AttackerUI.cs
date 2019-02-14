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
}
 