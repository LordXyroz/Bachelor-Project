using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(MeshRenderer))]
public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color originalColor;
    private Image image;

    public Sprite sprite;
    public Sprite spriteHighlight;

    public bool selected = false;

    public void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.red;
        //Debug.Log("Cursor Entering--- " + name + " ---GameObject");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
        //Debug.Log("Cursor Exiting--- " + name + " ---GameObject");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        selected = !selected;
        Selected(selected);
    }

    public void Selected(bool selected)
    {
        if (selected)
        {
            image.sprite = spriteHighlight;
        }
        else
        {
            image.sprite = sprite;
        }
    }
}
