using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script is added to all drag-and-drop UI elements that are supposed to be selectable and highlighted
/// </summary>

public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("The objects that need to be referenced")]
    private SystemComponentMenu systemComponentMenu;
    private ReferenceLineMenu referenceLineMenu;
    private RectTransform componentMenu;
    private SelectedObject objectSelect;
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
        dropZone = FindObjectOfType<DropZone>();
        image = GetComponent<Image>();

        /// Need to differentiate between connections and components
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

        informationColumn = canvas.GetComponentInChildren<InformationColumn>();

        componentMenu = canvas.transform.GetComponentInChildren<SystemComponentMenu>(true).gameObject.GetComponent<RectTransform>();
        systemComponentMenu = canvas.transform.GetComponentInChildren<SystemComponentMenu>(true);
        referenceLineMenu = canvas.transform.GetComponentInChildren<ReferenceLineMenu>(true);
    }

    /// <summary>
    /// Change selectable object's color to red when hovered over
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)  // Component
        {
            image.color = Color.red;
            systemComponent = this.gameObject.GetComponent<SystemComponent>();
        }
        else
        {
            foreach (Image img in images)   // Connection lines
            {
                if (img.name != "Firewall")
                    img.color = Color.red;
                img.transform.parent.SetAsLastSibling();
            }
        }
    }

    /// <summary>
    /// Change selectable object's color back to the original color when no longer hovedr over
    /// </summary>
    /// <param name="eventData"></param>
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
    }

    /// <summary>
    /// Update what object is selected, activating it's selection box if it is a system component and changing it's color to the selection color
    /// </summary>
    /// <param name="eventData"></param>
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
                FindObjectOfType<MouseScript>().SelectObject(gameObject);

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

                float width = dropZone.GetComponent<RectTransform>().rect.width;
                float height = dropZone.GetComponent<RectTransform>().rect.height;
                float newX;
                float newY;

                if (image != null)
                {
                    componentMenu.gameObject.SetActive(true);
                    if (eventData.position.x + componentMenu.rect.width > width)
                    {
                        newX = eventData.position.x - componentMenu.rect.width / 2;
                    }
                    else
                    {
                        newX = eventData.position.x + componentMenu.rect.width / 2;
                    }
                    if (eventData.position.y + componentMenu.rect.height > height)
                    {
                        newY = eventData.position.y - componentMenu.rect.height / 2;
                    }
                    else
                    {
                        newY = eventData.position.y + componentMenu.rect.height / 2;
                    }
                    systemComponentMenu.UpdatePosition(new Vector2(newX, newY));
                    systemComponentMenu.SetMenuInformation(systemComponent.componentType, systemComponent.securityLevel);
                }
                else
                {
                    referenceLineMenu.gameObject.SetActive(true);
                    RectTransform ReferenceLineMenuRect = referenceLineMenu.gameObject.GetComponent<RectTransform>();
                    if (eventData.position.x + ReferenceLineMenuRect.rect.width > width)
                    {
                        newX = eventData.position.x - ReferenceLineMenuRect.rect.width / 2;
                    }
                    else
                    {
                        newX = eventData.position.x + ReferenceLineMenuRect.rect.width / 2;
                    }
                    if (eventData.position.y + ReferenceLineMenuRect.rect.height / 2 > height)
                    {
                        newY = eventData.position.y - ReferenceLineMenuRect.rect.height / 2;
                    }
                    else
                    {
                        newY = eventData.position.y + ReferenceLineMenuRect.rect.height / 2;
                    }
                    referenceLineMenu.UpdatePosition(new Vector2(newX, newY));
                    referenceLineMenu.PopulateReferenceLineMenu(objectSelect.selected);
                }
            }
        }
    }
}
