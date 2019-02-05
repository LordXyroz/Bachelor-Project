using System.Collections;
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

        /*GameObject gm = GameObject.Find("GameManager");
        gm.AddComponent<ServerBehaviour>();
        gm.AddComponent<ClientBehaviour>();*/
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
            if (gm.GetComponent<ServerBehaviour>() == null)
                gm.AddComponent<ServerBehaviour>();
        }
        else
        {
            /// Add client behaviour and try to find a host.

            // Server here is used for testing.
            if (gm.GetComponent<ClientBehaviour>() == null)
            {
                gm.AddComponent<ClientBehaviour>();
                
                StartCoroutine(WaitForConnection());
            }
            else
            {
                /// Make client connect or disconnect to a host found:
                GameObject.Find("GameManager").GetComponent<ClientBehaviour>().ConnectOrDisconnect();
            }
        }
    }


    /// <summary>
    /// WaitForConnection is used for clients that needs to wait a moment for the client behaviour to be set before trying to connect to host.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForConnection()
    {
        yield return new WaitForSeconds(0.01f);
        /// Make client connect or disconnect to a host found:
        GameObject.Find("GameManager").GetComponent<ClientBehaviour>().ConnectOrDisconnect();
    }

    /// <summary>
    /// SendMessage will use the Message given through parameter(temporarily set as string, will be of type 'Message' later)
    /// to send a message that the gamestate has changed through the network to update the versions of other players in the game.
    /// </summary>
    public void SendChatMessage()
    {
        if (!isSpectator)
        {
            string msg = GameObject.Find("InputField").GetComponent<InputField>().text;
            GameObject.Find("GameManager").GetComponent<ClientBehaviour>().SendChatMessage(msg);
        }
    }

    /// <summary>
    /// SendScenario is an example of how to send a scenario through the network so all players have updated version of a match.
    /// TODO when this branch is merged with the branch that as scenario types that will be sent as parameters.
    /// </summary>
    public void SendScenario()
    {
        if (!isSpectator)
        {
            GameObject.Find("GameManager").GetComponent<ClientBehaviour>().SendScenario();
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
}
