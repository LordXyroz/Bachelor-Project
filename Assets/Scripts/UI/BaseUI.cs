using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public abstract class BaseUI : MonoBehaviour
{
    [Header("Progressbar")]
    public GameObject progressbarObject;
    public Image progressbar;
    public Text progressbarTitle;
    public Text progressbarText;

    [Header("OnClickMenu")]
    public GameObject onClickMenu;
    
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

    [Header("Top panel")]
    public Button quitButton;

    /// <summary>
    /// To be implemented by sub classes
    /// </summary>
    public abstract void Start();

    /// <summary>
    /// To be implemented by sub classes
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// To be implemented by sub classes
    /// </summary>
    public abstract void EnableStatsPanel();

    /// <summary>
    /// To be implemented by sub classes
    /// </summary>
    public abstract void EnableInfoPanel();

    /// <summary>
    /// To be implemented by sub classes
    /// </summary>
    public abstract void EnableActionPanel();

    /// <summary>
    /// To be implemented by sub classes
    /// </summary>Info to populate the panel with</param>
    public abstract void PopulateInfoPanel(NodeInfo info);

    /// <summary>
    /// Enables the tooltip box, and positions it with an offset.
    /// Changes allignment and direction based on if pos is on the left or right side of the screen.
    /// </summary>
    /// <param name="text">Text to be displayed</param>
    /// <param name="pos">Where to place tooltip</param>
    /// <param name="rect">Rect of the UI element used for width/height offset</param>
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

        if (pos.y <= Screen.height / 2f)
        {
            yOffset = (yOffset * -1);
        }

        Vector2 newPos = new Vector2(pos.x + xOffset, pos.y - yOffset);

        tooltipObject.GetComponent<RectTransform>().position = newPos;
        tooltipObject.SetActive(true);
    }

    /// <summary>
    /// Enables/Disables the progressbar popup, and sets the texts to display.
    /// </summary>
    /// <param name="toggle">Whether to enable or disable</param>
    /// <param name="title">Title text</param>
    /// <param name="text">Info text</param>
    public void ToggleProgressbar(bool toggle, string title, string text)
    {
        progressbarTitle.text = title;
        progressbarText.text = text;

        progressbarObject.SetActive(toggle);
    }

    /// <summary>
    /// Toggles the OnClick menu, and moves it to a target position
    /// </summary>
    /// <param name="toggle">Whether to enable or disable</param>
    /// <param name="pos">Position to move to</param>
    public void ToggleOnClickMenu(bool toggle, Vector3 pos)
    {
        if (popupWindowObject.activeSelf || progressbarObject.activeSelf)
            return;

        onClickMenu.SetActive(toggle);
        onClickMenu.GetComponent<RectTransform>().position = pos;
    }

    /// <summary>
    /// Toggles the popup window for displaying success/fail messages etc...
    /// </summary>
    /// <param name="toggle">Whether to toggle on or off</param>
    /// <param name="title">Title text of the window</param>
    /// <param name="text">Text message</param>
    public void TogglePopupWindow(bool toggle, string title, string text)
    {
        popupWindowTitle.text = title;
        popupWindowText.text = text;

        popupWindowObject.SetActive(toggle);
    }

    /// <summary>
    /// Fills the progressbar an amount based on current and max value
    /// </summary>
    /// <param name="value">Current value</param>
    /// <param name="max">Max value</param>
    public void UpdateProgressbar(float value, float max)
    {
        progressbar.fillAmount = value / max;
    }

    /// <summary>
    /// Disables the OnClick menu
    /// </summary>
    public void DisableOnClickMenu()
    {
        onClickMenu.SetActive(false);
    }

    /// <summary>
    /// Hides popup window
    /// </summary>
    public void DisablePopupWindow()
    {
        popupWindowObject.SetActive(false);
    }

    /// <summary>
    /// Hides tooltip
    /// </summary>
    public void DisableToolTip()
    {
        tooltipObject.SetActive(false);
    }
    
}
