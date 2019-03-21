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

        offset = (Vector2)this.transform.position - eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position + offset;
        this.systemComponent.MoveConnections();

        if (this.systemComponent.connectedReferenceLines != null)
        {
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

        /// Delete the object if it is placed outside of the editing screen field
        if (!dropZone.GetComponent<BoxCollider2D>().bounds.Contains(eventData.position))
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
}
