using TMPro;
using UnityEngine;

public class DefenderStatusField : MonoBehaviour
{

    public TextMeshProUGUI m_Text;
    public TextMeshProUGUI m_Timer;

    [SerializeField]
    public float m_TimerCount;
    public static bool m_TimerCountDown;
    public static bool m_TimerStarted = false;


    // Start is called before the first frame update
    void Start()
    {
        SetStatusText();
        m_TimerStarted = true;

        // If the timer is set to zero (or less), it will count up. otherwise, it will count up
        if (m_TimerCount <= 0)
        {
            m_TimerCount = 0;
            m_TimerCountDown = false;
        }
        else
        {
            m_TimerCountDown = true;
        }

        SetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        SetStatusText();

        if (m_TimerStarted)
        {
            if (m_TimerCountDown)
            {
                m_TimerCount -= Time.deltaTime;
            }
            else
            {
                m_TimerCount += Time.deltaTime;
            }
            SetTimer();
        }
    }

    void SetStatusText()
    {
        m_Text.text = "Status: " + "Defending";
    }

    void SetTimer()
    {
        /// In order to have a timer count down, set a start time (in seconds) and have "m_TimerCount -= Time.deltaTime;" in update

        float seconds = (m_TimerCount % 60);
        float minutes = Mathf.Floor(m_TimerCount / 60);
        float hours = Mathf.Floor(minutes / 60);

        if (m_TimerCount < 0)
        {
            m_Timer.text = "Time is out";
        }
        else
        {
            m_Timer.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }
}
