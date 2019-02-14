using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script placed on the dropzone (SystemSetupScreen) in order to make objects editable
/// </summary>

public class DropZone : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        DraggableObject draggableObject = eventData.pointerDrag.GetComponent<DraggableObject>();
        if (draggableObject != null)
        {
            draggableObject.parentToReturnTo = this.transform;
        }
    }
}
