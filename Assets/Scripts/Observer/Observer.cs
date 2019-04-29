using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MessagingInterfaces;
using System.Linq;

/// <summary>
/// Handles the observer's controls and capabilities.
/// </summary>
public class Observer : MonoBehaviour, ILogging, IError
{
    public ObserverUI uiScript;

    [SerializeField]
    private List<ObserverNodeInfo> nodes = new List<ObserverNodeInfo>();

    private GameObject target;

    /// <summary>
    /// Builds the nodes list with info.
    /// </summary>
    public void Start()
    {
        foreach (var o in GameObject.FindGameObjectsWithTag("Node"))
            UpdateNode(o);

        ClearTarget();
        uiScript.EnableLogPanel();
    }

    /// <summary>
    /// Function from <see cref="MessagingInterfaces.IError"/>.
    /// Currently unused.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void OnError(LoggingMessage message)
    {
        Debug.LogError(System.DateTime.Now.ToLongTimeString() + ": " + message.senderName + " - " + message.message);
    }

    /// <summary>
    /// Function from <see cref="MessagingInterfaces.ILogging"/>.
    /// </summary>
    /// <param name="message">Message containing relevant info to be handled by the function</param>
    public void OnLog(LoggingMessage message)
    {
        string msg = System.DateTime.Now.ToLongTimeString() + ": " + message.senderName + " - " + message.message;
        uiScript.AddLog(msg);

        Debug.Log(msg);
    }

    /// <summary>
    /// Updates a node within the list, or creates a new entry if it doesn't exist,
    /// and updates it's values.
    /// </summary>
    /// <param name="go">The game object (with attached GameNetworkComponent) to update from</param>
    public void UpdateNode(GameObject go)
    {
        if (target != null)
            target.GetComponent<GameNetworkComponent>().selectionBox.SetActive(false);

        target = go;

        target.GetComponent<GameNetworkComponent>().selectionBox.SetActive(true);

        ObserverNodeInfo node = nodes.FirstOrDefault(x => x.component.name == go.name);
        if (node == null)
        {
            node = new ObserverNodeInfo(target);
            nodes.Add(node);
        }

        var component = target.GetComponent<GameNetworkComponent>();

        node.difficulty = component.difficulty;
        node.numOfChildren = component.children.Count;
        node.numOfVulnerabilities = component.vulnerabilities.Count;

        node.vulnerabilities = component.vulnerabilities;
        node.availableDefenses = component.availableDefenses;
        node.implementedDefenses = component.implementedDefenses;

        node.upToDate = true;
        node.beenProbed = true;
        node.beenDiscoveredOn = true;
        node.beenAnalyzed = true;

        uiScript.PopulateInfoPanel(node);
    }

    /// <summary>
    /// Disables slection box and sets target to be null.
    /// </summary>
    public void ClearTarget()
    {
        if (target != null) 
            target.GetComponent<GameNetworkComponent>().selectionBox.SetActive(false);
        target = null;
    }

    /// <summary>
    /// Only exists in code when using the Unity Editor.
    /// Makes sure the destructor for the LogSaving static class runs.
    /// </summary>
#if UNITY_EDITOR
    private void OnDestroy()
    {
        LogSaving.Destructor(null, null);
    }
#endif
}
