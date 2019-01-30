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

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (this.transform.parent.gameObject.GetComponent<DropZone>() == null)
        {
            Destroy(this.gameObject);
        }


        //Debug.Log("OnEndDrag. X: " + eventData.position.x + ", Y: " + eventData.position.y);
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
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
