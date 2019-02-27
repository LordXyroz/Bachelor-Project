using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script is added to all drag-and-drop UI elements that are supposed to be selectable and highlighted
/// </summary>

public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("The objects that need to be referenced")]
    public SystemComponentMenu systemComponentMenu;
    private ReferenceLineMenu referenceLineMenu;
    private RectTransform componentMenu;
    private SelectedObject objectSelect;
    private TMP_Text currentToolTipText;
    private Canvas canvas;
    private DropZone dropZone;
    private SystemComponent systemComponent;
    private InformationColumn informationColumn;

    [Header("Visual properties for this object")]
    public GameObject selectionBox;
    public Sprite spriteDefault;
    public Sprite spriteHighlight;
    public string tooltipText;
    private Image image;
    private Image[] images;
    private Color originalColor;


    public void Start()
    {
        /// Need to differentiate between connections and components
        image = GetComponent<Image>();
        if (image != null)
        {
            originalColor = image.color;
            objectSelect = FindObjectOfType<SelectedObject>();

        }
        else
        {
            images = GetComponentsInChildren<Image>();
            originalColor = images[0].color;
            objectSelect = FindObjectOfType<SelectedObject>();
        }
        canvas = GetComponentInParent<Canvas>();
        currentToolTipText = canvas.transform.Find("TooltipText").GetComponent<TMP_Text>();
        currentToolTipText.text = "";

        informationColumn = canvas.GetComponentInChildren<InformationColumn>();

        componentMenu = canvas.transform.Find("SystemComponentMenu").gameObject.GetComponent<RectTransform>();
        systemComponentMenu = canvas.transform.GetComponentInChildren<SystemComponentMenu>(true);
        referenceLineMenu = canvas.transform.GetComponentInChildren<ReferenceLineMenu>(true);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)  // Component
        {
            image.color = Color.red;
            systemComponent = this.gameObject.GetComponent<SystemComponent>();
            informationColumn.PopulateInformationColumn(systemComponent.componentType, systemComponent.componentVulnerabilities, systemComponent.securityLevel);
        }
        else
        {
            foreach (Image img in images)   // Connection lines
            {
                img.color = Color.red;
                img.transform.parent.SetAsLastSibling();
            }
        }
        currentToolTipText.text = tooltipText;

    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)  // Component
        {
            image.color = originalColor;
        }
        else
        {
            foreach (Image img in images)   // Connection lines
            {
                img.color = originalColor;
            }
            if (objectSelect.selected != null)
            {
                objectSelect.selected.transform.SetAsLastSibling();
            }
        }
        currentToolTipText.text = "";
        informationColumn.ClearInformationColumn();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (componentMenu.gameObject.activeInHierarchy)
        {
            componentMenu.gameObject.SetActive(false);
        }
        if (referenceLineMenu.gameObject.activeInHierarchy)
        {
            referenceLineMenu.gameObject.SetActive(false);
        }

        /// Only selectable if it is located in the SystemSetupScreen editor area
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null
            || this.transform.parent.parent.gameObject.GetComponent<DropZone>() != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (image != null)
                {
                    objectSelect.SelectObject(this.gameObject, false, true);
                }
                else
                {
                    objectSelect.SelectObject(this.gameObject, false, false);
                }
            }

            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (objectSelect.selected != this.gameObject)
                {
                    if (image != null)
                    {
                        systemComponent = this.gameObject.GetComponent<SystemComponent>();
                        objectSelect.SelectObject(this.gameObject, false, true);
                    }
                    else
                    {
                        objectSelect.SelectObject(this.gameObject, false, false);
                    }
                }

                if (image != null)
                {

                    componentMenu.gameObject.SetActive(true);
                    systemComponentMenu.UpdatePosition(new Vector2(eventData.position.x + componentMenu.rect.width / 2,
                                                                eventData.position.y + componentMenu.rect.height / 2));
                    systemComponentMenu.SetMenuInformation(systemComponent.componentType, systemComponent.securityLevel);
                }
                else
                {
                    referenceLineMenu.gameObject.SetActive(true);
                    referenceLineMenu.UpdatePosition(new Vector2(eventData.position.x + componentMenu.rect.width / 2,
                                                                eventData.position.y + componentMenu.rect.height / 2));
                }
            }
        }
    }
}
