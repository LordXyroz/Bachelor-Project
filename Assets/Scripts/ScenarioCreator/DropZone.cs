using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script placed on the dropzone (SystemSetupScreen) in order to make objects editable
/// </summary>


public class DropZone : MonoBehaviour, IDropHandler
{
    public List<GameObject> editableSystemComponents = new List<GameObject>();

    public void OnDrop(PointerEventData eventData)
    {
        DraggableObject draggableObject = eventData.pointerDrag.GetComponent<DraggableObject>();
        if (draggableObject != null)
        {
            if (draggableObject.parentToReturnTo != this.transform)
            {
                editableSystemComponents.Add(draggableObject.gameObject);
            }
            draggableObject.parentToReturnTo = this.transform;
        }
    }
}

