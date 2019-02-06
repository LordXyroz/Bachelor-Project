using UnityEngine;
using UnityEngine.UI;

public class SelectedObject : MonoBehaviour
{
    private HighlightObject highlight;
    private GameObject selected;
    private GameObject selectionBox;
    private Sprite sprite;

    private Image image;

    [SerializeField]
    private Image imageBox;
    //public Material mat;


    // Start is called before the first frame update
    void Start()
    {
        selected = null;
        imageBox = null;
        //mat = null;
    }

    public void SelectObject(GameObject newSelected, GameObject box, Material mat)
    {
        image = newSelected.GetComponent<Image>();
        imageBox = newSelected.transform.Find("selectionBox").GetComponent<Image>();

        if (newSelected != null)
        {
            if (selected == newSelected)
            {
                Debug.Log("De-selected: " + selected.name + "   -   New: " + newSelected.name);
                image.material = default;
                imageBox.gameObject.SetActive(false);
                selected = null;
                return;
            }
            selected = newSelected;
            imageBox.gameObject.SetActive(true);
            image.material = mat;
            //Debug.Log("Selected object is now " + newSelected.name);
        }



        //if (selected && newSelected.transform.parent.gameObject.GetComponent<DropZone>() != null)
        //{
        //    //image.sprite = spriteHighlight;
        //    image.material = mat;
        //    selectionBox = newSelected.GetComponentInChildren<Image>().gameObject;
        //    selectionBox.SetActive(true);
        //}
        //else
        //{
        //    //image.sprite = sprite;
        //    image.material = default;
        //    selectionBox.SetActive(false);
        //}
    }
}
