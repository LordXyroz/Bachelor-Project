using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class AttackerUI : MonoBehaviour
{
    [Header("Progressbar")]
    public GameObject progressbarObject;
    public Image progressbar;
    public Text progressbarTitle;
    public Text progressbarText;

    [Header("OnClickMenu")]
    public GameObject onClickMenu;

    [Header("InfoPanel")]
    public GameObject infoPanel;
    public GameObject infoPanelArea;
    public Text targetText;
    public Text probeText;
    public Text analyzeText;
    public Text discoverText;
    public Text difficultyText;
    public Text nodesText;
    public Text numVulnText;
    public GameObject vulnPrefab;

    [Header("AttackPanel")]
    public GameObject attackPanel;
    public GameObject attackPanelArea;
    public GameObject attackButtonPrefab;

    public void Start()
    {
        List<AttackTypes> attacks = VulnerabilityPairings.GetAllAttacks();
        foreach (var a in attacks)
        {
            GameObject go = Instantiate(attackButtonPrefab, attackPanelArea.transform);

            go.GetComponentInChildren<Text>().text = VulnerabilityPairings.GetAttackString(a);
            go.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<Attacker>().StartAttack((int) a - 1));
            go.GetComponent<Button>().onClick.AddListener(DisableAttackPanel);
        }

        Debug.Log(GetComponentsInChildren<Transform>().Length);
    }

    public void UpdateProgressbar(float value, float max)
    {
        progressbar.fillAmount = value / max;
    }

    public void ToggleProgressbar(bool toggle, string title, string text)
    {
        progressbarObject.SetActive(toggle);

        progressbarTitle.text = title;
        progressbarText.text = text;
    }

    public void ToggleOnClickMenu(bool toggle, Vector3 pos)
    {
        onClickMenu.SetActive(toggle);
        onClickMenu.GetComponent<RectTransform>().localPosition = pos;
    }

    public void DisableOnClickMenu()
    {
        onClickMenu.SetActive(false);
    }
    
    public void PopulateInfoPanel(AttackerInfo info)
    {
        infoPanel.SetActive(true);

        foreach (Transform child in infoPanelArea.transform)
            if (child.CompareTag("VulnBox"))
                Destroy(child.gameObject);

        targetText.text = info.component.name;
        probeText.text = (info.beenProbed) ? "Yes" : "No";
        analyzeText.text = (info.beenAnalyzed) ? "Yes" : "No";
        discoverText.text = (info.beenDiscoveredOn) ? "Yes" : "No";

        difficultyText.text = (info.difficulty == -1) ? "Unknown" : info.difficulty.ToString();
        nodesText.text = (info.numOfChildren == -1) ? "Unknown" : info.numOfChildren.ToString();
        numVulnText.text = (info.numOfVulnerabilities == -1) ? "Unknown" : info.numOfVulnerabilities.ToString();

        foreach (var v in info.vulnerabilities)
        {
            GameObject go = Instantiate(vulnPrefab, infoPanelArea.transform);
            Text t = go.GetComponentsInChildren<Text>().First(x => x.CompareTag("VarText"));
            t.text = VulnerabilityPairings.GetAttackString(v);
        }
    }

    public void DisableInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    public void EnableAttackPanel()
    {
        attackPanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    public void DisableAttackPanel()
    {
        attackPanel.SetActive(false);
        infoPanel.SetActive(true);
    }
}
 