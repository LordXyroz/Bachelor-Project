using TMPro;
using UnityEngine;

/// <summary>
/// Class handling an error when saving a scenario in the scenario creator.
/// </summary>
public class SaveError : MonoBehaviour
{
    public TMP_Text text;

    /// <summary>
    /// Function that changes the message displayed on error.
    /// </summary>
    /// <param name="message">String to display</param>
    public void ChangeErrorMessage(string message)
    {
        text = this.gameObject.transform.Find("MessageText").GetComponent<TMP_Text>();
        text.text = message + "\n\n Scenario not saved! ";
    }
}
