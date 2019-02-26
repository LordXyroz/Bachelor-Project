using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is to be places on all system components.
/// </summary>
/// 

/// TODO 
/// 
/// Have a textbox with the type of component, only visible in the menu (next to the draggable prefab object)
/// 
/// Spawn a menu when rightclicked: display name, connect, delete, vulnerability list, component vulnerability level
///     - Load list of available vulnerabilities from file (list of enums)
/// Save list of vulnerabilities (vulnerabilities do not have levels)
/// save vulnerability level for component (1-5)
/// 
/// Make separate right-click menu for reference lines, name, bool firewall, delete

public class SystemComponent : MonoBehaviour
{

    [Header("Attributes for this object/node")]
    public string componentType;
    public List<String> componentVulnerabilities;

    [Header("Reference line components")]
    public List<GameObject> connectedReferenceLines;
    private GameObject connectedSystemCommponent;
    private Transform lineToEnd;
    private Transform lineFromStart;

    [Header("Items needed for deleting a system component")]
    private SystemComponent systemComponent;
    private DropZone dropZone;


    private void Start()
    {
        //availableVulnerabilitiesDropdown = gameObject.GetComponentInParent<Canvas>().gameObject.Find("AvailableVulnerabilitiesDropdown");
        connectedReferenceLines = new List<GameObject>();
        dropZone = FindObjectOfType<DropZone>();
        componentVulnerabilities = new List<string>();
    }


    /// <summary>
    /// This function sets the position and scale of the connecting line(s)
    /// </summary>
    /// <param name="startPos">The position of the system component GameObject the connection starts</param>
    /// <param name="endPos">The position of the system component GameObject the connection ends</param>
    /// <param name="connectionLine">The visual line GameObject connecting the two system component GameObjects</param>
    public void SetConnectionLines(Vector3 startPos, Vector3 endPos, GameObject connectionLine)
    {
        lineToEnd = connectionLine.transform.Find("LineToEnd").GetComponent<RectTransform>().transform;
        lineFromStart = connectionLine.transform.Find("LineFromStart").GetComponent<RectTransform>().transform;

        float XPosDiff = Mathf.Abs(startPos.x - endPos.x) / 100;        //100 pixels per unit
        float YPosDiff = Mathf.Abs(startPos.y - endPos.y) / 100;        //100 pixels per unit


        /// connection to the right
        if (startPos.x < endPos.x)
        {
            /// connection down
            if (startPos.y > endPos.y)
            {
                lineToEnd.localScale = new Vector3(XPosDiff, 0.05f, 1.0f);
                lineFromStart.localScale = new Vector3(0.05f, YPosDiff, 1.0f);
                connectionLine.transform.rotation = Quaternion.identity;

                lineFromStart.position = new Vector3(startPos.x, endPos.y + YPosDiff * 50);
                lineToEnd.position = new Vector3(lineFromStart.position.x + XPosDiff * 50, endPos.y);
            }
            ///connection up
            else
            {
                lineFromStart.localScale = new Vector3(0.05f, XPosDiff, 1.0f);
                lineToEnd.localScale = new Vector3(YPosDiff, 0.05f, 1.0f);
                connectionLine.transform.rotation = Quaternion.Euler(0, 0, 90);

                lineFromStart.position = new Vector3(startPos.x + XPosDiff * 50, startPos.y);
                lineToEnd.position = new Vector3(lineFromStart.position.x + XPosDiff * 50, endPos.y - YPosDiff * 50);
            }
        }
        /// connection to the left
        else
        {

            /// connection down
            if (startPos.y > endPos.y)
            {
                lineFromStart.localScale = new Vector3(0.05f, XPosDiff, 1.0f);
                lineToEnd.localScale = new Vector3(YPosDiff, 0.05f, 1.0f);
                connectionLine.transform.rotation = Quaternion.Euler(0, 0, 270);

                lineFromStart.position = new Vector3(startPos.x - XPosDiff * 50, startPos.y);
                lineToEnd.position = new Vector3(lineFromStart.position.x - XPosDiff * 50, endPos.y + YPosDiff * 50);
            }
            ///connection up
            else
            {
                lineFromStart.localScale = new Vector3(XPosDiff, 0.05f, 1.0f);
                lineToEnd.localScale = new Vector3(0.05f, YPosDiff, 1.0f);
                connectionLine.transform.rotation = Quaternion.Euler(0, 0, -180);

                lineFromStart.position = new Vector3(startPos.x - XPosDiff * 50, startPos.y);
                lineToEnd.position = new Vector3(lineFromStart.position.x - XPosDiff * 50, endPos.y - YPosDiff * 50);
            }
        }
    }


    public void AddReference(GameObject reference)
    {
        connectedReferenceLines.Add(reference);
    }


    /// <summary>
    /// Move the connecting lines with the movement of the GameObjects they are connected to
    /// </summary>
    public void MoveConnections()
    {
        if (connectedReferenceLines != null)
        {
            foreach (GameObject line in connectedReferenceLines)
            {
                if (line.GetComponent<ConnectionReferences>().referenceFromObject == this.gameObject)
                {
                    connectedSystemCommponent = line.GetComponent<ConnectionReferences>().referenceToObject;
                    SetConnectionLines(this.gameObject.transform.position, connectedSystemCommponent.transform.position, line);
                }
                else if (line.GetComponent<ConnectionReferences>().referenceToObject == this.gameObject)
                {
                    connectedSystemCommponent = line.GetComponent<ConnectionReferences>().referenceFromObject;
                    SetConnectionLines(connectedSystemCommponent.transform.position, this.gameObject.transform.position, line);
                }
            }
        }
    }


    public void DeleteSystemComponent()
    {
        foreach (GameObject line in connectedReferenceLines)
        {
            if (line.GetComponent<ConnectionReferences>().referenceFromObject == this.gameObject)
            {
                connectedSystemCommponent = line.GetComponent<ConnectionReferences>().referenceToObject;
            }
            else if (line.GetComponent<ConnectionReferences>().referenceToObject == this.gameObject)
            {
                connectedSystemCommponent = line.GetComponent<ConnectionReferences>().referenceFromObject;
            }

            systemComponent = connectedSystemCommponent.GetComponent<SystemComponent>();
            systemComponent.connectedReferenceLines.Remove(line);
            Destroy(line);
        }
        dropZone.editableSystemComponents.Remove(this.gameObject);
        Destroy(this.gameObject);
    }


}

