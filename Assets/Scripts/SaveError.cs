using TMPro;
using UnityEngine;

public class SaveError : MonoBehaviour
{
    public TMP_Text text;


    public void ChangeErrorMessage(string message)
    {
        text = this.gameObject.transform.Find("MessageText").GetComponent<TMP_Text>();
        text.text = message + "\n\n Scenario not saved! ";
    }
}
