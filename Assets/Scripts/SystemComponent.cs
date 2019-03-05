using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is to be places on all system components.
/// </summary>
/// 

/// TODO 
/// 
/// Load list of available vulnerabilities from file (list of enums)
/// 
/// 
/// Automatically generate name of system components upon initiation, where name = component + (no. of this component +1).
/// Save the components in the system in a script, in order to have number as well as the objects saved?
/// 
/// Save scenario to file
///     - Text input field for filename
///     - Error if file exists, override method?
/// Load scenario from file
///     - List up available json document names, select which to load
/// 
/// Functionality to delete specific vulnerabilities


public class SystemComponent : MonoBehaviour
{
    [Header("Attributes for this object/node")]
    public int securityLevel;
    public List<string> componentVulnerabilities = new List<string>();
    public string componentType;   // Must match the prefab name
    public string componentName;

    [Header("Reference line components")]
    public List<GameObject> connectedReferenceLines = new List<GameObject>();
    List<GameObject> connectedSystemComponents = new List<GameObject>();
    private GameObject connectedSystemCommponent;
    private Transform lineToEnd;
    private Transform lineFromStart;

    [Header("Items needed for deleting a system component")]
    private SystemComponent systemComponent;
    private DropZone dropZone;

    [Header("Empty objects intended for later development cycles")]
    public string OS = null;
    public string subnet = null;
    public GameObject configPresent = null;
    public string keypair = null;
    public bool floatingIP = false;
    public User user = new User();


    private void Start()
    {
        dropZone = FindObjectOfType<DropZone>();

        /// TODO delete these, for testing purposes only
        componentVulnerabilities.Add("Test vulnerability 1");
        componentVulnerabilities.Add("Test vulnerability 2");
        componentVulnerabilities.Add("Test vulnerability 3");
        ///////////////////////////////////////////////
    }



    /// <summary>
    /// This function sets the position and scale of the connecting line(s)
    /// </summary>
    /// <param name="startPos">The position of the system component GameObject the connection starts</param>
    /// <param name="endPos">The position of the system component GameObject the connection ends</param>
    /// <param name="connectionLine">The visual line GameObject connecting the two system component GameObjects (prefab)</param>
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


    public List<GameObject> GetConnectedComponents()
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
            connectedSystemComponents.Add(connectedSystemCommponent);

        }
        return connectedSystemComponents;
    }


    public void UpdateComponentVulnerabilityInformation(int security, List<string> vulnerabilities)
    {
        securityLevel = security;
        componentVulnerabilities = vulnerabilities;
    }
}

