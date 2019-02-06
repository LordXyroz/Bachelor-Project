using UnityEngine;
using UnityEngine.UI;

public class SelectedObject : MonoBehaviour
{
    private GameObject selected;
    private GameObject selectionBox;
    private Image image;
    private Image imageBox;

    void Start()
    {
        selected = null;
        imageBox = null;
    }

    public void SelectObject(GameObject newSelected, Material mat)
    {
        /// Reset the selected properties of the previously selected object, if there is one
        if (selected != null)
        {
            image.material = default;
            imageBox.gameObject.SetActive(false);
        }

        if (newSelected != null)
        {
            /// Reset and set active to null if the selected object is clicked again
            if (selected == newSelected)
            {
                image.material = default;
                imageBox.gameObject.SetActive(false);
                selected = null;
                return;
            }
            image = newSelected.GetComponent<Image>();
            imageBox = newSelected.transform.Find("selectionBox").GetComponent<Image>();

            selected = newSelected;
            image.material = mat;
            imageBox.gameObject.SetActive(true);
            //Debug.Log("Selected object is now " + newSelected.name);
        }
    }
}
