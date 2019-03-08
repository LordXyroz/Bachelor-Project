using TMPro;
using UnityEngine;


/// <summary>
/// This script is to be put on the rename menu
/// </summary>


public class RenameMenu : MonoBehaviour
{
    [Header("Local variables for changing the component name")]
    private string tempName;
    private TMP_InputField input;
    public TMP_Text renameMenuTitle;

    [Header("Saving the new name to the component")]
    private Canvas canvas;
    private SelectedObject selectedObject;
    private SystemComponent systemComponent;
    //private DropZone dropZone;


    public void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        systemComponent = canvas.transform.Find("Scripts").GetComponent<SelectedObject>().selected.GetComponent<SystemComponent>();
        //dropZone = FindObjectOfType<DropZone>();

        input = GetComponentInChildren<TMP_InputField>();

        renameMenuTitle = this.gameObject.transform.Find("ComponentNameText").GetComponent<TMP_Text>();
        renameMenuTitle.text = "Rename menu";
    }

    public void RenameAcceptPressed()
    {
        Debug.Log("Input text is: " + input.text);
        tempName = input.text;
        Debug.Log("New name is: " + tempName);

        // TODO validate/sanitize name
        Debug.Log("Old name is: " + systemComponent.componentName);
        systemComponent.componentName = tempName;
        Debug.Log("Changed name is: " + systemComponent.name);

        systemComponent.UpdateComponentName();
        this.gameObject.SetActive(false);
    }
}
