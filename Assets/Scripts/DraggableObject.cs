using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //private Color mouseOverColor = Color.blue;
    //private Color originalColor = Color.yellow;
    //private bool dragging = false;
    //private float distance;

    public Transform parentToReturnTo = null;

    private Vector2 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        /// Use an ofset in order to have the focus on the clicked point, not snap to the middle

        offset = (Vector2)this.transform.position - eventData.position;
        Cursor.visible = false;

        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;


        Debug.Log("on begin drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position + offset;


        //Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;

        this.transform.SetParent(parentToReturnTo);

        GetComponent<CanvasGroup>().blocksRaycasts = true;


        Debug.Log("OnEndDrag. X: " + eventData.position.x + ", Y: " + eventData.position.y);
    }



    /*void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = mouseOverColor;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = originalColor;
    }

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
