using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        DraggableObject d = eventData.pointerDrag.GetComponent<DraggableObject>();
        if (d != null)
        {
            d.parentToReturnTo = this.transform;
        }


        //Debug.Log(eventData.pointerDrag.name + " - was dropped on - " + gameObject.name);
    }
}
