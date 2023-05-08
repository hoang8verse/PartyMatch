using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Net.WebSockets;
using Newtonsoft.Json.Linq;

// Use plugin namespace
using NativeWebSocket;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.SceneManagement;
using CMF;
//using System.Globalization;
using Utility;

public enum EServerCmd: byte
{   
    RoomDetected = 0,
    RequestRoom,
    Join,
    JoinLobby,
    JoinLobbyRoom,
    JoinRoom,
    FailJoinRoom,    
    RoundAlready,        
    GotoGame,
    StartGame,
    PositionPlayer,
    UpdatePos,
    CheckPosition,
    HitEnemy,    
    Stunned,
    Moving,
    RequestTarget,
    ResponseTarget,
    CubeReset,
    CountDown,  
    CubeFall,    
    RoundPass,    
    PlayerDie,    
    PlayerWin,        
    PlayerLeaveRoom,
    Leave,
    EndGame    
}

public enum EPlayerProfile
{
    ClientId = 0,
    Index,
    AppId,
    Avatar,
    Gender,
    NickName,
    RoomId,
    IsHost,
    IsSpectator,
    CharacterIndex,
    StartIndex,
    Position,   
    IsStarted,    
    Status,    
    Round,
    AlivePlayers,
    AliveLobbyPlayers,
    CountPlayers,
    PhoneNumber,
    FollowedOA
}

public class SocketClient : Singleton<SocketClient>
{
    public enum EGameState
    {
        None = 0,
        InLobby,
        InGame
    }    
    
    public delegate void ReceiveAction(string message);
    public event ReceiveAction OnReceived;

    //public ClientWebSocket webSocket = null;
    public WebSocket webSocket;   
    private string url = "";
    static string baseUrl = "ws://192.168.1.12";
    static string HOST = "8082";

    //static string baseUrl = "wss://rlgl2-api.brandgames.vn";
    //static string HOST = "8081";

    public string ROOM = "";
    public string m_localClientId = "";

    public string playerJoinName = "";
    public int currentPlayerJoined = 0;

    private bool m_isHost = false;
    private bool m_isSpectator = false;
    public static bool IS_FIRST_JOIN = true;

    [SerializeField]
    private GameObject playerPrefab;
    public GameObject m_player = null;
    [SerializeField]
    private GameObject otherPlayerPrefab;
    private List<JToken> m_players = new List<JToken>();    
    private Dictionary<string,GameObject> m_otherPlayers = new Dictionary<string, GameObject>();
    private List<int> m_aliveIndexPlayers = new List<int>();
    private List<int> m_aliveLobbyPlayer = new List<int>();
    private bool m_isSinglePlayer = true;
    [SerializeField]
    private GameObject spectatorPrefab;  

    private Vector3 m_clientPosStart;    
    private string stunnedByEnemyId = "";
    private bool isEndGame = false;    
    private bool m_isRunOnMobile = false;
    private bool m_isMoving = false;
    private int m_localPlayerIndex = 0;
    private EGameState m_gameState = EGameState.None;

    public bool IsHost => m_isHost;

    public static string EServerCmd2String(EServerCmd serverCmd)
    {
        int iCmd = (int)serverCmd;
        string stringValue = iCmd.ToString();

        return stringValue;
    }
    public static EServerCmd String2EServerCmd(string str)
    {
        foreach (EServerCmd serverCmd in Enum.GetValues(typeof(EServerCmd)))
        {            
            string stringValue = EServerCmd2String(serverCmd);

            if (stringValue.ToLower() == str.ToLower())
            {
                return serverCmd;
            }
        }
        throw new ArgumentException("Invalid EServerCmd string.");
    }

    public static string EPlayerProfile2String(EPlayerProfile serverCmd)
    {
        int iCmd = (int)serverCmd;
        string stringValue = iCmd.ToString();

        return stringValue;
    }    
    public static EPlayerProfile String2EPlayerProfile(string str)
    {
        foreach (EPlayerProfile serverCmd in Enum.GetValues(typeof(EPlayerProfile)))
        {            
            string stringValue = EPlayerProfile2String(serverCmd);

            if (stringValue.ToLower() == str.ToLower())
            {
                return serverCmd;
            }
        }
        throw new ArgumentException("Invalid EServerCmd string.");
    }

    void Awake()
    {
        CheckDevice();
        DontDestroyOnLoad(gameObject);
        //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
    }
    void Start()
    {
    }

