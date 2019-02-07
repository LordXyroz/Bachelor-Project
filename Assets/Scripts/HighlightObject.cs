using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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
    public Material mat;

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
        //throw new System.NotImplementedException();
        Selected();
    }

    public void Selected()
    {
        /// Only selectable if it is located in the SystemSetupScreen editor area
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            objectSelect.SelectObject(this.gameObject, mat);
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
