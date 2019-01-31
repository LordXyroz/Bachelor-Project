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
        gm.AddComponent<ServerTesting>();
        gm.AddComponent<ClientTesting>();
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
         /*
          if (isSpectator)*/
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
            GameObject.Find("GameManager").GetComponent<ClientTesting>().SendMessage(msg);
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

    /// <summary>
    /// CreateConnection will use the client script to create a connection to the server(host of the game).
    /// </summary>
    public void CreateConnection()
    {
        GameObject.Find("GameManager").GetComponent<ClientTesting>().ConnectToServer();
    }
}
