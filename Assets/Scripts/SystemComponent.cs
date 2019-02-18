using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is to be places on all system components.
/// It keeps a list of all lines connected to the component.
/// </summary>
public class SystemComponent : MonoBehaviour
{
    [Header("List of reference lines connected to this system component")]
    public List<GameObject> connectedReferenceLines;

    [Header("Items for moving the connection lines when the system components are moved")]
    private GameObject connectedSystemCommponent;

    [Header("Items for calculating size of the connection lines")]
    private Transform lineToEnd;
    private Transform lineFromStart;

    [Header("Items needed for deleting a system component")]
    private SystemComponent systemComponent;


    private void Start()
    {
        connectedReferenceLines = new List<GameObject>();
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
        Destroy(this.gameObject);
    }
}

