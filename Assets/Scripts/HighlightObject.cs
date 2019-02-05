using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color originalColor;
    private Image image;

    public Sprite sprite;
    public Sprite spriteHighlight;
    public Material mat;

    public bool selected = false;

    public void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        selected = !selected;
        Selected(selected);
    }

    /// Only selectable if it is located in the setup screen editor area
    public void Selected(bool selected)
    {
        if (selected && this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            //image.sprite = spriteHighlight;
            image.material = mat;
        }
        else
        {
            //image.sprite = sprite;
            image.material = default;
        }
    }
}
