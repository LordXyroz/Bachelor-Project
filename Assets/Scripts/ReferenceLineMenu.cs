using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script put on the right-click menu for reference lines
/// </summary>
public class ReferenceLineMenu : MonoBehaviour
{
    [Header("Populating the reference menu")]
    public SelectedObject selectedObject;
    public Toggle firewallToggle;
    private Canvas canvas;
    private GameObject firewall;
    private Transform lineToEnd;
    private Transform lineFromStart;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        selectedObject = canvas.transform.Find("Scripts").GetComponent<SelectedObject>();

        firewallToggle = canvas.GetComponentInChildren<ReferenceLineMenu>().GetComponentInChildren<Toggle>(true);
        if (firewallToggle != null)
        {
            firewallToggle.onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(firewallToggle);
            });
        }
    }


    private void ToggleValueChanged(Toggle firewallToggle)
    {
        selectedObject.selected.gameObject.GetComponent<ConnectionReferences>().hasFirewall = firewallToggle.isOn;
        lineToEnd = selectedObject.selected.gameObject.transform.Find("LineToEnd").GetComponent<RectTransform>().transform;
        lineFromStart = selectedObject.selected.gameObject.transform.Find("LineFromStart").GetComponent<RectTransform>().transform;
        firewall = selectedObject.selected.gameObject.transform.Find("Firewall").gameObject;
        UpdateFirewall(selectedObject.selected.gameObject);
    }


    public void LoadFirewall(List<GameObject> connections)
    {
        //canvas = GetComponentInParent<Canvas>();
        foreach (GameObject line in connections)
        {
            //lineToEnd = line.gameObject.transform.Find("LineToEnd").GetComponent<RectTransform>().transform;
            //lineFromStart = line.gameObject.transform.Find("LineFromStart").GetComponent<RectTransform>().transform;

            if (line.GetComponent<ConnectionReferences>().hasFirewall)
            {
                firewall = line.gameObject.transform.Find("Firewall").gameObject;
                firewall.SetActive(true);
                UpdateFirewall(line);
            }
        }
    }


    public void UpdateFirewall(GameObject line)
    {
        lineToEnd = line.gameObject.transform.Find("LineToEnd").GetComponent<RectTransform>().transform;
        lineFromStart = line.gameObject.transform.Find("LineFromStart").GetComponent<RectTransform>().transform;
        firewall = line.gameObject.transform.Find("Firewall").gameObject;

        if (line.GetComponent<ConnectionReferences>().hasFirewall
            && lineToEnd != null)
        {
            /// Place the firewall icon on the longest vector/line
            if (lineToEnd.localScale.magnitude > lineFromStart.localScale.magnitude)
            {
                firewall.transform.position = lineToEnd.position;
            }
            else
            {
                firewall.transform.position = lineFromStart.position;
            }

            /// Need to rotate the firewall if both are negative
            if (firewall.transform.position.x < 0 && firewall.transform.position.y < 0)
            {
                firewall.transform.rotation = new Quaternion(0, 0, 90, 0);
            }
            else
            {
                firewall.transform.rotation = Quaternion.identity;
            }
            firewall.SetActive(true);
        }
        else if (firewall != null)
        {
            firewall.SetActive(false);
        }
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
