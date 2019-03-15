using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script is added to all drag-and-drop UI elements
/// </summary>

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Refferenced objects")]
    private Color originalColor;
    private Camera mainCamera;
    private Canvas canvas;
    private Image systemComponentImage;
    private SelectedObject selectedObject;
    private SystemComponent systemComponent;
    private SystemComponentMenu systemComponentMenu;
    private ReferenceLineMenu referenceLineMenu;

    [Header("Positioning objects")]
    public Vector2 spawnPosition;
    private Vector2 offset;
    private Vector2 objectPosition;

    [Header("The parent of the object (usually the list it is placed under)")]
    public Transform parentToReturnTo;
    private DropZone dropZone;


    void Start()
    {
        spawnPosition = new Vector2(300, 650);
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        selectedObject = FindObjectOfType<SelectedObject>();
        systemComponent = GetComponent<SystemComponent>();
        systemComponentMenu = canvas.transform.GetComponentInChildren<SystemComponentMenu>(true);
        referenceLineMenu = canvas.transform.GetComponentInChildren<ReferenceLineMenu>(true);
        dropZone = FindObjectOfType<DropZone>();

        /// Only system components are draggable and start with one image object
        if (GetComponent<Image>() != null)
        {
            originalColor = GetComponent<Image>().color;
            systemComponentImage = GetComponent<Image>();
            parentToReturnTo = this.gameObject.transform.parent;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Cursor.visible = false;
        /// Only selectable if it is located in the SystemSetupScreen editor area
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            if (this.GetComponent<Image>() != null)
            {
                if (systemComponentMenu.gameObject.activeInHierarchy)
                {
                    systemComponentMenu.gameObject.SetActive(false);
                }
                if (referenceLineMenu.gameObject.activeInHierarchy)
                {
                    referenceLineMenu.gameObject.SetActive(false);
                }
                selectedObject.SelectObject(this.gameObject, true, true);
            }
            else
            {
                selectedObject.SelectObject(this.gameObject, true, false);
            }
        }

        /// if the object is not dragged from a dropzone and is a system component, make a clone of it.
        if (this.transform.parent.gameObject.GetComponent<DropZone>() == null && GetComponent<Image>() != null)
        {
            Image systemComponentClone = (Image)Instantiate(systemComponentImage, parentToReturnTo);
            systemComponentClone.color = originalColor;
        }

        offset = (Vector2)this.transform.position - eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position + offset;
        this.systemComponent.MoveConnections();

        if (this.systemComponent.connectedReferenceLines != null)
        {
            Debug.Log("Calling Update firewall");
            foreach (GameObject connection in systemComponent.connectedReferenceLines)
            {
                referenceLineMenu.UpdateFirewall(connection);
            }
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        this.transform.SetParent(parentToReturnTo);

        /// Setting up the area for the drop zone, deleting objects dropped outside of the drop zone
        //GameObject dropZone = GameObject.Find("SystemSetupScreen");
        // RectTransform setupScreen = dropZone.GetComponent<RectTransform>();

        //objectPosition = this.transform.position;
        //float width = setupScreen.rect.width;
        //float height = setupScreen.rect.height;
        //float objectWidth = this.GetComponent<RectTransform>().rect.width;
        //float objectHeight = this.GetComponent<RectTransform>().rect.height;

        /// Delete the object if it is placed outside of the editing screen field
        if (eventData.pointerCurrentRaycast.gameObject == null
            || !eventData.pointerCurrentRaycast.gameObject.name.Equals("DropzoneBackgroundButton"))
        {
            if (this.transform.parent.gameObject.GetComponent<DropZone>() == null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Debug.Log("DraggableObject dropped outside edit field: " + this.gameObject.name);
                selectedObject.DeleteSelectedObject();
            }
        }
    }

    void OnBecameInvisible()
    {
        selectedObject.DeleteSelectedObject();
    }
}
