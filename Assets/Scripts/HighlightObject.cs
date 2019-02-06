using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color originalColor;
    private Image image;

    private SelectedObject objectSelect;

    public GameObject selectionBox;
    public Sprite sprite;
    public Sprite spriteHighlight;
    public Material mat;

    public bool selected = false;

    public void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        objectSelect = FindObjectOfType<SelectedObject>();
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
        Selected();
    }

    /// Only selectable if it is located in the SystemSetupScreen editor area
    public void Selected()
    {
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            ClearSelection();
            objectSelect.SelectObject(this.gameObject, selectionBox, mat);
        }
        else
        {
            ClearSelection();
        }
    }

    /// Clear the selected object, resetting it to default visuals
    void ClearSelection()
    {
        image.sprite = sprite;
        image.material = default;
        selectionBox.SetActive(false);
    }
}
