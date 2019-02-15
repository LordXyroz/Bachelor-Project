using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script is added to all drag-and-drop UI elements that are supposed to be selectable and highlighted
/// </summary>

public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("The objects that need to be refferenced")]
    private SelectedObject objectSelect;
    private TMP_Text currentToolTipText;
    private Canvas canvas;

    [Header("Visual properties for this object")]
    public GameObject selectionBox;
    private Image image;
    private Color originalColor;
    public Sprite spriteDefault;
    public Sprite spriteHighlight;

    [Header("The text displayed at the bottom of the screen for this object")]
    public string tooltipText;


    public void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        objectSelect = FindObjectOfType<SelectedObject>();
        canvas = GetComponentInParent<Canvas>();
        currentToolTipText = canvas.transform.Find("TooltipText").GetComponent<TMP_Text>();
        currentToolTipText.text = "";
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.red;
        currentToolTipText.text = tooltipText;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
        currentToolTipText.text = "";
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        /// Only selectable if it is located in the SystemSetupScreen editor area
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            objectSelect.SelectObject(this.gameObject, false);
        }
    }


    /// Clear the selected object, resetting it to default visuals
    void ClearSelection()
    {
        image.sprite = spriteDefault;
        image.material = default;
        selectionBox.SetActive(false);
    }
}
