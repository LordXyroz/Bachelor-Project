using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(MeshRenderer))]
public class HighlightObject : MonoBehaviour
{
    public float animationTime = 1f;
    public float threshold = 1.5f;

    private Material material;
    //public RectTransform uGuiElement;
    private Color normalColor;
    private Color highlightColor;

    private void Awake()
    {
        material = GetComponent<Image>().material;
        normalColor = material.color;

        highlightColor = Color.green;
        //new Color(
        //   Mathf.Clamp01(normalColor.r * threshold),
        //   Mathf.Clamp01(normalColor.g * threshold),
        //   Mathf.Clamp01(normalColor.b * threshold)
        //   );
    }

    private void Start()
    {
        //highlightColor = Color.green;
        StartHighlight();
    }


    public void StartHighlight()
    {
        if (this.transform.parent.gameObject.GetComponent<DropZone>() != null)
        {
            material.color = highlightColor;
        }
        //iTween.ColorTo(gameObject, iTween.Hash(
        //    "color", highlightColor,
        //    "time", animationTime,
        //    "easetype", iTween.EaseType.linear,
        //    "looptype", iTween.LoopType.pingPong
        //    ));
    }

    public void StopHighlight()
    {
        iTween.Stop(gameObject);
        material.color = normalColor;
    }

}
