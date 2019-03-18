using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Simple class that uses IPointerEnterHandler and IPointerExitHandler to call
/// uiScript.EnableToolTip() to display tooltips when a UI element has a pointer
/// hovering over it.
/// </summary>
public class PointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipInfo;

    private BaseUI uiScript;

    /// <summary>
    /// Finds the script that controlls the UI on start.
    /// </summary>
    public void Start()
    {
        uiScript = FindObjectOfType<PlayerManager>().GetUIScript();
    }

    /// <summary>
    ///  When the pointer enters the UI element's boundraries.
    /// </summary>
    /// <param name="eventData">Event payload assosiated with pointer (mouse/touch) events.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        uiScript.EnableToolTip(tooltipInfo, transform.position, GetComponent<RectTransform>().rect);
    }

    /// <summary>
    /// When the pointer exits the UI element's boundraries.
    /// </summary>
    /// <param name="eventData">Event payload assosiated with pointer (mouse/touch) events.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        uiScript.DisableToolTip();
    }
}
