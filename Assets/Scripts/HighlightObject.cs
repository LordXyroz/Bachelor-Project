﻿using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script is added to all drag-and-drop UI elements that are supposed to be selectable and highlighted
/// </summary>

public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("The objects that need to be referenced")]
    private SelectedObject objectSelect;
    private TMP_Text currentToolTipText;
    private Canvas canvas;

    [Header("Visual properties for this object")]
    public GameObject selectionBox;
    public Sprite spriteDefault;
    public Sprite spriteHighlight;
    private Image image;
    private Image[] images;
    private Color originalColor;
    public string tooltipText;


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
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)  // Component
        {
            image.color = Color.red;
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
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        /// Only selectable if it is located in the SystemSetupScreen editor area
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null
            || this.transform.parent.parent.gameObject.GetComponent<DropZone>() != null)
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

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            /*/// Only selectable if it is located in the SystemSetupScreen editor area
            if (this.transform.parent.gameObject.GetComponent<DropZone>() != null
                || this.transform.parent.parent.gameObject.GetComponent<DropZone>() != null)
            {


                GameObject componentMenu = this.gameObject.transform.Find("SelectedObjectMenu").gameObject;
                Debug.Log("Activating game object: " + componentMenu.name);
                componentMenu.SetActive(true);
                if (image != null)
                {
                    objectSelect.SelectObject(this.gameObject, false, true);
                }
                else
                {
                    objectSelect.SelectObject(this.gameObject, false, false);
                }
            }*/
        }

    }
}