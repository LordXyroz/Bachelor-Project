using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


/// <summary>
/// A more overviewed perspective that controls the network within the game.
/// Controls whether the user wants to host or join a host,
/// controls the UI of the lobby.
/// Makes sure that the connection/disconnection is done.
/// </summary>
[System.Serializable]
public class NetworkingManager : MonoBehaviour
{
    public string userName;
    
    /// <summary>
    /// Many different UI elements that are worked on througout the gameflow
    /// </summary>
    [Header("UI")]
    public GameObject chatField;
    public GameObject connectionField;

    public GameObject lobbyScrollField;
    public GameObject lobbyScrollFrame;
    public GameObject lobbyButton;

    public GameObject matchmakingObject;

    public GameObject hostText;
    public GameObject SwapLoadingScreen;

    public GameObject joinButton;
    public GameObject stopJoinButton;
    public GameObject startGameButton;

    public GameObject matchmakingCanvas;



    /// <summary>
    /// List of names is used to see name of players currently in the lobby.
    /// </summary>
    [Header("Lists")]
    public List<GameObject> attackerNames;
    public List<GameObject> defenderNames;
    public List<GameObject> messageList;


    /// <summary>
    /// There are two different behaviours a player can have, hosting a game or joining a game someone else is hosting:
    /// </summary>
    [Header("Behaviours")]
    public ClientBehaviour cb;
    public ServerBehaviour sb;

    [Header("Variables")]
    public PlayerManager.PlayerType playerType;
    public bool inGame = false;

    [Header("Savefile variables")]
    [SerializeField]
    public Save saveFile;
    public GameObject loadMenu;

