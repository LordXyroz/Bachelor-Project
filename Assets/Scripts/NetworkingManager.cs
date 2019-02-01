using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviour
{

    public bool isSpectator;

    /// <summary>
    /// Create the server script or clientscript based upon what is needed.
    /// </summary>
    void Start()
    {

        /// For now just create server and client on the same computer:

        GameObject gm = GameObject.Find("GameManager");
        gm.AddComponent<ServerBehaviour>();
        gm.AddComponent<ClientBehaviour>();
    }


    public void SetupNetworkingManager(bool isSpec)
    {
        isSpectator = isSpec;
        /*
         * TODO
         * 
         * This function will add client or server script based upon bool sent in.
         * This will either make the client be able to send and get messages as an attacker/defender
         * or receive messages as a host/spectator of the game.
         */
        GameObject gm = GameObject.Find("GameManager");
        if (isSpectator)
        {
            gm.AddComponent<ServerBehaviour>();
        }
        else
        {
            // Server here is used for testing.
            //gm.AddComponent<ServerBehaviour>();
            gm.AddComponent<ClientBehaviour>();
        }
    }
    

    /// <summary>
    /// SendMessage will use the Message given through parameter(temporarily set as string, will be of type 'Message' later)
    /// to send a message that the gamestate has changed through the network to update the versions of other players in the game.
    /// </summary>
    public void SendMessage()
    {
        if (!isSpectator)
        {
            string msg = GameObject.Find("InputField").GetComponent<InputField>().text;
            GameObject.Find("GameManager").GetComponent<ClientBehaviour>().SendMessage(msg);
        }
    }

    /// <summary>
    /// GetMessage will receive message collected through the server/client script.
    /// </summary>
    /// <param name="msg"></param>
    public void GetMessage(string msg)
    {
        GameObject.Find("MessageText").GetComponent<Text>().text = msg;
    }

    public void GetHostMessage(string msg)
    {
        Debug.Log("Host message: " + msg);
    }

    /// <summary>
    /// CreateConnection will use the client script to create a connection to the server(host of the game).
    /// </summary>
    public void CreateConnection()
    {
        if (!isSpectator)
        {
            GameObject.Find("GameManager").GetComponent<ClientBehaviour>().ConnectOrDisconnect();
        }
    }
}
