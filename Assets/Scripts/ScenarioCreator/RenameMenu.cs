using TMPro;
using UnityEngine;


/// <summary>
/// This script is to be put on the rename menu
/// </summary>
public class RenameMenu : MonoBehaviour
{
    [Header("Local variables for changing the component name")]
    private TMP_InputField input;

    [Header("Saving the new name to the component")]
    private Canvas canvas;
    private SystemComponent systemComponent;

    /// <summary>
    /// Get the new selected component every time the menu is enabled, to ensure the correct component is renamed
    /// </summary>
    public void OnEnable()
    {
        canvas = GetComponentInParent<Canvas>();
        systemComponent = canvas.transform.Find("Scripts").GetComponent<SelectedObject>().selected.GetComponent<SystemComponent>();
        input = GetComponentInChildren<TMP_InputField>();

        if (systemComponent.componentName == "")
        {
            input.text = "<Insert new name here>";
        }
        else
        {
            input.text = systemComponent.componentName;
        }
    }


    /// <summary>
    /// Accept button pressed for the new name
    /// </summary>
    public void RenameAcceptPressed()
    {
        // TODO validate/sanitize name
        systemComponent.componentName = input.text;

        systemComponent.UpdateComponentName();
        this.gameObject.SetActive(false);
    }
}