    int GetPlayerIndex(int createdIndex)
    {
        int index = 0;
        int counter = 0;

        foreach(var player in m_players)
        {
            if(player[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>() == createdIndex)
            {
                index = counter;
                break;
            }               
            counter++;
        }

        Debug.Log($"[SocketClient] GetPlayerIndex createdIndex = {createdIndex} => index = {index}");
        return index;
    }    
   
    void OnUpdateCountPlayers(int leaveIndex)
    {        
        if (m_aliveIndexPlayers.Contains(leaveIndex))
        {
            m_aliveIndexPlayers.Remove(leaveIndex);
            Debug.Log($"[SocketClient] leave room player m_aliveIndexPlayers = {m_aliveIndexPlayers}");
        }

        if (m_aliveLobbyPlayer.Contains(leaveIndex))
        {
            m_aliveLobbyPlayer.Remove(leaveIndex);
            Debug.Log($"[SocketClient] leave room player m_aliveLobbyPlayer = {m_aliveLobbyPlayer}");
        }       
    }    
    void OnCheckWinnerPlayer(string playerID, int serverCountPlayer, int countRoundPassed)
    {
        if (m_localClientId == playerID)
        {
            if (!IsSpectator(playerID))
            {
                if ((m_isSinglePlayer && countRoundPassed >= 5) || (!m_isSinglePlayer && serverCountPlayer - CountAliveSpectator(isLobby: false) <= 1))
                {
                    CubeManager.Instance.SetPlayerWin();
                }
            }
        }
    }

    void Update()
    {
        if (!isEndGame)
        {
            if (m_isRunOnMobile)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0); // trying to get the second touch input

                    if (touch.phase == TouchPhase.Began)
                    {

                        
                        if (m_player != null)
                        {
                            m_isMoving = true;
                            StartCoroutine(StartPlayerMoving());
                        }

                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {

                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {

                        m_isMoving = false;
                        OnUpdatePosPlayer();

                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0)) // 0 : left , 1 : right, 2 : wheel
                {

                    
                    if (m_player != null)
                    {
                        m_isMoving = true;
                        StartCoroutine(StartPlayerMoving());
                    }

                }
                else
                if (Input.GetMouseButton(0)) // 0 : left , 1 : right, 2 : wheel
                {

                }
                else
                if (Input.GetMouseButtonUp(0))
                {

                    m_isMoving = false;
                    OnUpdatePosPlayer();

                }
            }
        }


#if !UNITY_WEBGL || UNITY_EDITOR
        if (webSocket!=null)
            webSocket.DispatchMessageQueue();
#endif
    }

    async void OnDestroy()
    {
        if (webSocket != null)
        {
            //webSocket.Dispose();
           await webSocket.Close();
        }

        Debug.Log("WebSocket closed.");
    }

    private const string CHARS = "0123456789";
    public string Generate()
    {
        string result = "";
        System.Random rand = new System.Random();
        while (result.Length < 6)
        {
            result += CHARS[rand.Next(0, CHARS.Length)];
        }
        return result;
    }
    private Vector3 PositionByIndex(int ranIndex)
    {
        return CubeManager.Instance.CubeMeshs[ranIndex].transform.position;
    }
    private int GetRandomLength()
    {
        return CubeManager.Instance.CubeMeshs.Length;
    }
    private Vector3 RandomPosition()
    {
        System.Random rand = new System.Random();
        int ranIndex = rand.Next(0, GetRandomLength());
        Vector3 randomPoint = PositionByIndex(ranIndex);
        return randomPoint;
    }

    void CheckDevice()
    {
        if (Application.isMobilePlatform)
        {
            Debug.Log("Running on a mobile device.");
            m_isRunOnMobile = true;
        }
        else
        {
            Debug.Log("Running on a non-mobile device.");
            m_isRunOnMobile = false;
        }
    }

    IEnumerator StartPlayerMoving()
    {
        if (!m_isMoving || !m_player) yield return null;
        float _h = 0;
        float _v = 0;
        Vector3 _velocity = Vector3.zero;
        if (m_player)
        {
            _velocity = m_player.GetComponent<Mover>().GetVelocity();
            _h = m_player.GetComponent<CharacterInput>().GetHorizontalMovementInput();
            _v = m_player.GetComponent<CharacterInput>().GetVerticalMovementInput();

            OnMoving(_velocity, _h, _v);
        }

        yield return new WaitForSeconds(Time.deltaTime);

        if (m_isMoving)
        {
            StartCoroutine(StartPlayerMoving());
        }
        else
        {
            OnMoving(Vector3.zero, 0, 0);
        }

    }

