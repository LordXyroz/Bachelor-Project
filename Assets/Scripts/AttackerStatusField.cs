using TMPro;
using UnityEngine;


/*  ----------------TODO---------------
 * 
 * Hover over exploits in order to view information.
 * Exploits hve levels to them (i.e. 2. broken authentication has 1. brute force 2. Credential stuffing + dictionary 3. tools)
 * 
 * 
 * 
 * 
 * 
 */

public class AttackerStatusField : MonoBehaviour
{

    public TextMeshProUGUI m_Text;
    public TextMeshProUGUI m_Timer;

    public static float m_TimerCount;
    public static bool m_TimerStarted = false;



    // Start is called before the first frame update
    void Start()
    {
        SetStatusText();
        m_TimerStarted = true;
        m_TimerCount = 0;
        SetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        SetStatusText();

        if (m_TimerStarted)
        {
            m_TimerCount += Time.deltaTime;
            SetTimer();
        }
    }

    void SetStatusText()
    {
        m_Text.text = "Status: " + "Attacking";
    }

    void SetTimer()
    {
        float minutes = Mathf.Floor(m_TimerCount / 60);
        float seconds = (m_TimerCount % 60);


        m_Timer.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
