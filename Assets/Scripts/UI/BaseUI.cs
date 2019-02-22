using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUI : MonoBehaviour
{
    [Header("Progressbar")]
    public GameObject progressbarObject;
    public Image progressbar;
    public Text progressbarTitle;
    public Text progressbarText;

    [Header("OnClickMenu")]
    public GameObject onClickMenu;

    [Header("InfoPanel")]
    public GameObject infoPanelObject;
    public GameObject infoPanelArea;
    public Text targetText;
    public Text probeText;
    public Text analyzeText;
    public Text discoverText;
    public Text difficultyText;
    public Text nodesText;
    public Text numVulnText;
    public GameObject vulnPrefab;

    [Header("Popup Window")]
    public GameObject popupWindowObject;
    public Text popupWindowTitle;
    public Text popupWindowText;

    [Header("Stats Window")]
    public GameObject statsPanelObject;
    public Text statsResourcesText;
    public Text statsAtkDefLvlText;
    public Text statsAnalyzeLvlText;
    public Text statsDiscoverLvlText;

    [Header("Actions Window")]
    public GameObject actionsPanelObject;

    [Header("Tooltip")]
    public GameObject tooltipObject;
    public RectTransform tooltipFrameTransform;
    public Text tooltipText;
    protected const float refHeight = 1080;
    protected const float refWidth = 1920;

    // Start is called before the first frame update
    public abstract void Start();

    // Update is called once per frame
    public abstract void Update();

    public abstract void EnableStatsPanel();
    public abstract void EnableInfoPanel();
    public abstract void EnableActionPanel();

    public void EnableToolTip(string text, Vector2 pos, Rect rect)
    {
        tooltipText.text = text;

        float xOffset = (rect.width / 2f) * (Screen.width / refWidth);
        float yOffset = rect.height * (Screen.height / refHeight);

        if (pos.x >= Screen.width / 2f)
        {
            xOffset = (xOffset * -1);
            tooltipFrameTransform.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            tooltipFrameTransform.pivot = new Vector2(0f, 0.5f);
        }

        Vector2 newPos = new Vector2(pos.x + xOffset, pos.y - yOffset);

        tooltipObject.GetComponent<RectTransform>().position = newPos;
        tooltipObject.SetActive(true);
    }

    public void ToggleProgressbar(bool toggle, string title, string text)
    {
        progressbarTitle.text = title;
        progressbarText.text = text;

        progressbarObject.SetActive(toggle);
    }

    public void ToggleOnClickMenu(bool toggle, Vector3 pos)
    {
        if (popupWindowObject.activeSelf || progressbarObject.activeSelf)
            return;

        onClickMenu.SetActive(toggle);
        onClickMenu.GetComponent<RectTransform>().position = pos;
    }

    public void TogglePopupWindow(bool toggle, string title, string text)
    {
        popupWindowTitle.text = title;
        popupWindowText.text = text;

        popupWindowObject.SetActive(toggle);
    }

    public abstract void PopulateInfoPanel(NodeInfo info);
    public abstract void UpdateProgressbar(float value, float max);
    public abstract void UpdateStats(int res, int atkdefLvl, int analyzeLvl, int discoverLvl);

    public void DisableOnClickMenu()
    {
        onClickMenu.SetActive(false);
    }
    public void DisablePopupWindow()
    {
        popupWindowObject.SetActive(false);
    }
    public void DisableToolTip()
    {
        tooltipObject.SetActive(false);
    }
}
