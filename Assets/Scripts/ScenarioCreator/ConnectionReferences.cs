using UnityEngine;
using UnityEngine.UI;

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
        var fromBtn = referenceFromObject.GetComponentInChildren<Button>(true);
        var toBtn = referenceToObject.GetComponentInChildren<Button>(true);

        if (fromBtn == null || toBtn == null)
            return;

        if (fromBtn.gameObject.activeSelf &&
            toBtn.gameObject.activeSelf)
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

    /// <summary>
    /// Set the rferences for the connection line
    /// </summary>
    /// <param name="from">The system component the connection starts from</param>
    /// <param name="to">The system component the connection ends in</param>
    public void SetReferences(GameObject from, GameObject to)
    {
        referenceFromObject = from;
        referenceToObject = to;
    }

    /// <summary>
    /// Deletes a connection line, as well as its references in the connected system components
    /// </summary>
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
