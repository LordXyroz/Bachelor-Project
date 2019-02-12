using UnityEngine;

public class MakeConnection : MonoBehaviour
{

    private Vector2 startPosition;
    private Vector2 endPosition;
    public GameObject connectionLine;

    private SelectedObject selected;

    // Start is called before the first frame update
    void Start()
    {
        //selected = 
    }

    /*public void StartConnection()
    {
        float objectWidth = selected.GetComponent<RectTransform>().rect.width;
        float objectHeight = selected.GetComponent<RectTransform>().rect.height;

        startPosition.x = selected.transform.position.x - (objectWidth * 0.33f);
        startPosition.y = selected.transform.position.y - -objectHeight;

        endPosition.x = selected.transform.position.x - objectWidth * 1.3f;
        endPosition.y = selected.transform.position.y - 10;

        GameObject connectionLineClone = Instantiate(connectionLine, selected.transform);
    }*/
}
