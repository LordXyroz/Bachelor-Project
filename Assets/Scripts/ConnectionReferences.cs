using UnityEngine;

public class ConnectionReferences : MonoBehaviour
{
    [Header("The objects connected")]
    public GameObject referenceFromObject;
    public GameObject referenceToObject;


    public void SetReferences(GameObject from, GameObject to)
    {
        referenceFromObject = from;
        referenceToObject = to;
    }


    public void RemoveConnectionComponent()
    {

    }
}
