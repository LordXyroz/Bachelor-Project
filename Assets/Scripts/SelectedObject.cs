using UnityEngine;
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
            return;
        }
        else
        {
            connectionStartPos = oldSelected.transform.position;
            connectionEndPos = selected.transform.position;

            /// connection to the right
            if (connectionStartPos.x <= connectionEndPos.x)
            {
                //Debug.Log("Connection right--    start: " + connectionStartPos.x + " end: " + connectionEndPos.x);
                /// connection down
                if (connectionStartPos.y >= connectionEndPos.y)
                {
                    //Debug.Log("Connection down--");
                }
                ///connection up
                else
                {
                    //Debug.Log("Connection --up");
                }
            }
            /// connection to the left
            else
            {
                //Debug.Log("Connection --left");
                /// connection down
                if (connectionStartPos.y >= connectionEndPos.y)
                {
                    //Debug.Log("Connection down--");
                }
                ///connection up
                else
                {
                    //Debug.Log("Connection --up    start: " + connectionStartPos.y + " end: " + connectionEndPos.y);
                }
            }

            GameObject connectionLineClone = Instantiate(connectionLine, canvas.transform);
            //horizontalLine = connectionLineClone.transform.Find("HorizontalLine").GetComponent<RectTransform>();
            connectionLineClone.transform.position = oldSelected.transform.position;
            Debug.Log("Horizontal line position: " + connectionLineClone.transform.position);

            ///reset after connection is esablished
            connectionStarted = false;
        }
    }
}
