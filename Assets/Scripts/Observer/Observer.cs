using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour, ILogging
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //
        
    }

    public void OnLog(LoggingMessage message)
    {
        Debug.Log(System.DateTime.Now.ToLongTimeString() + ": " + message.senderName + " - " + message.message);
    }
}
