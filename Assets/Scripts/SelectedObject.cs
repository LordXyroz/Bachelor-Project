﻿using UnityEngine;
using UnityEngine.UI;

public class SelectedObject : MonoBehaviour
{
    [Header("Connection line elements")]
    public GameObject connectionLine;
    private RectTransform horizontalLine;
    private RectTransform verticalLine;
    private bool connectionStarted;
    private Vector3 connectionStartPos;
    private Vector3 connectionEndPos;

    [Header("Selected object elements")]
    public GameObject selected;
    private GameObject oldSelected;
    private GameObject selectionBox;
    private Image image;
    private Image imageBox;
    public Canvas canvas;


    void Start()
    {
        oldSelected = null;
        selected = null;
        imageBox = null;
        connectionStarted = false;
    }

    public void SelectObject(GameObject newSelected, Material mat)
    {
        /// Reset the selected properties of the previously selected object, if there is one
        if (selected != null)
        {
            image.material = default;
            imageBox.gameObject.SetActive(false);
        }

        if (newSelected != null)
        {
            /// Reset and set active to null if the selected object is clicked again
            if (selected == newSelected)
            {
                image.material = default;
                imageBox.gameObject.SetActive(false);
                selected = null;
                connectionStarted = false;
                return;
            }

            image = newSelected.GetComponent<Image>();
            imageBox = newSelected.transform.Find("selectionBox").GetComponent<Image>();

            oldSelected = selected;
            selected = newSelected;
            image.material = mat;
            imageBox.gameObject.SetActive(true);


            Debug.Log("Status of connectionStarted:  " + connectionStarted);
            /// If button pressed for starting a connection
            if (oldSelected != null && connectionStarted)
            {
                ConnectObjects();
            }
            //Debug.Log("Selected object is now " + newSelected.name);
        }
    }

    public void StartConnection()
    {
        connectionStarted = true;
    }

    public void ConnectObjects()
    {
        if (!connectionStarted)
        {
            /// checks double to make sure function is not called from elsewhere
            return;
        }
        else
        {
            GameObject connectionLineClone = Instantiate(connectionLine, canvas.transform);
            connectionLineClone.transform.SetAsFirstSibling();
            //horizontalLine = connectionLineClone.transform.Find("HorizontalLine").GetComponent<RectTransform>();
            connectionLineClone.transform.position = oldSelected.transform.position;


            connectionStartPos = oldSelected.transform.position;
            connectionEndPos = selected.transform.position;

            /// connection to the right
            if (connectionStartPos.x <= connectionEndPos.x)
            {
                //Debug.Log("Connection right--    start: " + connectionStartPos.x + " end: " + connectionEndPos.x);
                /// connection down
                if (connectionStartPos.y >= connectionEndPos.y)
                {
                    //connectionLineClone.transform.rotation = rotate;
                    //Debug.Log("Connection down--");
                }
                ///connection up
                else
                {
                    connectionLineClone.transform.Rotate(new Vector3(0, 180, 180));
                    //Debug.Log("Connection --up " + rotate);
                }
            }
            /// connection to the left
            else
            {
                //Debug.Log("Connection --left");
                /// connection down
                if (connectionStartPos.y >= connectionEndPos.y)
                {
                    connectionLineClone.transform.Rotate(new Vector3(0, 0, 270));
                    //Debug.Log("Connection down--");
                }
                ///connection up
                else
                {
                    connectionLineClone.transform.Rotate(new Vector3(180, 0, 270));
                    //Debug.Log("Connection --up    start: " + connectionStartPos.y + " end: " + connectionEndPos.y);
                }
            }


            ///reset after connection is esablished
            connectionStarted = false;
        }
    }
}
