using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviour
{
    public string userName;
    public string matchName;

    public bool isSpectator;
    public GameObject chatField;
    public GameObject connectionField;

    
    // TODO make these into one array - easier to scale further then.
    public GameObject playerName1;
    public GameObject playerName2;
    public GameObject playerName3;

    private ClientBehaviour cb;
    private ServerBehaviour sb;

    void Start()
    {
        QualitySettings.vSyncCount = 0;

        cb = null;
        sb = null;

        /// Get playername positions from the beginning for use in future.
        playerName1 = GameObject.Find("Player1");
        playerName2 = GameObject.Find("Player2");
        playerName3 = GameObject.Find("Player3");

        connectionField = GameObject.Find("ConnectionField");
        chatField = GameObject.Find("ChatField");
        chatField.SetActive(false);

        GameObject.Find("UserNameInputField").GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }


    void FixedUpdate()
    {
        if (cb != null)
        {
            cb.FixedUpdate();
        }
        else if (sb != null)
        {
            sb.FixedUpdate();
        }
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
            if (sb == null && cb == null)
            {
                sb = new ServerBehaviour();
            }

            /// Get match name from user before changing view:
            matchName = GameObject.Find("MatchNameInputField").GetComponent<InputField>().text;
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
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Online";
            GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.green;

            /// Set match name as title on new view, and username as first one in lobby:
            GameObject.Find("MatchName").GetComponent<Text>().text = matchName;
            playerName1.transform.Find("Text").GetComponent<Text>().text = userName;

            /// Set to false as host is the only one in lobby when it starts.
            playerName2.SetActive(false);
            playerName3.SetActive(false);

        }
        else
        {
            /// Add client behaviour and try to find a host.
            if (cb == null && sb == null)
            {
                cb = new ClientBehaviour(this);
            }

            /// Get username from user before changing view to lobby view if host is found.
            userName = GameObject.Find("UserNameInputField").GetComponent<InputField>().text;
            playerName3.SetActive(false);

            
            /// Make client connect to host or disconnect.
            cb.Connect(this);
        }
    }

    

    /// <summary>
    /// This function is run through a button that shows ONLY when user is online(connected to server), so it will result in user disconnecting.
    /// TODO stop connecting function before starting new coroutine.
    /// </summary>
    public void DisconnectFromServer()
    {
        StartCoroutine(Disconnect());
    }

    /// <summary>
    /// WaitForConnection is used for clients that needs to wait a moment for the client behaviour to be set before trying to connect to host.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Disconnect()
    {
        yield return new WaitForSeconds(0.01f);

        if (isSpectator)
        {
            /// Disconnect clients from server:
            
            if (sb.m_ServerDriver.IsCreated)
            {
                sb.m_ServerDriver.ScheduleUpdate().Complete();
                
                if (sb.m_connections.IsCreated)
                {
                    for (int i = 0; i < sb.m_connections.Length; ++i)
                    {
                        /// Send message for client to disconnect:
                        string message = "<Disconnect>";
                        var messageWriter = new DataStreamWriter(message.Length, Allocator.Temp);
                        
                        messageWriter.Write(Encoding.ASCII.GetBytes(message));

                        sb.m_ServerDriver.Send(sb.m_connections[i], messageWriter);
                        messageWriter.Dispose();


                        /// Wait for client to disconnect:
                        Thread.Sleep(100);
                        /// Disconnect server:
                        sb.m_ServerDriver.Disconnect(sb.m_connections[i]);
                    }
                }
            }
            sb.StopListening();
            sb.m_connections.Clear();
            sb.m_connections.Dispose();
            sb.m_ServerDriver.Dispose();
            sb = null;

            /// Delete chat from lobby exiting:
            Transform trans = chatField.transform.Find("MessageField").transform;
            foreach (Transform child in trans)
            {
                if (child != trans)
                {
                    child.gameObject.GetComponent<Text>().text = "";
                }
            }


            /// Change connection text:
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Offline";
            GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.red;
            
            /// Change view to not be in a lobby:
            chatField.SetActive(false);
            connectionField.SetActive(true);
        }
        else
        {
            /// Get reference to chatfield for client connection to host:
            List<GameObject> messageTexts = new List<GameObject>();
            Transform trans = chatField.transform.Find("MessageField").transform;
            foreach (Transform child in trans)
            {
                if (child != trans)
                {
                    messageTexts.Add(child.gameObject);
                }
            }

            /// Delete chat text as client is disconnecting from lobby.
            cb.Disconnect(messageTexts);

            /// Disconnect client from host.
            cb.m_clientToServerConnection.Disconnect(cb.m_ClientDriver);
            cb.m_clientToServerConnection = default;
            cb.m_ClientDriver.Dispose();
            cb = null;
            yield return new WaitForSeconds(1);
        }
    }


    /// <summary>
    /// When a player leaves the lobby their name is removed from the list.
    /// </summary>
    public void RemovePlayerAtPosition(int listNr)
    {
        /// listNr gives the number in list after host(Host is always nr.1).

        if (listNr == 0)
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
        else if (listNr == 1)
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
    /// SendMessage will use the Message given through parameter(temporarily set as string, will be of type 'Message' later)
    /// to send a message that the gamestate has changed through the network to update the versions of other players in the game.
    /// </summary>
    public void SendChatMessage()
    {
        GameObject gm = GameObject.Find("GameManager");
        string msg = userName + " - " + GameObject.Find("ChatInputField").GetComponent<InputField>().text;
        if (!isSpectator)
        {
            if (cb != null)
            {
                cb.SendChatMessage(msg);
            }
        }
        else
        {
            if (sb != null)
            {
                sb.SendChatMessage(msg);
            }
        }
    }
    
    
    /// <summary>
    /// GetMessage will receive message collected through the server/client script.
    /// </summary>
    /// <param name="msg"></param>
    public void GetMessage(string msg)
    {
        //GameObject.Find("MessageText").GetComponent<Text>().text = msg;

        // Put message in textbox for testing:
        MoveChatBox();
        GameObject.Find("MessageText1").GetComponent<Text>().text = msg;
    }

    /// <summary>
    /// Moves every text in chatField one step up for the new message to be put at bottom.
    /// </summary>
    void MoveChatBox()
    {
        GameObject.Find("MessageText5").GetComponent<Text>().text = GameObject.Find("MessageText4").GetComponent<Text>().text;
        GameObject.Find("MessageText4").GetComponent<Text>().text = GameObject.Find("MessageText3").GetComponent<Text>().text;
        GameObject.Find("MessageText3").GetComponent<Text>().text = GameObject.Find("MessageText2").GetComponent<Text>().text;
        GameObject.Find("MessageText2").GetComponent<Text>().text = GameObject.Find("MessageText1").GetComponent<Text>().text;
    }
}
