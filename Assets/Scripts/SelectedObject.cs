using UnityEngine;
using UnityEngine.UI;


/// <TODO>
/// 
/// Delete selected connection line
///     - Have visual display of what line will be deleted
///     
/// </TODO>

public class SelectedObject : MonoBehaviour
{
    [Header("Connection line elements")]
    public GameObject connectionLine;
    private Transform lineToEnd;
    private Transform lineFromStart;
    private bool connectionStarted;
    private ConnectionReferences connectionReferences;
    private SystemComponent systemComponentsFromObject;
    private SystemComponent systemComponentsToObject;

    [Header("Selected object elements")]
    public GameObject selected;
    public GameObject oldSelected;
    public Canvas canvas;
    public Material selectMat;
    private Image image;
    private Image imageBox;


    void Start()
    {
        oldSelected = null;
        selected = null;
        imageBox = null;
        connectionStarted = false;
        connectionReferences = null;
        systemComponentsFromObject = null;
        systemComponentsToObject = null;
    }


    /// <summary>
    /// This function recieves the selected system component GameObject, and marks this as the newly selected object.
    /// If there is already an object selected, this gets saved as the oldSelected object, and the visuals are set to default.
    /// </summary>
    /// <param name="newSelected">The newly selected system component GameObject</param>
    /// <param name="dragged">Check if the object is dragged or clicked</param>
    public void SelectObject(GameObject newSelected, bool dragged)
    {
        if (newSelected.GetComponent<Image>() != null)
        {
            /// Reset the selected properties of the previously selected object, if there is one
            if (selected != null)
            {
                image.material = default;
                imageBox.gameObject.SetActive(false);
            }

            if (newSelected != null)
            {
                /// Reset and set active to null if the selected object is clicked again (not dragged)
                if (selected == newSelected && !dragged)
                {
                    image.material = default;
                    imageBox.gameObject.SetActive(false);
                    oldSelected = selected;
                    selected = null;
                    connectionStarted = false;
                    return;
                }

                if (newSelected != selected && selected != null)
                {
                    oldSelected = selected;
                }
                selected = newSelected;

                image = newSelected.GetComponent<Image>();
                image.material = selectMat;
                imageBox = newSelected.transform.Find("selectionBox").GetComponent<Image>();
                imageBox.gameObject.SetActive(true);

                /// If button pressed for starting a connection
                if (oldSelected != null && connectionStarted)
                {
                    ConnectObjects();
                }
            }
        }
    }


    /// <summary>
    /// Activate the flag for connecting two GameObjects
    /// </summary>
    public void StartConnectionButton()
    {
        connectionStarted = true;
    }


    /// <summary>
    /// This function does the logic behind connecting two objects in the editor. 
    /// It takes the previously selected gameobject and connects it with a visible line (2 image gameobjects) 
    /// if a new gameobject is selected after the "connect" button is pressed.
    /// Alternatively, if no object is selected when the button is pressed, the two next objects selected will be connected
    /// </summary>
    public void ConnectObjects()
    {
        /// checks again to make sure function is not called from somewhere it should not
        if (connectionStarted)
        {
            GameObject connectionLineClone = Instantiate(connectionLine, canvas.transform);
            connectionLineClone.transform.SetParent(oldSelected.transform.parent);
            connectionLineClone.transform.SetAsFirstSibling();
            connectionLineClone.transform.position = oldSelected.transform.position;

            connectionReferences = connectionLineClone.GetComponent<ConnectionReferences>();
            connectionReferences.SetReferences(oldSelected, selected);

            systemComponentsToObject = selected.GetComponent<SystemComponent>();
            systemComponentsFromObject = oldSelected.GetComponent<SystemComponent>();
            systemComponentsToObject.AddReference(connectionLineClone);
            systemComponentsFromObject.AddReference(connectionLineClone);

            systemComponentsFromObject.SetConnectionLines(oldSelected.transform.position, selected.transform.position, connectionLineClone);
            connectionStarted = false;
        }
    }


    /// <summary>
    /// This function starts the chain to delete a system component object.
    /// Needs to be in a publicly available script to easily be asccesible for the button.
    /// </summary>
    public void DeleteSelectedObject()
    {
        if (selected != null)
        {
            systemComponentsToObject = selected.GetComponent<SystemComponent>();
            systemComponentsToObject.DeleteSystemComponent();
        }
    }
}