using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is to be places on all system components
/// </summary>

public class SystemComponent : MonoBehaviour
{
    [Header("List of reference lines connected to this system component")]
    public List<GameObject> connectedReferenceLines;

    private void Start()
    {
        connectedReferenceLines = new List<GameObject>();
    }

    public void AddReference(GameObject reference)
    {
        connectedReferenceLines.Add(reference);
    }
}
