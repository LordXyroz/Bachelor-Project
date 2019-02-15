using UnityEngine;


/// <summary>
/// Keeps the references to the connected GameObjects
/// </summary>
public class ConnectionReferences : MonoBehaviour
{
    [Header("The objects connected")]
    public GameObject referenceFromObject;
    public GameObject referenceToObject;


    /// <summary>
    /// Set the references to the two GameObjects the line is connecting
    /// </summary>
    /// <param name="from">The GameObject the connecting line originates from</param>
    /// <param name="to">The GameObject the connecting line ends in</param>
    public void SetReferences(GameObject from, GameObject to)
    {
        referenceFromObject = from;
        referenceToObject = to;
    }


    /// <summary>
    /// Remove the references to the selected connection line
    /// </summary>
    public void RemoveConnectionComponent()
    {

    }
}
