﻿using UnityEngine;


/// <summary>
/// Keeps the references to the connected GameObjects
/// </summary>
public class ConnectionReferences : MonoBehaviour
{
    [Header("Attributes for this reference line")]
    public bool hasFirewall = false;

    [Header("The objects connected")]
    public GameObject referenceFromObject;
    public GameObject referenceToObject;

    [Header("Items needed for deleting references to the connecting line")]
    private SystemComponent systemComponent;

    [Header("Line objects")]
    public GameObject lineFromStart;
    public GameObject lineToEnd;
    public GameObject firewall;

    public void Update()
    {
        if (referenceFromObject.activeSelf && referenceToObject.activeSelf)
        {
            lineFromStart.SetActive(true);
            lineToEnd.SetActive(true);
            if (hasFirewall)
                firewall.SetActive(true);
        }
        else
        {
            lineFromStart.SetActive(false);
            lineToEnd.SetActive(false);
            firewall.SetActive(false);
        }
    }

    public void SetReferences(GameObject from, GameObject to)
    {
        referenceFromObject = from;
        referenceToObject = to;
    }


    public void RemoveConnectionComponent()
    {
        systemComponent = referenceFromObject.GetComponent<SystemComponent>();
        systemComponent.connectedReferenceLines.Remove(this.gameObject);
        systemComponent = referenceToObject.GetComponent<SystemComponent>();
        systemComponent.connectedReferenceLines.Remove(this.gameObject);
        FindObjectOfType<SelectedObject>().connectionReferencesList.Remove(this.gameObject);
        hasFirewall = false;
        Destroy(this.gameObject);
    }
}
