using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Color mouseOverColor = Color.blue;
    private Color originalColor;
    //private bool dragging = false;
    //private float distance;

    public Transform parentToReturnTo = null;
    public Image API;

    private Vector2 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        /// Use an ofset in order to have the focus on the clicked point, not snap to the middle
        //Image APIClone = (Image)Instantiate(API, this.transform.position, this.transform.rotation);

        //DropZone dropZone = eventData.pointerDrag.GetComponent<DropZone>();

        //Debug.Log(dropZone.transform.parent.GetComponent<DropZone>());
        if (this.transform.parent.gameObject.GetComponent<DropZone>() == null)
        {
            Debug.Log("Instantiating API clone");
            Image API2 = (Image)Instantiate(API, parentToReturnTo);
        }

        offset = (Vector2)this.transform.position - eventData.position;
        Cursor.visible = false;
        originalColor = GetComponent<Image>().color;

        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position + offset;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;

        this.transform.SetParent(parentToReturnTo);
        Debug.Log("OnDrop parent: " + parentToReturnTo);

        if (this.transform.parent != parentToReturnTo)
        {
            Debug.Log("Wrong parent on drop: " + this.transform.parent + " - " + parentToReturnTo);
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;


        //Debug.Log("OnEndDrag. X: " + eventData.position.x + ", Y: " + eventData.position.y);
    }



    void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter (color change)");
        GetComponent<Image>().material.color = mouseOverColor;
    }

    void OnMouseExit()
    {
        GetComponent<Image>().material.color = originalColor;
    }
    /*
    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
    }*/
}
