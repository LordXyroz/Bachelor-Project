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
    private Toggle firewallToggle;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        selectedObject = canvas.transform.Find("Scripts").GetComponent<SelectedObject>();

        firewallToggle = GetComponentInChildren<Toggle>(true);
        firewallToggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(firewallToggle);
        });
    }


    private void ToggleValueChanged(Toggle firewall)
    {
        selectedObject.selected.gameObject.GetComponent<ConnectionReferences>().hasFirewall = firewall.isOn;
    }


    public void PopulateReferenceLineMenu(GameObject recievedSelectedObject)
    {
        firewallToggle = GetComponentInChildren<Toggle>(true);
        firewallToggle.isOn = recievedSelectedObject.transform.GetComponent<ConnectionReferences>().hasFirewall;
    }


    public void UpdatePosition(Vector2 pos)
    {
        this.gameObject.GetComponent<RectTransform>().transform.position = pos;
    }
}
