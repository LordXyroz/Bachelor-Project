using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Adds function call on right click on a ui element.
/// </summary>
public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// The function to be called. Must be set through inspector or script.
    /// </summary>
    public UnityAction onRight;

    /// <summary>
    /// Event function that's called on click events.
    /// Invokes onRight action if the event is a right click.
    /// </summary>
    /// <param name="eventData">Contains data from the event</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRight.Invoke();
        }
    }
}
