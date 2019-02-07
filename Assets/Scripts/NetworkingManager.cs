using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviour
{

    public bool isSpectator;
    public GameObject chatField;
    public GameObject connectionField;

    void Start()
    {
        connectionField = GameObject.Find("ConnectionField");
        chatField = GameObject.Find("ChatField");
        chatField.SetActive(false);

        GameObject.Find("UserNameInputField").GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }



    public void ValueChangeCheck()
    {
        if (GameObject.Find("UserNameInputField").GetComponent<InputField>().text == null || GameObject.Find("UserNameInputField").GetComponent<InputField>().text.Length < 1)
        {
            GameObject.Find("RequiredText").GetComponent<Text>().text = "*";
        }
        else
        {
            GameObject.Find("RequiredText").GetComponent<Text>().text = "";
        }
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
            if (gm.GetComponent<ServerBehaviour>() == null && gm.GetComponent<ClientBehaviour>() == null)
            {
                gm.AddComponent<ServerBehaviour>();

                /// Get match name from user before changing view:
                string matchName = GameObject.Find("MatchNameInputField").GetComponent<InputField>().text;
                string userName = GameObject.Find("UserNameInputField").GetComponent<InputField>().text;

                /// Setting premade name of game if not user defined.
                if (matchName == null || matchName == "")
                {
                    matchName = "Summoner's Rift";
                }

                /// Changing view:
                chatField.SetActive(true);
                connectionField.SetActive(false);

                /// Set match name as title on new view, and username as first one in lobby:
                GameObject.Find("MatchName").GetComponent<Text>().text = matchName;
                GameObject.Find("Player1").transform.Find("Text").GetComponent<Text>().text = userName;
            }
        }
        else
        {
            /// Add client behaviour and try to find a host.
            if (gm.GetComponent<ClientBehaviour>() == null && gm.GetComponent<ServerBehaviour>() == null)
            {
                gm.AddComponent<ClientBehaviour>();
                
                StartCoroutine(ConnectOrDisconnect());
            }
        }
    }


    /// <summary>
    /// This function is run through a button that shows ONLY when user is online(connected to server), so it will result in user disconnecting.
    /// </summary>
    public void Disconnect()
    {
        StartCoroutine(ConnectOrDisconnect());
    }

    /// <summary>
    /// WaitForConnection is used for clients that needs to wait a moment for the client behaviour to be set before trying to connect to host.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ConnectOrDisconnect()
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
            GameObject gm = GameObject.Find("GameManager");
            if (gm.GetComponent<ClientBehaviour>() != null)
            {
                string msg = GameObject.Find("ChatInputField").GetComponent<InputField>().text;
                GetComponent<ClientBehaviour>().SendChatMessage(msg);
            }
        }
        else
        {
            /// Let the host chat in the lobby aswell: :)

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
            GameObject gm = GameObject.Find("GameManager");
            if (gm.GetComponent<ClientBehaviour>() != null)
            {
                GameObject.Find("GameManager").GetComponent<ClientBehaviour>().SendScenario();
            }
        }
    }


    /// <summary>
    /// ExitLobby is run when pressing the ExitButton, the client will return to the main menu and stop the connecting/connection or stop the hosting.
    /// </summary>
    public void ExitLobby()
    {
        /// TODO stop connecting...

        /// Instead of exiting the game here in the future we will send the player back to main menu.
        UnityEditor.EditorApplication.isPlaying = false;
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