    /// <summary>
    /// When starting the scene default values will be set here in Start.
    /// </summary>
    void Start()
    {
        cb = null;
        sb = null;
        
        playerType = default;
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

        chatField = GameObject.Find("Lobby");
        chatField.SetActive(false);

        lobbyScrollField = GameObject.Find("LobbyScrollField");
        lobbyScrollFrame = GameObject.Find("LobbyScrollFieldFrame");

        matchmakingObject = GameObject.Find("MatchmakingObject");

        lobbyButton = (GameObject)Resources.Load("Prefabs/LobbyButton", typeof(GameObject));

        connectionField = GameObject.Find("ConnectionField");
        joinButton = connectionField.transform.Find("JoinButton").gameObject;
        stopJoinButton = connectionField.transform.Find("StopJoinButton").gameObject;
        stopJoinButton.SetActive(false);
        startGameButton = chatField.transform.Find("StartGame").gameObject;
        
        
        GameObject.Find("UserNameInputField").GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    /// <summary>
    /// This will start the gameplay scene when all clients in a lobby is ready.
    /// </summary>
    public void StartGame()
    {
        inGame = true;
        if (playerType == PlayerManager.PlayerType.Observer)
            sb.StartGame();
    }

    /// <summary>
    /// This will exit the gameplay and go back to before-lobby view.
    /// </summary>
    public void ExitGame()
    {
        if (cb != null)
        {
            StopConnecting();
            cb.Dispose();
        }
        if (sb != null)
            sb.Dispose();

        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// Used for client to be able to stop looking for a host.
    /// </summary>
    public void StopConnecting()
    {
        /// Not a client:
        if (playerType == PlayerManager.PlayerType.Observer)
        {
            return;
        }

        if (cb != null)
        {
            cb.StopConnecting(this);

            /// Set the view correct:
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Offline";
            GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.red;

            stopJoinButton.SetActive(false);
            joinButton.SetActive(true);

            playerType = default;
        }
    }

    /// <summary>
    /// Makes sure to update the Client/Server-Behaviour every 0.05 seconds approximately.
    /// </summary>
    void FixedUpdate()
    {
        if (cb != null)
        {
            cb.FixedUpdate(this);
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
    public void SetupNetworkingManager(bool isHost)
    {
        /// Get username from user before changing view:
        userName = GameObject.Find("UserNameInputField").GetComponent<InputField>().text;

        /// Can try to join lobby/start game when player actually has a name.
        if (userName != null && userName != "")
        {
            if (isHost)
            {
                if (cb != null)
                {
                    StopConnecting();
                    cb.Dispose();
                }

                if (sb != null)
                    sb.Dispose();

                /// Make HostText available:
                chatField.SetActive(true);
                /// Set username of host on top of screen for everyone to see.
                Text T = GameObject.Find("HostText").GetComponent<Text>();
                T.text = userName + "'s lobby";
                chatField.SetActive(false);

                loadMenu.SetActive(true);
            }
            else
            {
                /// Make list of available hosts ready for use:
                RemoveLobbies();

                if (cb != null)
                    cb.Dispose();
                if (sb != null)
                    sb.Dispose();
                

                /// Add client behaviour and try to find a host.
                cb = new ClientBehaviour();

                lobbyScrollField.SetActive(true);
                lobbyScrollFrame.SetActive(true);

                matchmakingObject.SetActive(true);

                /// Make players able to stop trying to connect.
                stopJoinButton.SetActive(true);
                joinButton.SetActive(false);
                startGameButton.SetActive(false);
                

                /// This is set as a temp value not default for player to be able to stop looking for host.
                playerType = PlayerManager.PlayerType.Attacker;
                
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
                cb.FindHosts(this);
            }
        }
    }
    
    /// <summary>
    /// This function is run through a button that shows ONLY when user is online(connected to server), so it will result in user disconnecting.
    /// </summary>
    public void DisconnectFromServer()
    {
        matchmakingCanvas.SetActive(true);
        lobbyScrollField.SetActive(true);
        lobbyScrollFrame.SetActive(true);

        matchmakingObject.SetActive(true);

        if (playerType == PlayerManager.PlayerType.Observer)
        {
            if (sb == null)
            {
                return;
            }
            /// Disconnect clients from server:
            if (sb.m_ServerDriver.IsCreated)
            {
                sb.m_ServerDriver.ScheduleUpdate().Complete();

                if (sb.m_connections.IsCreated)
                {
                    sb.BroadcastMessage(new Message("Client", userName, MessageTypes.Network.Disconnect));

                    Thread.Sleep(100);
                    for (int i = 0; i < sb.m_connections.Length; i++)
                        sb.m_ServerDriver.Disconnect(sb.m_connections[i]);

                }
            }

            sb.StopListening();
            sb.Dispose();

            /// Delete chat when exiting lobby:
            for (int i = 0; i < messageList.Count; i++)
            {
                messageList[i].gameObject.GetComponent<Text>().text = "";
            }


            connectionField.SetActive(true);
            /// Change connection text:
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Offline";
            GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.red;

            /// Change view to not be in a lobby:
            chatField.SetActive(false);

            /// Make it possible for them to start game if they choose to be host:
            startGameButton.SetActive(true);
        }
        else if (playerType == PlayerManager.PlayerType.Attacker || playerType == PlayerManager.PlayerType.Defender)
        {
            if (cb == null)
            {
                return;
            }

            /// Delete past chat:
            for (int i = 1; i < messageList.Count; i++)
            {
                messageList[i].GetComponent<Text>().text = "";
            }

            /// Delete chat text as client is disconnecting from lobby.
            cb.Disconnect();

            foreach (var a in attackerNames)
                a.transform.Find("Text").GetComponent<Text>().text = "";

            foreach (var b in defenderNames)
                b.transform.Find("Text").GetComponent<Text>().text = "";

            cb.Dispose();
        }

        playerType = default;
    }

    /// <summary>
    /// Will swap the position of two players in lobby, so that they can switch between being a defender/attacker.
    /// </summary>
    public void SwapPosition()
    {
        /// Hosts are not playing the game themselves so they may not swap positions.
        if (playerType == PlayerManager.PlayerType.Observer)
        {
            return;
        }
        
        /// Check if player is yourself:
        if (EventSystem.current.currentSelectedGameObject.transform.Find("Text").GetComponent<Text>().text == userName)
        {
            Debug.Log("This is yourself!");
            return;
        }

        /// Check if player is on your team:
        if (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name == "Attackers")
        {
            if (playerType == PlayerManager.PlayerType.Attacker)
            {
                /// You are attacker, same as client clicked on.
                Debug.Log("This is your teammate!");
                return;
            }
        }
        else /// Defender:
        {
            if (playerType == PlayerManager.PlayerType.Defender)
            {
                /// You are defender, same as client clicked on.
                Debug.Log("This is your teammate!");
                return;
            }
        }

        /// Get name of player you want to swap with and your own name for that client to know who wants to swap.
        string clientName = EventSystem.current.currentSelectedGameObject.transform.Find("Text").GetComponent<Text>().text;

        /// Send message to other client about wanting to swap.
        cb.SendSwapMessage(clientName);

        /// Start the loading bar countdown.
        StartCoroutine(StartSwapLoadBar(clientName, true));
    }

    /// <summary>
    /// Starts loading bar for client wanting to swap playertype with another client; It will only last for x seconds.
    /// </summary>
    public IEnumerator StartSwapLoadBar(string name, bool isSettingUp)
    {
        
        /// Create SwapLoadScreen:
        GameObject sls = Instantiate(SwapLoadingScreen, GameObject.Find("Canvas").transform);
        sls.name = "SwapLoadingScreen";

        /// Set name: 
        sls.transform.Find("Name").GetComponent<Text>().text = name;

        /// Set listener for buttons:
        sls.transform.Find("AcceptButton").GetComponent<Button>().onClick.AddListener(() => AcceptSwap());
        sls.transform.Find("DeclineButton").GetComponent<Button>().onClick.AddListener(() => DeclineSwap());


        /// Do not show accept button if client is the one setting up loadbar.
        if (isSettingUp)
        {
            sls.transform.Find("AcceptButton").gameObject.SetActive(false);
            /// Set declinebutton to center:
            Vector3 tempPosition = sls.transform.Find("DeclineButton").transform.position;
            tempPosition.x -= 1; /// Default is x = 130;
            sls.transform.Find("DeclineButton").transform.position = tempPosition;
        }

        /// Set position of loadbar:
        if (FindPlayerType() == PlayerManager.PlayerType.Attacker)
        {
            sls.transform.position = GameObject.Find("Attackers").transform.position;
        }
        else
        {
            sls.transform.position = GameObject.Find("Defenders").transform.position;
        }

        
        /// Update SwapLoadScreen:
        while (sls != null)
        {
            sls.transform.Find("Slider").GetComponent<Slider>().value += 0.01f;    /// Just a number to get the bar loading at a decent speed.
            if (sls.transform.Find("Slider").GetComponent<Slider>().value == 1)
            {
                /// Finished task, delete loading screen.
                Destroy(sls);
                break;
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void Swap(GameObject currentClient, GameObject otherClient)
    {
        string temp = otherClient.transform.Find("Text").GetComponent<Text>().text;
        otherClient.transform.Find("Text").GetComponent<Text>().text = currentClient.transform.Find("Text").GetComponent<Text>().text;
        currentClient.transform.Find("Text").GetComponent<Text>().text = temp;
    }

    /// <summary>
    /// Swap the names of current client(userName) and client with name sent as parameter.
    /// </summary>
    /// <param name="otherClientName"></param>
    public void FindSwapNames(string fromClientName, string toClientName)
    {
        /// Swap names:
        GameObject currentClient = attackerNames.Find(x => x.transform.Find("Text").GetComponent<Text>().text == toClientName);
        if (currentClient)
        {
            /// Username lies in attacker list.
            GameObject otherClient = attackerNames.Find(x => x.transform.Find("Text").GetComponent<Text>().text == fromClientName);
            if (otherClient)
            {
                /// OtherClient lies in attackerList.
                Swap(currentClient, otherClient);
            }
            else
            {
                /// OtherClient lies in defenderList.
                otherClient = defenderNames.Find(x => x.transform.Find("Text").GetComponent<Text>().text == fromClientName);
                Swap(currentClient, otherClient);
            }
        }
        else if ((currentClient = defenderNames.Find(x => x.transform.Find("Text").GetComponent<Text>().text == toClientName)))
        {
            /// Username lies in defenderNames list.
            currentClient = defenderNames.Find(x => x.transform.Find("Text").GetComponent<Text>().text == toClientName);
            GameObject otherClient = attackerNames.Find(x => x.transform.Find("Text").GetComponent<Text>().text == fromClientName);
            if (otherClient)
            {
                /// OtherClient lies in attackerList.
                Swap(currentClient, otherClient);
            }
            else
            {
                /// OtherClient lies in defenderList.
                otherClient = defenderNames.Find(x => x.transform.Find("Text").GetComponent<Text>().text == fromClientName);
                Swap(currentClient, otherClient);
            }
        }

        /// Switch type:
        if (playerType == PlayerManager.PlayerType.Attacker)
        {
            playerType = PlayerManager.PlayerType.Defender;
        }
        else if (playerType == PlayerManager.PlayerType.Defender)
        {
            playerType = PlayerManager.PlayerType.Attacker;
        }
    }
    
    /// <summary>
    /// Client Accepts swap. and deletes loadingscreen:
    /// </summary>
    public void AcceptSwap()
    {
        /// Get name of other client to send message to:
        GameObject screen = GameObject.Find("Canvas").transform.Find("SwapLoadingScreen").gameObject;
        GameObject screen2 = screen.transform.Find("Name").gameObject;
        string name = screen2.GetComponent<Text>().text;
        
        cb.AcceptSwap(name);

        DestroySwap();
    }

    /// <summary>
    /// Client declines/stops swap.
    /// </summary>
    public void DeclineSwap()
    {
        /// Get name of other client to send message to:
        string name = GameObject.Find("Canvas").transform.Find("SwapLoadingScreen").transform.Find("Name").GetComponent<Text>().text;
        cb.DeclineSwap(name);

        DestroySwap();
    }

    /// <summary>
    /// Destroy swaploadingscreen from the UI when not needed anymore.
    /// </summary>
    public void DestroySwap()
    {
        try
        {
            Destroy(GameObject.Find("Canvas").transform.Find("SwapLoadingScreen").gameObject);
        }
        catch
        {
            /// Could most likely not find the loading bar screen.
        }
    }

    /// <summary>
    /// Finds the type for the player based on positioning in the UI.
    /// </summary>
    /// <returns></returns>
    public PlayerManager.PlayerType FindPlayerType()
    {
        /// Find type:
        for (int i = 0; i < attackerNames.Count; i++)
        {
            /// Check for player being in attackerlist:
            if (attackerNames[i].activeSelf)
            {
                if (attackerNames[i].transform.Find("Text").GetComponent<Text>().text == userName)
                {
                    return PlayerManager.PlayerType.Attacker;
                }
            }
        }
        /// Return type defender to client if not found in attackerlist:
        return PlayerManager.PlayerType.Defender;
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
                if (defenderNames[i].transform.Find("Text").GetComponent<Text>().text == name)
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
        nameList[position].transform.Find("Text").GetComponent<Text>().text = "";
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
    /// Function run when message is received through network, broadcast it to where it is wanted.
    /// </summary>
    /// <param name="msg"></param>
    public void GetMessage(Message msg)
    {
        MessagingManager.BroadcastMessage(msg);
    }

    /// <summary>
    /// Sends message to everyone else through network.
    /// </summary>
    public void SendMessage(Message msg)
    {
        cb.SendMessage(msg);
    }

    /// <summary>
    /// SendMessage will use the Message given through parameter(temporarily set as string, will be of type 'Message' later)
    /// to send a message that the gamestate has changed through the network to update the versions of other players in the game.
    /// </summary>
    public void SendChatMessage()
    {
        GameObject gm = GameObject.Find("GameManager");
        string msg = playerType.ToString() + ":" + userName + " - " + GameObject.Find("ChatInputField").GetComponent<InputField>().text;
        if (playerType != PlayerManager.PlayerType.Observer)
        {
            if (cb != null)
            {
                cb.SendMessage(new ChatMessage("Server", userName, MessageTypes.Network.Chat, msg));
            }
        }
        else
        {
            if (sb != null)
            {
                GetChatMessage(msg);
                sb.BroadcastMessage(new ChatMessage("Client", userName, MessageTypes.Network.Chat, msg));
            }
        }
    }
    
    /// <summary>
    /// GetMessage will receive message collected through the server/client script.
    /// </summary>
    /// <param name="msg"></param>
    public void GetChatMessage(string msg)
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

    public void RemoveLobbies()
    {
        foreach (Transform child in lobbyScrollField.transform.Find("ButtonListViewport").Find("ButtonListContent").transform)
        {
            Destroy(child.gameObject);
        }

        lobbyScrollField.SetActive(false);
        lobbyScrollFrame.SetActive(false);
        matchmakingObject.SetActive(false);
    }

    public void LoadSaveFile(string path)
    {
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);

            saveFile = JsonUtility.FromJson<Save>(json);

            loadMenu.SetActive(false);

            sb = new ServerBehaviour();
            playerType = PlayerManager.PlayerType.Observer;

            lobbyScrollField.SetActive(false);
            lobbyScrollFrame.SetActive(false);
            matchmakingObject.SetActive(false);

            /// Changing view:
            chatField.SetActive(true);
            connectionField.SetActive(false);
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Online";
            GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.green;

            //Text T = GameObject.Find("HostText").GetComponent<Text>();
            //T.text = userName + "'s lobby";

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
    }
}