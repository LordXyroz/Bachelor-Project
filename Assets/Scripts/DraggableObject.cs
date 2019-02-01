using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Color mouseOverColor = Color.blue;
    private Color originalColor;

    public float setupScreenWidth;
    public float setupScreenHeight;

    private Camera mainCamera;

    public Transform parentToReturnTo = null;
    public Image API;

    private Vector2 offset;
    Vector2 objectPosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        /// if the object is not dragged from a dropzone, make a clone of it.
        if (this.transform.parent.gameObject.GetComponent<DropZone>() == null)
        {
            Image APIClone = (Image)Instantiate(API, parentToReturnTo);
        }

        offset = (Vector2)this.transform.position - eventData.position;
        Cursor.visible = false;
        originalColor = GetComponent<Image>().color;

        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;


        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            Debug.Log("Dropzone is present");
            EventSystem.current.SetSelectedGameObject(null);
            //this.gameObject.GetComponent<HighlightObject>().StartHighlight();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position + offset;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        this.transform.SetParent(parentToReturnTo);


        if (this.transform.parent.gameObject.GetComponent<DropZone>() == null)
        {
            Destroy(this.gameObject);
        }


        /// Setting up the area for the drop zone, deleting objects dropped outside of the drop zone
        GameObject dropZone = GameObject.Find("SystemSetupScreen");
        RectTransform setupScreen = dropZone.GetComponent<RectTransform>();// transform as RectTransform;

        objectPosition = this.transform.position;
        float width = setupScreen.rect.width;
        float height = setupScreen.rect.height;
        float objectWidth = this.GetComponent<RectTransform>().rect.width;
        float objectHeight = this.GetComponent<RectTransform>().rect.height;

        Debug.Log("position: " + objectPosition.x + ", " + objectPosition.y +
          "----, width low: " + (width * 0.5f - objectWidth * 0.33f) + " width high: " + (width * 1.5f - objectWidth * 1.3f));

        if (objectPosition.x < width * 0.5f - objectWidth * 0.33f ||
            objectPosition.x > width * 1.5f - objectWidth * 1.3f ||
            objectPosition.y < height * 0.32f - objectHeight ||
            objectPosition.y > height + 10)
        {
            Destroy(this.gameObject);
        }
    }


    void OnMouseEnter()
    {
        GetComponent<Image>().material.color = mouseOverColor;
        Debug.Log("OnMouseEnter (color change) " + GetComponent<Image>().material.color);
    }

    void OnMouseExit()
    {
        GetComponent<Image>().material.color = originalColor;
    }

    /// destroys objects outside of the screen (outside camera view)
    /// TODO sometimes deletes objects before exiting screen, needs debugging
    /*void OnBecameInvisible()
    {
        Debug.Log("Outside of screen");
        Destroy(gameObject);
    }*/
}
