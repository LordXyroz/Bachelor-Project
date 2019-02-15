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
    private Image systemComponentImage;
    private SelectedObject objectSelect;
    private SystemComponent systemComponent;

    [Header("Position objects")]
    private Vector2 offset;
    Vector2 objectPosition;


    [Header("The parent of the object (usually the list it is placed under)")]
    public Transform parentToReturnTo;


    void Start()
    {
        mainCamera = Camera.main;
        objectSelect = FindObjectOfType<SelectedObject>();
        systemComponent = GetComponent<SystemComponent>();
        if (GetComponent<Image>() != null)
        {
            originalColor = GetComponent<Image>().color;
            systemComponentImage = GetComponent<Image>();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Cursor.visible = false;
        /// Only selectable if it is located in the SystemSetupScreen editor area
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            objectSelect.SelectObject(this.gameObject, true);
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
        //Debug.Log("Calling MoveConnections from DraggableObject - " + this.name);
        this.systemComponent.MoveConnections();
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        this.transform.SetParent(parentToReturnTo);

        /// delete the game object if it is placed outside the drop zone
        if (this.transform.parent.gameObject.GetComponent<DropZone>() == null)
        {
            Destroy(this.gameObject);
        }

        /// Setting up the area for the drop zone, deleting objects dropped outside of the drop zone
        GameObject dropZone = GameObject.Find("SystemSetupScreen");
        RectTransform setupScreen = dropZone.GetComponent<RectTransform>();

        objectPosition = this.transform.position;
        float width = setupScreen.rect.width;
        float height = setupScreen.rect.height;
        float objectWidth = this.GetComponent<RectTransform>().rect.width;
        float objectHeight = this.GetComponent<RectTransform>().rect.height;

        if (objectPosition.x < width * 0.5f - objectWidth * 0.33f ||
            objectPosition.x > width * 1.5f - objectWidth * 1.3f ||
            objectPosition.y < height * 0.32f - objectHeight ||
            objectPosition.y > height + 10)
        {
            Destroy(this.gameObject);
        }
    }
}
