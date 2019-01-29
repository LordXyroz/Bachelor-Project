using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.yellow;
    private bool dragging = false;
    private float distance;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("on begin drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
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
