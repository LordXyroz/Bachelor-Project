using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(MeshRenderer))]
public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Material mat;
    public Color originalColor;
    public Image image;

    public void Start()
    {
        mat = GetComponent<Image>().material;
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //mat.color = Color.red;
        image.color = Color.red;
        Debug.Log("Cursor Entering--- " + name + " ---GameObject");
    }

    public void OnMouseOver()
    {
        mat.color -= new Color(0.1f, 0, 0) * Time.deltaTime;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //mat.color = originalColor;
        image.color = originalColor;
        Debug.Log("Cursor Exiting--- " + name + " ---GameObject");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
