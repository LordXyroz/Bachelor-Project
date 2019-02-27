using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script put on the right-click menu for reference lines
/// </summary>
public class ReferenceLineMenu : MonoBehaviour
{
    [Header("Populating the reference menu")]
    public SelectedObject selectedObject;
    private Canvas canvas;
    private Toggle firewall;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        selectedObject = canvas.transform.Find("Scripts").GetComponent<SelectedObject>();

        firewall = GetComponentInChildren<Toggle>(true);
        firewall.isOn = false;
        firewall.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(firewall);
        });
    }

    private void ToggleValueChanged(Toggle firewall)
    {
        selectedObject.selected.gameObject.GetComponent<ConnectionReferences>().hasFirewall = firewall.isOn;
    }

    public void UpdatePosition(Vector2 pos)
    {
        this.gameObject.GetComponent<RectTransform>().transform.position = pos;
    }
}