    IEnumerator CheckPlayerMoving()
    {

        if (isEndGame || !m_player) yield return null ;
        float _h = 0;
        float _v = 0;
        Vector3 _velocity = Vector3.zero;
        if (m_player)
        {
            _velocity = m_player.GetComponent<Mover>().GetVelocity();
            _h = m_player.GetComponent<CharacterInput>().GetHorizontalMovementInput();
            _v = m_player.GetComponent<CharacterInput>().GetVerticalMovementInput();
        }

        //if(_h != 0 || _v != 0)
        {
            Debug.Log("CheckPlayerMoving -------------------------------_velocity = " + _velocity);
            if (!isEndGame)
            {
                OnMoving(_velocity, _h, _v);
            } 
          
        } 
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        if (!isEndGame)
        {
            LoopCheckPlayerMoving();
        }
    }
    void LoopCheckPlayerMoving()
    {
        StartCoroutine("CheckPlayerMoving");
    }
    IEnumerator UpdatePositionOtherPlayers()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        foreach (var item in m_players)
        {
            if (m_otherPlayers.ContainsKey(item[EPlayerProfile2String(EPlayerProfile.ClientId)].ToString()))
            {
                Vector3 pos = Vector3.zero;
                JArray arrPos = JArray.Parse(item[EPlayerProfile2String(EPlayerProfile.Position)].ToString());
                if (arrPos.Count > 0)
                {
                    pos = new Vector3(arrPos[0].Value<float>(),
                            arrPos[1].Value<float>(),
                            arrPos[2].Value<float>());
                }
                m_otherPlayers[item[EPlayerProfile2String(EPlayerProfile.ClientId)].ToString()].transform.position = pos;
                Debug.Log(" otherPlayers pos :  " + pos);
            }
        }
    }

    public void OnConnectWebsocket(Action<bool> callback = null)
    {
        url = baseUrl + ":" + HOST;
        Connect(url, callback);
        //OnReceived = ReceiveSocket;
    }
    async void Connect(string uri, Action<bool> callback = null)
    {
        try
        {
            webSocket = new WebSocket(uri);

            Debug.Log(" webSocket connect ===========================================  " + webSocket.State);
            webSocket.OnOpen += () =>
            {
                Debug.Log("WS OnOpen  ");
                OnRequestRoom();
                callback?.Invoke(true);
            };
            webSocket.OnMessage += (bytes) =>
            {
                // Reading a plain text message
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
                ReceiveSocket(message);
            };

            webSocket.OnError += (string errMsg) =>
            {
                Debug.Log("WS error: " + errMsg);
                callback?.Invoke(false);
            };

            // Add OnClose event listener
            webSocket.OnClose += (WebSocketCloseCode code) =>
            {
                Debug.Log("WS closed with code: " + code.ToString());

                if(code.ToString() != "Normal")
                {
                    OnDisconnect();
                }
            };
            // Keep sending messages at every 0.3s
            //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);
            //Receive();
            await webSocket.Connect();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }

    private async void Send(string message)
    {
        var encoded = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);


        //await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        if (webSocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await webSocket.Send(encoded);

            // Sending plain text
            //await webSocket.SendText(message);
        }
    }
    private void Receive()
    {
        while (webSocket.State== WebSocketState.Open)
        {
            webSocket.OnMessage += (byte[] msg) =>
            {

                Debug.Log("WS received message:  " + System.Text.Encoding.UTF8.GetString(msg));
                string message = Encoding.UTF8.GetString(msg);

                Debug.Log("session response : " + message);
                if (OnReceived != null) OnReceived(message);

            };

            // Add OnError event listener
            webSocket.OnError += (string errMsg) =>
            {
                Debug.Log("WS error: " + errMsg);
            };

            // Add OnClose event listener
            webSocket.OnClose += (WebSocketCloseCode code) =>
            {
                Debug.Log("WS closed with code: " + code.ToString());
            };

        }
    }
    void OnAddPlayer(JToken player)
    {
        bool isFound = false;

        for(int i = 0; i < m_players.Count; i++)
            if(m_players[i][EPlayerProfile2String(EPlayerProfile.Index)].Value<int>() ==  player[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>())
            {
                m_players[i] = player;
                isFound = true;
                break;
            }

        if (!isFound)
            m_players.Add(player);
    }

    bool IsSpectator(string playerId)
    {
        bool isSpectator = false;

        foreach(var player in m_players)
        {
            if(player[EPlayerProfile2String(EPlayerProfile.ClientId)].ToString() == playerId)
            {
                if (player[EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "1")
                    isSpectator = true;

                break;
            }    
        }    

        return isSpectator;
    }    

    int CountAliveSpectator(bool isLobby)
    {
        int spectatorCount = 0;
        var alivePlayers = isLobby ? m_aliveLobbyPlayer : m_aliveIndexPlayers;

        foreach (var index in alivePlayers)
        {
            foreach(var player in m_players)
            {
                if (player[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>() == index && player[EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "1")
                    spectatorCount++;
            }    
        }
        Debug.Log($"[SocketClient] GetAliveSpectator = {spectatorCount}");
        return spectatorCount;
    }    

    int CountAllPlayerInGame()
    {
        return m_aliveIndexPlayers.Count - CountAliveSpectator(isLobby: false);
    }
    int CountAllPlayerInLobby()
    {
        return m_aliveLobbyPlayer.Count - CountAliveSpectator(isLobby: true);
    }    

    byte[] StringToByteArray(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    void OnCreateOtherPlayer(JObject data)
    {
        Debug.Log("[SocketClient] OnCreateOtherPlayer  other player data:" + data);
        string _clientId = data["clientId"].ToString();

        if (!m_otherPlayers.ContainsKey(_clientId))
        {
            int rand = GetPlayerIndex(data[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>());
            Vector3 pos = PositionByIndex(rand);
            Debug.Log("[SocketClient] OnCreateOtherPlayer  other player =================  " + pos);
            // other player
            if (data[EPlayerProfile2String(EPlayerProfile.Gender)].ToString() == "0")
            {
                otherPlayerPrefab.GetComponent<OtherPlayer>().SetActiveCharacter(0);
                m_otherPlayers[_clientId] = Instantiate(otherPlayerPrefab, pos, Quaternion.identity);
            }
            else
            {
                otherPlayerPrefab.GetComponent<OtherPlayer>().SetActiveCharacter(1);
                m_otherPlayers[_clientId] = Instantiate(otherPlayerPrefab, pos, Quaternion.identity);
            }

            m_otherPlayers[_clientId].name = _clientId;
            m_otherPlayers[_clientId].transform.tag = "enemy";
            OtherPlayer otherPlayer = m_otherPlayers[_clientId].GetComponent<OtherPlayer>();
            otherPlayer.IndexPlayer = data[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>();

            var playerUI = m_otherPlayers[_clientId].gameObject.GetComponent<Player>();
            playerUI.IndexPlayer = otherPlayer.IndexPlayer;
            playerUI.OnInitialize(m_otherPlayers[_clientId], data[EPlayerProfile2String(EPlayerProfile.NickName)].ToString(), MainMenu.instance.IsSpectatorMode());

            m_otherPlayers[_clientId].SetActive(true);
            //GameManager.Instance.ShowDebugInfo($"\n create player  = {_clientId}");
            Debug.Log($"===>[SocketClient] created instantiate  other player = {m_otherPlayers[_clientId]} otherPlayer.IndexPlayer = {otherPlayer.IndexPlayer} _clientId = {_clientId}");         
        }
    }

    string GetPlayerName(int index)
    {
        string playerName = "";

        foreach (var player in m_players)
        {
            if (player[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>() == index)
            {
                playerName = player[EPlayerProfile2String(EPlayerProfile.NickName)].ToString();
            }
        }

        return playerName;
    }

    int GetPlayerIndex(string playerID)
    {
        int index = -1;

        if(m_otherPlayers.TryGetValue(playerID, out GameObject player))
        {
            index = player.GetComponent<OtherPlayer>().IndexPlayer;
            Debug.Log($"[SocketClient] GetPlayerIndex playerID = {playerID} => {index}");
        }    
          
        return index;
    }
    JObject GetPlayerData(int index)
    {
        foreach(var player in m_players)
        {
            if(player[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>() == index)
            {
                return player.Value<JObject>();
            }    
        }

        return null;
    }    
    void ReceiveSocket(string message)
    {
        JObject data = JObject.Parse(message);
        EServerCmd serverCmd = String2EServerCmd(data["event"].ToString());       

        switch (serverCmd)
        {
            case EServerCmd.RoomDetected:
                ROOM = data[EPlayerProfile2String(EPlayerProfile.RoomId)].ToString();
                m_localClientId = data["clientId"].ToString();
                //OnJoinRoom();
                OnJoinLobbyRoom();

                break;
            case EServerCmd.FailJoinRoom:

                MainMenu.instance.ShowFailScreen(data["message"].ToString());
                break;
            case EServerCmd.RoundAlready:
                Debug.Log("roundAlready  ===============================  " + data );
                if (LevelManager.Instance.isStartGame == false)
                {
                    Debug.Log("roundAlready  ===============================  " + data);
                    LevelManager.Instance.startGame();
                }
                    
                break;
            case EServerCmd.JoinLobbyRoom:

                IS_FIRST_JOIN = false;
                MainMenu.instance.ShowLobby();

                if(data.ContainsKey("players"))
                {
                    var players = JArray.Parse(data["players"].ToString());

                    foreach(var player in players)
                    {
                        OnAddPlayer(player);
                    }    
                }
                else if (data.ContainsKey("newPlayer"))
                {
                    var newPlayer = data["newPlayer"];
                    OnAddPlayer(newPlayer);
                }
                               
                var aliveLobbyPlayer = JArray.Parse(data[EPlayerProfile2String(EPlayerProfile.AliveLobbyPlayers)].ToString());

                foreach (var item in aliveLobbyPlayer)
                    if (!m_aliveLobbyPlayer.Contains(item.Value<int>()))
                        m_aliveLobbyPlayer.Add(item.Value<int>());         

                Debug.Log($"[SocketClient] joinRoom player m_aliveLobbyPlayer.Count = {m_aliveLobbyPlayer.Count}");

                currentPlayerJoined = m_players.Count;
                Debug.Log(" playerName  join room  " + data[EPlayerProfile2String(EPlayerProfile.NickName)].ToString());

                int countUserPlay = 0;
                int countSpectator = 0;
                // for new player
                if (data["clientId"].ToString() == m_localClientId && m_player == null)
                {

                    for (int i = 0; i < m_players.Count; i++)
                    {
                        m_isHost = data[EPlayerProfile2String(EPlayerProfile.IsHost)].ToString() == "1";

                        if (m_players[i][EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "0")
                        {
                            countUserPlay++;
                            StartCoroutine(LoadAvatarImage(m_players[i][EPlayerProfile2String(EPlayerProfile.Avatar)].ToString(), m_players[i][EPlayerProfile2String(EPlayerProfile.ClientId)].ToString()));
                        }
                        else
                        {
                            countSpectator++;
                        }
                    }
                }
                // for old player
                else
                {
                    
                    if (data[EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "0")
                        StartCoroutine(LoadAvatarImage(data[EPlayerProfile2String(EPlayerProfile.Avatar)].ToString(), data["clientId"].ToString()));
                }
                if (data[EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "0")
                    MainMenu.instance.ShowPlayerJoinRoom(data[EPlayerProfile2String(EPlayerProfile.NickName)].ToString());

                MainMenu.instance.ShowTotalPlayers(CountAllPlayerInLobby());

                //clientPosStart = RandomPosition();
                break;

            case EServerCmd.GotoGame:
                MainMenu.instance.GotoGame();
                m_gameState = EGameState.InGame;
                //OnCheckPosition();
                break;
            case EServerCmd.PositionPlayer:
                if (data["clientId"].ToString() == m_localClientId)
                {
                    int _indexRan = int.Parse(data["ranIndex"].ToString());
                    m_clientPosStart = PositionByIndex(_indexRan);
                }
                
                break;            
            case EServerCmd.JoinRoom:

                //players = JArray.Parse(data["players"].ToString());
                JArray arrRanPos = JArray.Parse(data["roomPos"].ToString());
                var aliveIndexPlayers = JArray.Parse(data[EPlayerProfile2String(EPlayerProfile.AlivePlayers)].ToString());

                foreach (var item in aliveIndexPlayers)
                {
                    if(!m_aliveIndexPlayers.Contains(item.Value<int>()))
                        m_aliveIndexPlayers.Add(item.Value<int>());
                }

                Debug.Log($"[SocketClient] joinRoom player m_aliveIndexPlayers.Count = {m_aliveIndexPlayers.Count}");
                Debug.Log(" arrRanPos ------------------------ " + arrRanPos[0]);
                JArray arrPos;

                string _clientId = data["clientId"].ToString();
                if (_clientId == m_localClientId )
                {
                    m_localPlayerIndex = data[EPlayerProfile2String(EPlayerProfile.Index)].Value<int>();

                    if (m_player == null && data[EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "0")
                    {   
                        int rand = GetPlayerIndex(m_localPlayerIndex);
                        m_clientPosStart = PositionByIndex(rand);
                        
                        Debug.Log("[SocketClient]  ===========  player ================= m_localPlayerIndex = " + m_localPlayerIndex);           
                        
                        m_isHost = data[EPlayerProfile2String(EPlayerProfile.IsHost)].ToString() == "1";   
                        if (data[EPlayerProfile2String(EPlayerProfile.Gender)].ToString() == "0")
                        {
                            playerPrefab.GetComponent<characterSpawn>().SetActiveCharacter(0);
                            m_player = Instantiate(playerPrefab, m_clientPosStart, Quaternion.identity);
                        }
                        else
                        {
                            playerPrefab.GetComponent<characterSpawn>().SetActiveCharacter(1);
                            m_player = Instantiate(playerPrefab, m_clientPosStart, Quaternion.identity);
                        }

                        var playerUI = m_player.gameObject.GetComponent<Player>();
                        playerUI.IndexPlayer = m_localPlayerIndex;
                        playerUI.OnInitialize(m_player, data[EPlayerProfile2String(EPlayerProfile.NickName)].ToString(), MainMenu.instance.IsSpectatorMode());

                        m_player.name = "Player-" + data[EPlayerProfile2String(EPlayerProfile.NickName)].ToString();
                        m_player.transform.tag = "Player";
                        m_player.SetActive(true);
                        Debug.Log(" Instantiate  player =================  " + m_player);                        
                    }                
                }
                else if (_clientId != m_localClientId && data[EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "0")
                {
                    Debug.Log("  ===========  other player =================  ");

                    //if (!otherPlayers.ContainsKey(_clientId))
                    OnCreateOtherPlayer(data);
                }


                //StartCoroutine(UpdatePositionOtherPlayers());

                break;
            case EServerCmd.StartGame:
                Debug.Log(" [SocketClient] startGame =================  " + data);
                //GameManager.Instance.ShowDebugInfo($"\n m_otherPlayers.Count = {m_otherPlayers.Count}");
                
                if(CountAllPlayerInGame() >  1)
                    m_isSinglePlayer = false;
                else
                    m_isSinglePlayer = true;

                int totalPlayer = MainMenu.instance.IsSpectatorMode() ? m_otherPlayers.Count : m_otherPlayers.Count + 1;

                if (CountAllPlayerInGame() != totalPlayer)
                {
                    Debug.Log($"[SocketClient] !!!startGame with count other players = {m_otherPlayers.Count}");
                  
                    foreach(var indexPlayer in m_aliveIndexPlayers)
                    {
                        bool isFound = false;

                        foreach(var otherPlayer in m_otherPlayers.Values)
                        {
                            OtherPlayer cotherPlayer = otherPlayer.GetComponent<OtherPlayer>();

                            if(cotherPlayer.IndexPlayer == indexPlayer)
                            {
                                isFound = true;
                                break;
                            }
                        }

                        if (!isFound && m_localPlayerIndex != indexPlayer)
                        {
                            Debug.Log($"===> [SocketClient] !!! create missing otherPlayer index = {indexPlayer}");
                            var dataPlayer = GetPlayerData(indexPlayer);
                            OnCreateOtherPlayer(dataPlayer);
                        }
                    }
                }

                if (m_isSpectator)
                {
                    // code spectator screen here

                }
                else
                {
                    //StartCoroutine(UpdatePositionOtherPlayers());
                    LevelManager.Instance.startGame();
                    // start moving
                    //LoopCheckPlayerMoving();
                    isEndGame = false;
                }
                break;
            case EServerCmd.HitEnemy:
                Debug.Log("  hitEnemy =================  " + data);

                if(m_localClientId == data["clientId"].ToString())
                {                    
                    Vector3 pos = Vector3.zero;
                    arrPos = JArray.Parse(data["hitPos"].ToString());
                    if (arrPos.Count > 0)
                    {
                        pos = new Vector3(arrPos[0].Value<float>(),
                                arrPos[1].Value<float>(),
                                arrPos[2].Value<float>());
                    }

                    m_player.GetComponent<AnimationControl>().PlayerHitEnemy(pos);
                }
                if (m_otherPlayers.ContainsKey(data["clientId"].ToString()))
                {
                    m_otherPlayers[data["clientId"].ToString()].GetComponent<OtherPlayer>().SetAnimHit();
                }

                //if (clientId == data["hitEnemyId"].ToString())
                //{
                //    Vector3 pos = Vector3.zero;
                //    arrPos = JArray.Parse(data["hitPos"].ToString());
                //    if (arrPos.Count > 0)
                //    {
                //        pos = new Vector3(arrPos[0].Value<float>(),
                //                arrPos[1].Value<float>(),
                //                arrPos[2].Value<float>());
                //    }
                //    //StartCoroutine(CheckPlayerStunned(pos, data["clientId"].ToString()));

                //    //player.GetComponent<AnimationControl>().PlayerHitEnemy(pos);
                //}

                break;
            case EServerCmd.Stunned:
                Debug.Log("  stunned =================  " + data);

                if (m_localClientId == data["clientId"].ToString())
                {
                    LevelManager.Instance.OnDisableController(1.5f);
                    stunnedByEnemyId = data["stunnedByEnemyId"].ToString();
                    Vector3 pos = Vector3.zero;
                    arrPos = JArray.Parse(data["hitPos"].ToString());
                    if (arrPos.Count > 0)
                    {
                        pos = new Vector3(arrPos[0].Value<float>(),
                                arrPos[1].Value<float>(),
                                arrPos[2].Value<float>());
                    }

                    m_player.GetComponent<AnimationControl>().PlayerStunned(pos);
                }
                if (m_otherPlayers.ContainsKey(data["clientId"].ToString()))
                {
                    m_otherPlayers[data["clientId"].ToString()].GetComponent<OtherPlayer>().SetAnimStunned();
                }


                break;
            case EServerCmd.UpdatePos:
                Debug.Log("  updatePos =================  " + data);
                if (m_otherPlayers.ContainsKey(data["clientId"].ToString()))
                {
                    Vector3 pos = Vector3.zero;
                    arrPos = JArray.Parse(data["pos"].ToString());
                    if (arrPos.Count > 0)
                    {
                        pos = new Vector3(arrPos[0].Value<float>(),
                                arrPos[1].Value<float>(),
                                arrPos[2].Value<float>());
                    }
                    Quaternion rot = Quaternion.identity;
                    JArray arrRot = JArray.Parse(data["rot"].ToString());
                    if (arrRot.Count > 0)
                    {
                        rot = new Quaternion(arrRot[0].Value<float>(),
                                arrRot[1].Value<float>(),
                                arrRot[2].Value<float>(),
                                arrRot[3].Value<float>());
                    }

                    m_otherPlayers[data["clientId"].ToString()].GetComponent<OtherPlayer>().transform.position = pos;
                    m_otherPlayers[data["clientId"].ToString()].GetComponent<OtherPlayer>().transform.rotation = rot;
                }
                    

                break;
            case EServerCmd.CountDown:
                //Debug.Log("  countDown =================  " + data);
                float timer = float.Parse(data["timer"].ToString());
                OnCheckWinnerPlayer(m_localClientId, m_aliveLobbyPlayer.Count, CubeManager.Instance.RoundCount);

                if (Mathf.FloorToInt(timer) > 0)
                {
                    StartCoroutine(TimerCountdown());
                }
                
                break;
            case EServerCmd.Moving:

                float h = float.Parse(data["h"].ToString());
                float v = float.Parse(data["v"].ToString());
                Vector3 posVeclocity = Vector3.zero;
                JArray arrPosV = JArray.Parse(data["velocity"].ToString());
                if (arrPosV.Count > 0)
                {
                    posVeclocity = new Vector3(arrPosV[0].Value<float>(),
                            arrPosV[1].Value<float>(),
                            arrPosV[2].Value<float>());
                }

                Debug.Log("  moving position data posVeclocity =================  " + posVeclocity);
                if (m_localClientId == data["clientId"].ToString())
                {
                    //Debug.Log("  moving position data   ==========  " + player.transform.position);
                    //player.GetComponent<Mover>().SetVelocityFromServer(posVeclocity);
                    m_player.GetComponent<AdvancedWalkerController>().moving_h = h;
                    m_player.GetComponent<AdvancedWalkerController>().moving_v = v;
                }
                else
                {
                    if(m_otherPlayers.Count > 0)
                    {
                        if (m_otherPlayers.ContainsKey(data["clientId"].ToString()))
                        {
                            //Debug.Log("  moving position data other hhhhhhhhhhh =================  " + h);
                            //Debug.Log("  moving position data other vvvvvvvvvvvvvvv =================  " + v);
                            //Debug.Log("  moving position data other   ==========  " + otherPlayers[data["clientId"].ToString()].transform.position);
                            //otherPlayers[data["clientId"].ToString()].GetComponent<AdvancedWalkerController>().SetInputMovementVelocity(posVeclocity);
                            m_otherPlayers[data["clientId"].ToString()].GetComponent<OtherPlayer>().SetVelocity(posVeclocity);
                            //otherPlayers[data["clientId"].ToString()].GetComponent<AdvancedWalkerController>().moving_h = h;
                            //otherPlayers[data["clientId"].ToString()].GetComponent<AdvancedWalkerController>().moving_v = v;
                        }
                        else
                        {
                            Debug.Log("  moving position data nullllllll ==========  " + data);
                        }
                            
                    }
                }

                break;
            case EServerCmd.ResponseTarget:
                Debug.Log("  responseTarget data ==========  " + data);     
                byte[] _rans = StringToByteArray(data["rans"].ToString());  
                CubeManager.Instance.PerformCube(_rans);
                OnCheckWinnerPlayer(m_localClientId, m_aliveLobbyPlayer.Count, CubeManager.Instance.RoundCount);

                break;
            case EServerCmd.CubeFall:
                Debug.Log("  cubeFall data ==========  " + data);
                OnCheckWinnerPlayer(m_localClientId, m_aliveLobbyPlayer.Count, CubeManager.Instance.RoundCount);
                CubeManager.Instance.PerformCubeFall();
                break;

            case EServerCmd.CubeReset:
                Debug.Log("  cubeReset data ==========  " + data);
                OnCheckWinnerPlayer(m_localClientId, m_aliveLobbyPlayer.Count, CubeManager.Instance.RoundCount);
                CubeManager.Instance.PerformCubeReset();
                break;
            case EServerCmd.RoundPass:
                {
                    Debug.Log("  roundPass data ==========  " + data);
                    string playerID = data["clientId"].ToString();
                    int serverCountPlayer = int.Parse(data[EPlayerProfile2String(EPlayerProfile.CountPlayers)].ToString());
                    int countRoundPassed = int.Parse(data["roundPass"].ToString());

                    OnCheckWinnerPlayer(playerID, serverCountPlayer, countRoundPassed);
                }               
                break;
            case EServerCmd.PlayerDie:
                Debug.Log("  playerDie data ==========  " + data);
                string diePlayerId = data["clientId"].ToString();
                int dieIndex = GetPlayerIndex(diePlayerId);

                if (dieIndex >= 0)
                {
                    string diePlayerName = GetPlayerName(dieIndex);

                    if(!string.IsNullOrEmpty(diePlayerName))
                        GameResultMgr.Instance.OnAddPlayerData(diePlayerId, diePlayerName, isWinner: false);

                    OnUpdateCountPlayers(dieIndex);
                }

                if (m_localClientId == diePlayerId)
                {
                    StopCoroutine("CheckPlayerMoving");
                    isEndGame = true;
                    //players.RemoveAll();
                    //foreach (var item in otherPlayers)
                    //{
                    //    Destroy(otherPlayers[item.Key]);

                    //}
                    
                    OnCloseConnectSocket();
                    Destroy(m_player);
                }
                //if (otherPlayers.ContainsKey(data["clientId"].ToString()))
                //{
                //    Destroy(otherPlayers[data["clientId"].ToString()]);
                //}

                break;
            case EServerCmd.PlayerWin:
                Debug.Log("  playerWin data ==========  " + data);
                string winPlayerId = data["clientId"].ToString();
                int winIndex = GetPlayerIndex(winPlayerId);

                if (winIndex >= 0)
                {
                    string winPlayerName = GetPlayerName(winIndex);

                    if (!string.IsNullOrEmpty(winPlayerName))
                        GameResultMgr.Instance.OnAddPlayerData(winPlayerId, winPlayerName, isWinner: true);
                }              

                if (m_localClientId == winPlayerId)
                {
                    StopCoroutine("CheckPlayerMoving");
                    isEndGame = true;
                    //players.RemoveAll();
                    //foreach (var item in otherPlayers)
                    //{
                    //    Destroy(otherPlayers[item.Key]);

                    //}
                    OnCloseConnectSocket();
                }

                break;
            case EServerCmd.EndGame:
                Debug.Log("  endGame data ==========  " + data);  
                
                
                break;
            case EServerCmd.PlayerLeaveRoom:
                string playerLeaveId = data["clientId"].ToString();

                for (int i = 0; i < m_players.Count; i++)
                {
                    //Debug.Log(" players player leave ==   " + players[i].ToString());
                    if (playerLeaveId == m_players[i][EPlayerProfile2String(EPlayerProfile.ClientId)].ToString())
                    {
                        int leaveIndex = m_players[i][EPlayerProfile2String(EPlayerProfile.Index)].Value<int>();

                        OnUpdateCountPlayers(leaveIndex);

                        if (m_gameState == EGameState.InLobby)
                        {
                            if (m_players[i][EPlayerProfile2String(EPlayerProfile.IsSpectator)].ToString() == "0")
                            {
                                MainMenu.instance.RemovePlayerJoinRoomByAvatar(playerLeaveId);
                            }
                            MainMenu.instance.ShowTotalPlayers(CountAllPlayerInLobby());
                        }
                       
                        m_players[i] = null;
                        m_players.RemoveAt(i);
                        Debug.Log(" players playerLeaveRoom 222222222222222  " + playerLeaveId);
                    }                    
                }

                if (m_otherPlayers.ContainsKey(playerLeaveId))
                {
                    m_otherPlayers[playerLeaveId].SetActive(false);
                    Destroy(m_otherPlayers[playerLeaveId]);
                    m_otherPlayers.Remove(playerLeaveId);                
                }                    

                if (MainMenu.instance.IsSpectatorMode() && m_gameState == EGameState.InGame)
                {
                    if (CountAllPlayerInGame() == 0)
                    {
                        LevelManager.Instance.ShowEndGameScreen();
                        isEndGame = true;
                        OnCloseConnectSocket();
                    }
                }

                
                // check new host 
                string checkNewHost = data["newHost"].ToString();

                if (checkNewHost != "" && checkNewHost == m_localClientId)
                {
                    m_isHost = true;

                    if (m_player != null)
                    {
                        Debug.Log(" client is new host -----------    ");
                        LevelManager.Instance.CheckHost();
                    }
                    else
                    {
                        Debug.Log(" client is new lobby host ---=====  ");

                        MainMenu.instance.isHost = "1";
                        MainMenu.instance.CheckTheHost();
                    }
                }
                
                break;
            default:
                break;
        }
    }

    IEnumerator CheckPlayerStunned(Vector3 pos , string enemyId)
    {
        yield return new WaitForSeconds(0.3f);
        if (stunnedByEnemyId != "" && stunnedByEnemyId == enemyId)
        {
            m_player.GetComponent<AnimationControl>().PlayerStunned(pos);
            stunnedByEnemyId = "";
        }
    }
    IEnumerator TimerCountdown()
    {
        yield return new WaitForSeconds(1f); 
        OnCountDown();
    }
    public void OnRequestRoom()
    {
        m_gameState = EGameState.InLobby;
        Debug.Log("  MainMenu.instance.isSpectator OnRequestRoom =================  " + MainMenu.instance.isSpectator);
        string room = MainMenu.instance.roomId;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.RequestRoom));
        jsData.Add("playerLen", 8);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), room);
        jsData.Add("host", MainMenu.instance.isHost);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.IsSpectator), MainMenu.instance.isSpectator);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }

    public void OnJoinLobbyRoom()
    {
        Debug.Log("  MainMenu.instance.isSpectator OnJoinLobbyRoom   " + MainMenu.instance.isSpectator);
        Debug.Log("  MainMenu.instance.gender gender   " + MainMenu.instance.gender);
        string playerName = MainMenu.instance.playerName;

        if (playerName.Length <= 1)
        {
            playerName = "anonymous";
        }

        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.JoinLobby));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.IsHost), MainMenu.instance.isHost);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.Gender), MainMenu.instance.gender);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.IsSpectator), MainMenu.instance.isSpectator);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.NickName), playerName);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.AppId), MainMenu.instance.userAppId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.Avatar), MainMenu.instance.userAvatar);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.PhoneNumber), MainMenu.instance.phoneNumber);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.FollowedOA), MainMenu.instance.followedOA);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnGotoGame()
    {      
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.GotoGame));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnJoinRoom()
    {
        Debug.Log(" OnJoinRoom ==================  " );       
        string playerName = MainMenu.instance.playerName;

        if (playerName.Length <= 1 )
        {
            playerName = "anonymous";
        }

        //clientPosStart = RandomPosition();

        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.Join));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.IsHost), MainMenu.instance.isHost);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.NickName), playerName);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.CharacterIndex), MainMenu.instance.selectedCharacter);
        jsData.Add("pos", m_clientPosStart.ToString());
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnStartGame()
    {        
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.StartGame));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnHitEnemy(Vector3 hitPos, string enemyName)
    {
        if (!LevelManager.Instance.isStartGame) return;

        //string enemyId = enemyName.Replace("otherplayer-","");
        //Debug.Log("enemyName ==================  " + enemyName + "    , sub  = " + enemyId);
        if (isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.HitEnemy));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add("hitPos", hitPos.ToString());
        jsData.Add("enemyId", enemyName);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnStunnedByEnemy(Vector3 hitPos, string enemyName)
    {
        if (!LevelManager.Instance.isStartGame) return;

        //string enemyId = enemyName.Replace("otherplayer-", "");
        //Debug.Log("enemyName ==================  " + enemyName + "    , sub  = " + enemyId);
        if (isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.Stunned));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add("hitPos", hitPos.ToString());
        jsData.Add("enemyId", enemyName);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnUpdatePosPlayer()
    {
        if (isEndGame || !m_player) return;
        Vector3 pos = m_player.transform.position;
        Quaternion rot = m_player.GetComponent< AdvancedWalkerController>().modelTransform.localRotation;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.UpdatePos));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add("pos", pos.ToString());
        jsData.Add("rot", rot.ToString());
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnCheckPosition()
    {

        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.CheckPosition));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add("ranLength", GetRandomLength());
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnRoundAlready()
    {
        if (!m_isHost || isEndGame) return;
        Debug.Log(" OnCountDown =================  ");
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.RoundAlready));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add("ready", "1");
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnRoundPass(int round)
    {
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.RoundPass));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.Round), round);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnCountDown()
    {
        if (!m_isHost || isEndGame) return;
        Debug.Log(" OnCountDown =================  ");
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.CountDown));
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add("timer", "");
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnMoving(Vector3 _velocity, float _h, float _v)
    {
        if (isEndGame || !m_player) return;
        m_clientPosStart = m_player.transform.position;
        //Quaternion rot = player.transform.rotation;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.Moving));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        jsData.Add("velocity", _velocity.ToString());
        jsData.Add("h", _h);
        jsData.Add("v", _v);
        jsData.Add("pos", m_clientPosStart.ToString());
        //jsData.Add("rot", rot.ToString());
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnRequestRandomTarget(byte[] _rans)
    {
        if (!m_isHost || isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.RequestTarget));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        string byteArray = BitConverter.ToString(_rans).Replace("-", "");        
        jsData.Add("rans", byteArray);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    
    public void OnCubeFall()
    {
        if (!m_isHost || isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.CubeFall));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnCubeReset()
    {
        if (!m_isHost || isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.CubeReset));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnPlayerDie()
    {
        Debug.Log(" ======================== OnPlayerDie() ======================================");
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.PlayerDie));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnPlayerWin()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.PlayerWin));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnEndGame()
    {
        if (!m_isHost) return;
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.EndGame));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnLeaveRoom()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", EServerCmd2String(EServerCmd.Leave));
        jsData.Add("clientId", m_localClientId);
        jsData.Add(EPlayerProfile2String(EPlayerProfile.RoomId), ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public async void OnCloseConnectSocket()
    {
        m_gameState = EGameState.None;
        m_localClientId = "";
        ROOM = "";
        isEndGame = true;
        if (m_player)
        {
            Destroy(m_player);
            m_player = null;
        }
        m_players.Clear();

        foreach(var otherPlayer in m_otherPlayers.Values)
        {
            otherPlayer.SetActive(false);
            Destroy(otherPlayer);
        }    
        m_otherPlayers.Clear();
        
        m_aliveIndexPlayers.Clear();
        m_aliveLobbyPlayer.Clear();
        m_isSinglePlayer = true;
        await webSocket.Close();
    }
    public void OnDisconnect()
    {
        m_localClientId = "";
        ROOM = "";
        isEndGame = true;
        m_players.Clear();
        
        foreach (var otherPlayer in m_otherPlayers.Values)
        {
            otherPlayer.SetActive(false);
            Destroy(otherPlayer);
        }
        m_otherPlayers.Clear();
        m_isSinglePlayer = true;

        if (m_player)
        {
            Destroy(m_player);
            m_player = null;
            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator LoadAvatarImage(string imageUrl, string playerID)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            Texture2D textureImageUrl = null;
            MainMenu.instance.AddPlayerJoinRoomByAvatar(textureImageUrl, playerID);
        }
        else
        {
            Texture2D textureImageUrl = ((DownloadHandlerTexture)request.downloadHandler).texture;
            // use the texture here
            MainMenu.instance.AddPlayerJoinRoomByAvatar(textureImageUrl, playerID);
        }
    }

}
