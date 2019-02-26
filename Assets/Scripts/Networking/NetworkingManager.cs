using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// A more overviewed perspective that controls the network within the game.
/// Controls whether the user wants to host or join a host,
/// controls the UI of the lobby.
/// Makes sure that the connection/disconnection is done.
/// </summary>
public class NetworkingManager : MonoBehaviour
{
    public string userName;

    public bool isSpectator;
    public GameObject chatField;
    public GameObject connectionField;
    public GameObject gameField;

    public GameObject hostText;


    /// <summary>
    /// List of names is used to see name of players currently in the lobby.
    /// </summary>
    public List<GameObject> attackerNames;
    public List<GameObject> defenderNames;
    public List<GameObject> messageList;


    /// <summary>
    /// There are two different behaviours a player can have, hosting a game or joining a game someone else is hosting:
    /// </summary>
    private ClientBehaviour cb;
    private ServerBehaviour sb;

    /// <summary>
    /// When starting the scene default values will be set here in Start.
    /// </summary>
    void Start()
    {
        QualitySettings.vSyncCount = 0;

        cb = null;
        sb = null;

        hostText = GameObject.Find("HostText");

        /// Get playername positions from the beginning for use in future.
        GameObject attackers = GameObject.Find("Attackers");
        GameObject defenders = GameObject.Find("Defenders");
        GameObject messages = GameObject.Find("MessageField");

        /// Get reference to objects used later on:
        foreach (Transform attacker in attackers.transform)
        {
            if (attacker != attackers.transform)
            {
                attackerNames.Add(attacker.gameObject);
            }
        }
        foreach (Transform defender in defenders.transform)
        {
            if (defender != defenders.transform)
            {
                defenderNames.Add(defender.gameObject);
            }
        }
        foreach (Transform message in messages.transform)
        {
            if (message != messages.transform)
            {
                messageList.Add(message.gameObject);
            }
        }

        connectionField = GameObject.Find("ConnectionField");
        chatField = GameObject.Find("ChatField");
        chatField.SetActive(false);

        gameField = GameObject.Find("GameField");
        gameField.SetActive(false);

        GameObject.Find("UserNameInputField").GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    /// <summary>
    /// This will start the gameplay scene when all clients in a lobby is ready.
    /// </summary>
    public void StartGame()
    {
        /// TODO check for enough players in lobby.
        
        /// Open game view:
        gameField.SetActive(true);
        /// Close lobby view:
        chatField.SetActive(false);
        SceneManager.LoadScene("TestScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// This will exit the gameplay and go back to before-lobby view.
    /// </summary>
    public void ExitGame()
    {
        /// Stop connection.
        DisconnectFromServer();
        /// Close game view:
        gameField.SetActive(false);
        /// Open connection view:
        connectionField.SetActive(true);
        SceneManager.UnloadSceneAsync("TestScene");
    }

    /// <summary>
    /// Makes sure to update the Client/Server-Behaviour every 0.02 seconds approximately.
    /// </summary>
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

    /// <summary>
    /// Cosmetic function that shows the user requirements for starting a game.
    /// </summary>
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
    
    /// <summary>
    /// Function used to set up default values for networking based on player wanting to be host or client(given as bool).
    /// </summary>
    /// <param name="isSpec"></param>
    public void SetupNetworkingManager(bool isSpec)
    {
        /// Get username from user before changing view:
        userName = GameObject.Find("UserNameInputField").GetComponent<InputField>().text;

        /// Can try to join lobby/start game when player actually has a name.
        if (userName != null && userName != "")
        {
            isSpectator = isSpec;
            GameObject gm = GameObject.Find("GameManager");
            if (isSpectator)
            {
                /// Instantiate behaviour:
                if (sb == null && cb == null)
                {
                    sb = new ServerBehaviour();
                }

                /// Changing view:
                chatField.SetActive(true);
                connectionField.SetActive(false);
                GameObject.Find("ConnectionText").GetComponent<Text>().text = "Online";
                GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.green;

                /// Set username of host on top of screen for everyone to see.
                Text T = GameObject.Find("HostText").GetComponent<Text>();
                T.text = "Host: " + userName;
                
                /// Set to false as host no client has joined a lobby before it starts.
                for (int i = 0; i < attackerNames.Count; i++)
                {
                    attackerNames[i].SetActive(false);
                }
                for (int i = 0; i < defenderNames.Count; i++)
                {
                    defenderNames[i].SetActive(false);
                }
            }
            else
            {
                /// Add client behaviour and try to find a host.
                if (cb == null && sb == null)
                {
                    cb = new ClientBehaviour();
                }
                
                /// Set to false as host no client has joined a lobby before it starts.
                for (int i = 0; i < attackerNames.Count; i++)
                {
                    attackerNames[i].SetActive(false);
                }
                for (int i = 0; i < defenderNames.Count; i++)
                {
                    defenderNames[i].SetActive(false);
                }

                /// Make client connect to host or disconnect.
                cb.Connect(this);
            }
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
            
            /// Delete chat when exiting lobby:
            for (int i = 0; i < messageList.Count; i++)
            {
                messageList[i].gameObject.GetComponent<Text>().text = "";
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
            
            /// Delete past chat:
            for (int i = 1; i < messageList.Count; i++)
            {
                messageList[i].GetComponent<Text>().text = "";
            }

            /// Delete chat text as client is disconnecting from lobby.
            cb.Disconnect(messageList);

            /// Disconnect client from host.
            cb.m_clientToServerConnection.Disconnect(cb.m_ClientDriver);
            cb.m_clientToServerConnection = default;
            cb.m_ClientDriver.Dispose();
            cb = null;
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// Will swap the position of two players in lobby, so that they can switch between being a defender/attacker. TODO make this shit
    /// </summary>
    public void SwapPosition()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Find location in lobby where name (given as parameter) exists and remove it.
    /// </summary>
    /// <param name="name"></param>
    public void FindPlayerForRemoval(string name)
    {
        /// Find position of player:
        for (int i = 0; i < attackerNames.Count; i++)
        {
            /// Check for player wanted to be removed being in attackerlist.
            if (attackerNames[i].activeSelf)
            {
                if (attackerNames[i].transform.Find("Text").GetComponent<Text>().text == name)
                {
                    RemovePlayer(attackerNames, i);
                    return;
                }
            }
        }

        for (int i = 0; i < defenderNames.Count; i++)
        {
            /// Check for player wanted to be removed being in defenderlist.
            if (defenderNames[i].activeSelf)
            {
                if (attackerNames[i].transform.Find("Text").GetComponent<Text>().text == name)
                {
                    RemovePlayer(defenderNames, i);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Removes the player at given position and resets the list of players.
    /// </summary>
    /// <param name="nameList"></param>
    public void RemovePlayer(List<GameObject> nameList, int position)
    {
        /// Move one above in list to the one below: (Will overwrite position of where wanted player to be removed is.)
        while (position < nameList.Count - 1)
        {
            /// Check if next in list is used.
            if (nameList[position + 1].activeSelf)
            {
                nameList[position].transform.Find("Text").GetComponent<Text>().text = nameList[position + 1].transform.Find("Text").GetComponent<Text>().text;
            }
            else
            {
                break;
            }
            position++;
        }
        /// Remove last position in list that is no longer used.
        nameList[position].SetActive(false);
    }

    /// <summary>
    /// When a player joins the lobby their name is added to one of the lists.
    /// </summary>
    public void AddPlayerName(string newName)
    {
        /// Look through all available spots and use the first available:
        for (int i = 0; i < attackerNames.Count; i++)
        {
            /// If spot within attacker list is available; Use spot in that list.
            if (!attackerNames[i].activeSelf)
            {
                attackerNames[i].SetActive(true);
                attackerNames[i].transform.Find("Text").GetComponent<Text>().text = newName;
                return;
            }

            /// If spot within defender list is available; Use spot in that list.
            if (!defenderNames[i].activeSelf)
            {
                defenderNames[i].SetActive(true);
                defenderNames[i].transform.Find("Text").GetComponent<Text>().text = newName;
                return;
            }
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
        /// Put message in textbox:
        MoveChatBox();
        messageList[messageList.Count - 1].GetComponent<Text>().text = msg;
    }

    /// <summary>
    /// Moves every text in chatField one step up for the new message to be put at bottom.
    /// </summary>
    void MoveChatBox()
    {
        /// Move messages in chat one line in list:
        for (int i = 0; i < messageList.Count - 1; i++)
        {
            if (!messageList[i].activeSelf)
            {
                messageList[i].SetActive(true);
            }
            messageList[i].GetComponent<Text>().text = messageList[i + 1].GetComponent<Text>().text;
        }
    }
}
