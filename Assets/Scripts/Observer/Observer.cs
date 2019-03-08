using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MessagingInterfaces;

/// <summary>
/// Handles the observer's controls and capabilities.
/// </summary>
public class Observer : MonoBehaviour, ILogging, IError
{
    public ObserverUI uiScript;

    /// <summary>
    /// From the IError interface.
    /// 
    /// TODO: Make this do something!
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void OnError(LoggingMessage message)
    {
        Debug.LogError(System.DateTime.Now.ToLongTimeString() + ": " + message.senderName + " - " + message.message);
    }

    /// <summary>
    /// From the ILogging interface.
    /// 
    /// TODO: Log to file and text box on the UI
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void OnLog(LoggingMessage message)
    {
        string msg = System.DateTime.Now.ToLongTimeString() + ": " + message.senderName + " - " + message.message;
        uiScript.AddLog(msg);

        Debug.Log(msg);
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        LogSaving.Destructor(null, null);
    }
#endif
}
