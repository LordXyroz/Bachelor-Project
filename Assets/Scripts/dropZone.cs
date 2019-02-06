using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    Vector2 objectPosition;

    public void OnDrop(PointerEventData eventData)
    {
        /// Change the parent from the lists to the dropzone object, making it editable
        DraggableObject draggableObject = eventData.pointerDrag.GetComponent<DraggableObject>();
        if (draggableObject != null)
        {
            draggableObject.parentToReturnTo = this.transform;
        }
    }
}
