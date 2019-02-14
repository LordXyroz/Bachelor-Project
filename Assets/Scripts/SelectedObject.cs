using UnityEngine;
using UnityEngine.UI;


/// <TODO>
/// 
/// Reference all connections to selected object (List<GameObject>)
/// Delete selected objects
///     - If selected object is a system component, delete all connections
///     
/// 
/// </TODO>

public class SelectedObject : MonoBehaviour
{
    [Header("Connection line elements")]
    public GameObject connectionLine;
    private Transform lineToEnd;
    private Transform lineFromStart;
    private bool connectionStarted;
    private Vector3 connectionStartPos;
    private Vector3 connectionEndPos;
    private ConnectionReferences connectionReferences;
    private SystemComponent systemComponents;

    [Header("Selected object elements")]
    public GameObject selected;
    public Canvas canvas;
    private GameObject oldSelected;
    private Image image;
    private Image imageBox;


    void Start()
    {
        oldSelected = null;
        selected = null;
        imageBox = null;
        connectionStarted = false;
        connectionReferences = null;
        systemComponents = null;
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

            /// If button pressed for starting a connection
            if (oldSelected != null && connectionStarted)
            {
                ConnectObjects();
            }
        }
    }

    public void StartConnection()
    {
        connectionStarted = true;
    }

    /// <summary>
    /// This function does the logic behind connecting two objects in the editor. 
    /// It takes the previously selected gameobject and connects it with a visible line (2 image gameobjects) 
    /// if a new gameobject is selected after the "connect" button is pressed.
    /// </summary>
    public void ConnectObjects()
    {
        if (!connectionStarted)
        {
            /// checks again to make sure function is not called from somewhere it should not
            return;
        }
        else
        {
            connectionStartPos = oldSelected.transform.position;
            connectionEndPos = selected.transform.position;

            GameObject connectionLineClone = Instantiate(connectionLine, canvas.transform);
            connectionLineClone.transform.SetAsFirstSibling();
            connectionLineClone.transform.position = connectionStartPos;

            connectionReferences = connectionLineClone.GetComponent<ConnectionReferences>();
            connectionReferences.SetReferences(oldSelected, selected);

            lineToEnd = connectionLineClone.transform.Find("LineToEnd").GetComponent<RectTransform>().transform;
            lineFromStart = connectionLineClone.transform.Find("LineFromStart").GetComponent<RectTransform>().transform;

            float XPosDiff = Mathf.Abs(connectionStartPos.x - connectionEndPos.x) / 100;        //100 pixels per unit
            float YPosDiff = Mathf.Abs(connectionStartPos.y - connectionEndPos.y) / 100;        //100 pixels per unit

            /// connection to the right
            if (connectionStartPos.x <= connectionEndPos.x)
            {
                lineToEnd.localScale = new Vector3(XPosDiff, 0.05f, 1.0f);
                lineFromStart.localScale = new Vector3(0.05f, YPosDiff, 1.0f);
                /// connection down
                if (connectionStartPos.y >= connectionEndPos.y)
                {
                    lineFromStart.position = new Vector3(connectionStartPos.x, connectionEndPos.y + YPosDiff * 50);
                    lineToEnd.position = new Vector3(lineFromStart.position.x + XPosDiff * 50, connectionEndPos.y);
                }
                ///connection up
                else
                {
                    connectionLineClone.transform.Rotate(new Vector3(0, 180, 180));

                    lineFromStart.position = new Vector3(connectionStartPos.x, connectionEndPos.y - YPosDiff * 50);
                    lineToEnd.position = new Vector3(lineFromStart.position.x + XPosDiff * 50, connectionEndPos.y);
                }
            }
            /// connection to the left
            else
            {
                lineFromStart.localScale = new Vector3(0.05f, XPosDiff, 1.0f);
                lineToEnd.localScale = new Vector3(YPosDiff, 0.05f, 1.0f);

                /// connection down
                if (connectionStartPos.y >= connectionEndPos.y)
                {
                    connectionLineClone.transform.Rotate(new Vector3(180, 180, 90));

                    lineFromStart.position = new Vector3(connectionStartPos.x - XPosDiff * 50, connectionStartPos.y);
                    lineToEnd.position = new Vector3(lineFromStart.position.x - XPosDiff * 50, connectionEndPos.y + YPosDiff * 50);
                }
                ///connection up
                else
                {
                    connectionLineClone.transform.Rotate(new Vector3(180, 0, 270));

                    lineFromStart.position = new Vector3(connectionStartPos.x - XPosDiff * 50, connectionStartPos.y);
                    lineToEnd.position = new Vector3(lineFromStart.position.x - XPosDiff * 50, connectionEndPos.y - YPosDiff * 50);
                }
            }
            connectionStarted = false;
            systemComponents = selected.GetComponent<SystemComponent>();
            systemComponents.AddReference(connectionLineClone);
            systemComponents = oldSelected.GetComponent<SystemComponent>();
            systemComponents.AddReference(connectionLineClone);

        }
    }
}
