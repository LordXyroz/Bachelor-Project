using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviour
{
    private string userName;

    public bool isSpectator;
    public GameObject chatField;
    public GameObject connectionField;

    private GameObject playerName1;
    private GameObject playerName2;
    private GameObject playerName3;

    void Start()
    {
        playerName1 = GameObject.Find("Player1");
        playerName2 = GameObject.Find("Player2");
        playerName3 = GameObject.Find("Player3");

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
        GameObject gm = GameObject.Find("GameManager");
        if (isSpectator)
        {
            if (gm.GetComponent<ServerBehaviour>() == null && gm.GetComponent<ClientBehaviour>() == null)
            {
                gm.AddComponent<ServerBehaviour>();

                /// Get match name from user before changing view:
                string matchName = GameObject.Find("MatchNameInputField").GetComponent<InputField>().text;
                userName = GameObject.Find("UserNameInputField").GetComponent<InputField>().text;

                /// Setting premade names if none is already there.
                if (matchName == null || matchName == "")
                {
                    matchName = "Summoner's Rift";
                }
                if (userName == null || userName == "")
                {
                    userName = "Player 1";
                }

                /// Changing view:
                chatField.SetActive(true);
                connectionField.SetActive(false);

                /// Set match name as title on new view, and username as first one in lobby:
                GameObject.Find("MatchName").GetComponent<Text>().text = matchName;
                playerName1.transform.Find("Text").GetComponent<Text>().text = userName;

                /// Set to false as host is the only one in lobby when it starts.
                playerName2.SetActive(false);
                playerName3.SetActive(false);
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
    /// When a player leaves the lobby their name is removed from the list.
    /// </summary>s
    public void RemovePlayerName(int listNr)
    {
        /// listNr gives the number in list after host(Host is always nr.1).

        if (listNr == 1)
        {
            if (playerName2.activeSelf && playerName3.activeSelf)
            {
                playerName2.transform.Find("Text").GetComponent<Text>().text = playerName3.transform.Find("Text").GetComponent<Text>().text;
            }
            else
            {
                playerName2.SetActive(false);
            }
        }
        else if (listNr == 2)
        {
            playerName3.SetActive(false);
        }
    }

    /// <summary>
    /// When a player joins the lobby their name is added to the list.
    /// </summary>
    public void AddPlayerName(string newName)
    {
        if (playerName2.activeSelf)
        {
            if (playerName2.transform.Find("Text").GetComponent<Text>().text == "" || playerName2.transform.Find("Text").GetComponent<Text>().text == null)
            {
                playerName2.transform.Find("Text").GetComponent<Text>().text = newName;
            }
            else
            {
                playerName3.SetActive(true);
                playerName3.transform.Find("Text").GetComponent<Text>().text = newName;
            }
        }
        else
        {
            playerName2.SetActive(true);
            playerName2.transform.Find("Text").GetComponent<Text>().text = newName;
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
        GameObject gm = GameObject.Find("GameManager");
        string msg = userName + " - " + GameObject.Find("ChatInputField").GetComponent<InputField>().text;
        if (!isSpectator)
        {
            if (gm.GetComponent<ClientBehaviour>() != null)
            {
                GetComponent<ClientBehaviour>().SendChatMessage(msg);
            }
        }
        else
        {
            if (gm.GetComponent<ServerBehaviour>() != null)
            {
                GetComponent<ServerBehaviour>().SendChatMessage(msg);
            }
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
        //GameObject.Find("MessageText").GetComponent<Text>().text = msg;
    }

    public void GetHostMessage(string msg)
    {
        Debug.Log("Host message: " + msg);
    }
}
