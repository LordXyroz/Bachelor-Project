﻿using UnityEngine;
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
    private ConnectionReferences connectionReferencesOfObject;

    [Header("Selected object elements")]
    public GameObject selected;
    public GameObject oldSelected;
    public Canvas canvas;
    public Material selectMat;
    private Image image;
    private Image[] images;
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
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        images = GetComponentsInChildren<Image>();
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
        /// checks again to make sure function is not called from somewhere it should not
        if (connectionStarted
            && selected.GetComponent<SystemComponent>() != null
            && oldSelected.GetComponent<SystemComponent>() != null)
        {
            GameObject connectionLineClone = Instantiate(connectionLine, canvas.transform);
            connectionLineClone.transform.position = oldSelected.transform.position;
            connectionLineClone.transform.SetParent(oldSelected.transform.parent);
            connectionLineClone.transform.SetAsFirstSibling();

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
            if (systemComponentsToObject != null)
            {
                systemComponentsToObject.DeleteSystemComponent();
            }
            else
            {
                connectionReferencesOfObject = selected.GetComponent<ConnectionReferences>();
                connectionReferencesOfObject.RemoveConnectionComponent();
            }
        }
    }
}