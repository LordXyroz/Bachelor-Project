using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    Vector2 objectPosition;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableObject draggableObject = eventData.pointerDrag.GetComponent<DraggableObject>();
        if (draggableObject != null)
        {
            draggableObject.parentToReturnTo = this.transform;
        }

        /*RectTransform setupScreen = transform as RectTransform;


        objectPosition = draggableObject.transform.position;
        float width = setupScreen.rect.width;
        float height = setupScreen.rect.height;
        float objectWidth = draggableObject.GetComponent<RectTransform>().rect.width;
        float objectHeight = draggableObject.GetComponent<RectTransform>().rect.height;

        if (objectPosition.x > width * 0.5f - objectWidth * 0.33f ||
            objectPosition.x < width * 1.5f - objectWidth * 1.3f ||
            objectPosition.y > height * 0.32f - objectHeight ||
            objectPosition.y < height)
        {
            Debug.Log("-----------------------Inside IF");
        }
        else
        {
            Debug.Log("++++++++++++++++++++++++ outside IF");
        }*/
        //Debug.Log("width: " + objectPosition.x + " left: " + (width * 0.5f - objectWidth * 0.33f) + " right: " + (width * 1.5f - objectWidth * 1.3f));
        //Debug.Log("height: " + objectPosition.x + " left: " + (height * 0.32f - objectHeight) + " right: " + height);
        //Debug.Log("Width: " + objectWidth + "     Width * 0.5: " + objectWidth * 0.5f);
        //Debug.Log("position: " + objectPosition.x + ", " + objectPosition.y +
        //  "----, width low: " + (height * 0.32f - objectHeight) + " width high: " + (height));


        //Debug.Log(eventData.pointerDrag.name + " - was dropped on - " + gameObject.name);
    }
}
