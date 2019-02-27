using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The script for selecting an object.
/// Placed at the top of the hierarchy (under the canvas)
/// </summary>

public class SelectedObject : MonoBehaviour
{
    [Header("Connection line elements")]
    public GameObject connectionLine;
    private Transform lineToEnd;
    private Transform lineFromStart;
    private bool connectionStarted;
    private ConnectionReferences connectionReferences;
    private SystemComponent systemComponentOfOldSelected;
    private SystemComponent systemComponentOfSelected;
    private ConnectionReferences connectionReferencesOfObject;

    [Header("Selected object elements")]
    public GameObject selected;
    public GameObject oldSelected;
    public Canvas canvas;
    public Material selectMat;
    private Image image;
    private Image[] images;     /// For reference lines
    private Image imageBox;     /// For system components
    private RectTransform componentMenu;
    private ReferenceLineMenu referenceLineMenu;


    void Start()
    {
        oldSelected = null;
        selected = null;
        imageBox = null;
        connectionStarted = false;
        connectionReferences = null;
        systemComponentOfOldSelected = null;
        systemComponentOfSelected = null;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        images = GetComponentsInChildren<Image>();
        componentMenu = canvas.transform.Find("SystemComponentMenu").gameObject.GetComponent<RectTransform>();
        referenceLineMenu = canvas.transform.Find("ReferenceLineMenu").gameObject.GetComponent<ReferenceLineMenu>();
    }


    /// <summary>
    /// This function recieves the selected system component GameObject, and marks this as the newly selected object.
    /// If there is already an object selected, this gets saved as the oldSelected object, and the visuals are set to default.
    /// </summary>
    /// <param name="newSelected">The newly selected system component GameObject</param>
    /// <param name="dragged">Check if the object is dragged or clicked</param>
    /// <param name="systemComponent">Flag for system component</param>
    public void SelectObject(GameObject newSelected, bool dragged, bool systemComponent)
    {
        if (newSelected != null)
        {
            /// Reset the selected properties of the previously selected object, if there is one
            if (selected != null)
            {
                image.material = default;
                foreach (Image img in images)
                {
                    img.material = default;
                }

                imageBox.gameObject.SetActive(false);
            }
            images = GetComponentsInChildren<Image>();

            if (newSelected != null)
            {
                if (systemComponent)
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
                else if (!systemComponent)
                {
                    images = newSelected.GetComponentsInChildren<Image>();

                    /// Reset and set active to null if the selected object is clicked again (not dragged)
                    if (selected == newSelected && !dragged)
                    {
                        foreach (Image img in images)   // Connection line
                        {
                            img.material = default;
                        }
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

                    foreach (Image img in images)
                    {
                        img.material = selectMat;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Selected connection line:  " + newSelected.name);
        }
    }


    public void DeselectObjects()
    {
        if (selected != null)
        {
            image.material = default;
            foreach (Image img in images)
            {
                img.material = default;
            }
            imageBox.gameObject.SetActive(false);
            oldSelected = selected;
            selected = null;
        }

        if (componentMenu.gameObject.activeInHierarchy)
        {
            componentMenu.gameObject.SetActive(false);
        }
        if (referenceLineMenu.gameObject.activeInHierarchy)
        {
            referenceLineMenu.gameObject.SetActive(false);
        }
    }


    public void StartConnectionButton()
    {
        if (selected != null && selected.GetComponent<SystemComponent>() != null)
        {
            connectionStarted = true;
        }
    }


    /// <summary>
    /// This function does the logic behind connecting two objects in the editor. 
    /// It takes the previously selected gameobject and connects it with a visible line (2 image gameobjects) 
    /// if a new gameobject is selected after the "connect" button is pressed.
    /// Alternatively, if no object is selected when the button is pressed, the two next objects selected will be connected
    /// </summary>
    public void ConnectObjects()
    {
        /// Make sure both selected are system components
        if (connectionStarted
            && selected.GetComponent<SystemComponent>() != null
            && oldSelected.GetComponent<SystemComponent>() != null)
        {
            GameObject connectionLineClone = Instantiate(connectionLine, canvas.transform);
            connectionLineClone.transform.position = oldSelected.transform.position;
            connectionLineClone.transform.SetParent(GameObject.Find("Connections").transform);
            connectionLineClone.transform.SetAsFirstSibling();

            connectionReferences = connectionLineClone.GetComponent<ConnectionReferences>();
            connectionReferences.SetReferences(oldSelected, selected);

            systemComponentOfSelected = selected.GetComponent<SystemComponent>();
            systemComponentOfOldSelected = oldSelected.GetComponent<SystemComponent>();
            systemComponentOfSelected.AddReference(connectionLineClone);
            systemComponentOfOldSelected.AddReference(connectionLineClone);

            systemComponentOfOldSelected.SetConnectionLines(oldSelected.transform.position, selected.transform.position, connectionLineClone);
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
            systemComponentOfSelected = selected.GetComponent<SystemComponent>();
            if (systemComponentOfSelected != null)
            {
                systemComponentOfSelected.DeleteSystemComponent();
            }
            else
            {
                connectionReferencesOfObject = selected.GetComponent<ConnectionReferences>();
                connectionReferencesOfObject.RemoveConnectionComponent();
            }
        }
    }
}