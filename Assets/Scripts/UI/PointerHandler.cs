using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipInfo;

    private AttackerUI uiScript;

    public void Start()
    {
        uiScript = FindObjectOfType<AttackerUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiScript.EnableToolTip(tooltipInfo, transform.position, GetComponent<RectTransform>().rect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiScript.DisableToolTip();
    }
}
