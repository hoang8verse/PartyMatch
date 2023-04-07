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

public class SocketClient : MonoBehaviour
{

    public static SocketClient instance;

    public delegate void ReceiveAction(string message);
    public event ReceiveAction OnReceived;

    //public ClientWebSocket webSocket = null;
    public WebSocket webSocket;

    [SerializeField]
    private string url = "";
    static string baseUrl = "ws://192.168.1.39";
    static string HOST = "3000";

    //static string baseUrl = "wss://rlgl2-api.brandgames.vn";
    //static string HOST = "8081";

    public string ROOM = "";
    public string clientId = "";

    public string playerJoinName = "";
    public int currentPlayerJoined = 0;

    public bool isHost = false;
    public bool isSpectator = false;

    public static bool IS_FIRST_JOIN = true;

    [SerializeField]
    private GameObject playerPrefab;
    public GameObject player = null;
    [SerializeField]
    private GameObject otherPlayerPrefab;
    public JArray players;

    private Dictionary<string,GameObject> otherPlayers;

    [SerializeField]
    private GameObject spectatorPrefab;

    [SerializeField]
    private GameObject cubeObjectPrefab;

    Vector3 clientPosStart;
    string hitEnemyId = "";
    string stunnedByEnemyId = "";
    public bool isEndGame = false;
    bool isEndMovePlayer = false;
    void Awake()
    {
        CheckDevice();
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
    }
    void Start()
    {
        //OnConnectWebsocket();

        otherPlayers = new Dictionary<string, GameObject>();
    }

    void Update()
    {
        //if (!isEndGame)
        //{
        //    if (isRunOnMobile)
        //    {
        //        if (Input.touchCount > 0)
        //        {
        //            Touch touch = Input.GetTouch(0); // trying to get the second touch input

        //            if (touch.phase == TouchPhase.Began)
        //            {

        //                isMoving = true;
        //            }
        //            else if (touch.phase == TouchPhase.Moved)
        //            {

        //                isMoving = true;

        //                if (player)
        //                {

        //                    Vector3 _velocity = player.GetComponent<Mover>().GetVelocity();
        //                    Debug.Log(" ======================== GetMouseButtonUp _velocity ===========  " + _velocity);
        //                    OnMoving(_velocity);
        //                    //_v = player.GetComponent<CharacterInput>().GetVerticalMovementInput();
        //                }
        //            }
        //            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        //            {
        //                OnMoving(Vector3.zero);
        //                isMoving = false;

        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (Input.GetMouseButton(0)) // 0 : left , 1 : right, 2 : wheel
        //        {

        //            isMoving = true;
        //            if (player)
        //            {
        //                Vector3 _velocity = player.GetComponent<Mover>().GetVelocity();
        //                OnMoving(_velocity);
        //                //_v = player.GetComponent<CharacterInput>().GetVerticalMovementInput();
        //            }

        //        }
        //        else
        //        if (Input.GetMouseButtonUp(0))
        //        {

        //            isMoving = false;
        //            OnMoving(Vector3.zero);
                    
        //        }
        //    }
        //}
        

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
    private Vector3 RandomPosition()
    {
        System.Random rand = new System.Random();
        int ranIndex = rand.Next(0, cubeObjectPrefab.GetComponent<CubeManager>().CubeMeshs.Length);
        Vector3 randomPoint = cubeObjectPrefab.GetComponent<CubeManager>().CubeMeshs[ranIndex].transform.position;
        return randomPoint;
    }
    bool isRunOnMobile;
    public bool isMoving = false;
    void CheckDevice()
    {
        if (Application.isMobilePlatform)
        {
            Debug.Log("Running on a mobile device.");
            isRunOnMobile = true;
        }
        else
        {
            Debug.Log("Running on a non-mobile device.");
            isRunOnMobile = false;
        }
    }

    IEnumerator CheckPlayerMoving()
    {

        if (isEndGame || !player) yield return null ;
        float _h = 0;
        float _v = 0;
        Vector3 _velocity = Vector3.zero;
        if (player)
        {
            _velocity = player.GetComponent<Mover>().GetVelocity();
            _h = player.GetComponent<CharacterInput>().GetHorizontalMovementInput();
            _v = player.GetComponent<CharacterInput>().GetVerticalMovementInput();
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
    IEnumerator UpdatePositionOtherPlayers()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        foreach (var item in players)
        {
            if (otherPlayers.ContainsKey(item["id"].ToString()))
            {
                Vector3 pos = Vector3.zero;
                JArray arrPos = JArray.Parse(item["position"].ToString());
                if (arrPos.Count > 0)
                {
                    pos = new Vector3(arrPos[0].Value<float>(),
                            arrPos[1].Value<float>(),
                            arrPos[2].Value<float>());
                }
                otherPlayers[item["id"].ToString()].transform.position = pos;
                Debug.Log(" otherPlayers pos :  " + pos);
            }
        }
    }
    void LoopCheckPlayerMoving()
    {
        StartCoroutine("CheckPlayerMoving");
    }

    public void OnConnectWebsocket()
    {
        url = baseUrl + ":" + HOST;
        Connect(url);
        //OnReceived = ReceiveSocket;
    }
    async void Connect(string uri)
    {
        try
        {
            webSocket = new WebSocket(uri);

            Debug.Log(" webSocket connect ===========================================  " + webSocket.State);
            webSocket.OnOpen += () =>
            {
                Debug.Log("WS OnOpen  ");
                OnRequestRoom();
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

    void ReceiveSocket(string message)
    {
        JObject data = JObject.Parse(message);

        switch (data["event"].ToString())
        {
            case "roomDetected":
                ROOM = data["room"].ToString();
                clientId = data["clientId"].ToString();
                //OnJoinRoom();
                OnJoinLobbyRoom();

                break;
            case "failJoinRoom":

                MainMenu.instance.ShowFailScreen(data["message"].ToString());
                break;
            case "roundAlready":
                Debug.Log("roundAlready  ===============================  " + data );
                if (LevelManager.instance.isStartGame == false)
                {
                    Debug.Log("roundAlready  ===============================  " + data);
                    LevelManager.instance.startGame();
                }
                    
                break;
            case "joinLobbyRoom":

                IS_FIRST_JOIN = false;
                MainMenu.instance.ShowLobby();
                players = JArray.Parse(data["players"].ToString());

                currentPlayerJoined = players.Count;
                Debug.Log(" playerName  join room  " + data["playerName"].ToString());

                int countUserPlay = 0;
                int countSpectator = 0;
                // for new player
                if (data["clientId"].ToString() == clientId && player == null)
                {

                    for (int i = 0; i < players.Count; i++)
                    {
                        isHost = data["isHost"].ToString() == "1";

                        if (players[i]["isSpectator"].ToString() == "0")
                        {
                            countUserPlay++;
                            StartCoroutine(LoadAvatarImage(players[i]["avatar"].ToString(), players[i]["id"].ToString()));
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
                    
                    if (data["isSpectator"].ToString() == "0")
                        StartCoroutine(LoadAvatarImage(data["avatar"].ToString(), data["clientId"].ToString()));
                }

                MainMenu.instance.ShowPlayerJoinRoom(data["playerName"].ToString());
                MainMenu.instance.ShowTotalPlayers(players.Count);

                clientPosStart = RandomPosition();
                break;
            case "gotoGame":
                MainMenu.instance.GotoGame();
                clientPosStart = RandomPosition();
                break;
            case "joinRoom":

                players = JArray.Parse(data["players"].ToString());
                //Debug.Log(" joinRoom joinRoom data ------------------------ " + data);

                JArray arrPos;
                JObject playerObj = players.FirstOrDefault(o => (string)o["id"] == clientId) as JObject;

                // created player 

                arrPos = JArray.Parse(playerObj["position"].ToString());
                Vector3 playerPos = Vector3.zero;
                if (arrPos.Count > 0)
                {
                    playerPos = new Vector3(arrPos[0].Value<float>(),
                            arrPos[1].Value<float>(),
                            arrPos[2].Value<float>());
                }
                if (playerObj["id"].ToString() == clientId && player == null)
                {
                    if (player == null)
                    {
                        Debug.Log("  ===========  player =================  ");
                        //  player
                        //isSpectator = _player["isSpectator"].ToString() == "1" ? true : false;
                        //isHost = _player["isHost"].ToString() == "1" ? true : false;

                        //if (_player["isSpectator"].ToString() == "1")
                        //{
                        //    player = Instantiate(spectatorPrefab);
                        //}
                        //else
                        {
                            isHost = playerObj["isHost"].ToString() == "1";
                            //int characterIndex = int.Parse(_player["characterIndex"].ToString());
                            //playerPrefab.GetComponent<characterSpawn>().SetActiveCharacter(characterIndex);
                            //Debug.Log("  characterIndex =================  " + characterIndex);
                            if (playerObj["gender"].ToString() == "0")
                            {
                                player = Instantiate(playerPrefab, clientPosStart, Quaternion.identity);
                            }
                            else
                            {
                                player = Instantiate(playerPrefab, clientPosStart, Quaternion.identity);
                                //player = Instantiate(playerPrefab);
                            }
                            player.name = "Player-" + playerObj["playerName"];
                            player.transform.tag = "Player";
                            player.SetActive(true);
                            Debug.Log(" Instantiate  player =================  " + player);
                        }
                    }
                    else
                    {
                        Debug.Log("  =========== player is same client =================  " + playerPos);
                        //player.transform.position = playerPos;
                    }


                }

                currentPlayerJoined = players.Count;
                Debug.Log(" joinRoom players  " + players);


                foreach (var _player in players)
                {
                    string _clientId = _player["id"].ToString();

                    playerJoinName = _player["playerName"].ToString();
                    Debug.Log(" clientId =================  " + clientId + "   ---   _clientId ==  " + _clientId);
                    Vector3 pos = Vector3.zero;
                    arrPos = JArray.Parse(_player["position"].ToString());
                    if (arrPos.Count > 0)
                    {
                        pos = new Vector3(arrPos[0].Value<float>(),
                                arrPos[1].Value<float>(),
                                arrPos[2].Value<float>());
                    }

                    //if (_clientId == clientId && player == null)
                    //{
                    //    if (player == null)
                    //    {
                    //        Debug.Log("  ===========  player =================  ");
                    //        //  player
                    //        //isSpectator = _player["isSpectator"].ToString() == "1" ? true : false;
                    //        //isHost = _player["isHost"].ToString() == "1" ? true : false;

                    //        //if (_player["isSpectator"].ToString() == "1")
                    //        //{
                    //        //    player = Instantiate(spectatorPrefab);
                    //        //}
                    //        //else
                    //        {
                    //            isHost = _player["isHost"].ToString() == "1";
                    //            //int characterIndex = int.Parse(_player["characterIndex"].ToString());
                    //            //playerPrefab.GetComponent<characterSpawn>().SetActiveCharacter(characterIndex);
                    //            //Debug.Log("  characterIndex =================  " + characterIndex);
                    //            if (_player["gender"].ToString() == "0")
                    //            {
                    //                player = Instantiate(playerPrefab, clientPosStart, Quaternion.identity);
                    //            }
                    //            else
                    //            {
                    //                player = Instantiate(playerPrefab, clientPosStart, Quaternion.identity);
                    //                //player = Instantiate(playerPrefab);
                    //            }
                    //            player.name = "Player-" + playerJoinName;
                    //            player.transform.tag = "Player";
                    //            player.SetActive(true);
                    //            Debug.Log(" Instantiate  player =================  " + player);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Debug.Log("  =========== player is same client =================  " + pos);
                    //        player.transform.position = pos;
                    //    }


                    //}
                    //else
                    if (_clientId != clientId && _player["isSpectator"].ToString() == "0")
                    {
                        Debug.Log("  ===========  other player =================  ");
                        if (!otherPlayers.ContainsKey(_clientId))
                        {
                            Debug.Log("  ===========  player =================  " + pos);


                            // other player
                            if (_player["gender"].ToString() == "0")
                            {
                                otherPlayers[_clientId] = Instantiate(otherPlayerPrefab, pos, Quaternion.identity);
                            }
                            else
                            {
                                otherPlayers[_clientId] = Instantiate(otherPlayerPrefab, pos, Quaternion.identity);
                            }

                            otherPlayers[_clientId].name = _clientId;
                            otherPlayers[_clientId].transform.tag = "enemy";

                            //int characterIndex = int.Parse(_player["characterIndex"].ToString());
                            //otherPlayers[_clientId].GetComponent<characterSpawn>().SetActiveCharacter(characterIndex);
                            //otherPlayers[_clientId].GetComponent<AdvancedWalkerController>().gameObject.SetActive(false);
                            otherPlayers[_clientId].SetActive(true);
                            Debug.Log(" Instantiate  other player  =================  " + otherPlayers[_clientId]);
                        }
                        else
                        {
                            Debug.Log("  =========== other player is same client before  " + pos);
                            if(pos != Vector3.zero)
                            {
                                otherPlayers[_clientId].transform.position = pos;
                            }
                            Debug.Log("  =========== other player is same client affert  " + otherPlayers[_clientId].transform.position);
                        }

                    }

                }

                StartCoroutine(UpdatePositionOtherPlayers());

                break;
            case "startGame":
                Debug.Log("  startGame =================  " + data);
                if (isSpectator)
                {
                    // code spectator screen here

                }
                else
                {
                    StartCoroutine(UpdatePositionOtherPlayers());
                    LevelManager.instance.startGame();
                    // start moving
                    LoopCheckPlayerMoving();
                    isEndGame = false;
                }
                break;
            case "hitEnemy":
                Debug.Log("  hitEnemy =================  " + data);

                if(clientId == data["clientId"].ToString())
                {
                    hitEnemyId = data["hitEnemyId"].ToString();
                    Vector3 pos = Vector3.zero;
                    arrPos = JArray.Parse(data["hitPos"].ToString());
                    if (arrPos.Count > 0)
                    {
                        pos = new Vector3(arrPos[0].Value<float>(),
                                arrPos[1].Value<float>(),
                                arrPos[2].Value<float>());
                    }

                    //player.GetComponent<AnimationControl>().PlayerHitEnemy(pos);
                }
                if (clientId == data["hitEnemyId"].ToString())
                {
                    Vector3 pos = Vector3.zero;
                    arrPos = JArray.Parse(data["hitPos"].ToString());
                    if (arrPos.Count > 0)
                    {
                        pos = new Vector3(arrPos[0].Value<float>(),
                                arrPos[1].Value<float>(),
                                arrPos[2].Value<float>());
                    }
                    StartCoroutine(CheckPlayerStunned(pos, data["clientId"].ToString()));

                    //player.GetComponent<AnimationControl>().PlayerHitEnemy(pos);
                }

                break;
            case "stunned":
                Debug.Log("  stunned =================  " + data);

                if (clientId == data["clientId"].ToString())
                {
                    stunnedByEnemyId = data["stunnedByEnemyId"].ToString();
                    Vector3 pos = Vector3.zero;
                    arrPos = JArray.Parse(data["hitPos"].ToString());
                    if (arrPos.Count > 0)
                    {
                        pos = new Vector3(arrPos[0].Value<float>(),
                                arrPos[1].Value<float>(),
                                arrPos[2].Value<float>());
                    }

                    //player.GetComponent<AnimationControl>().PlayerStunned(pos);
                }


                break;
            case "countDown":
                //Debug.Log("  countDown =================  " + data);
                float timer = float.Parse(data["timer"].ToString());

                if(Mathf.FloorToInt(timer) > 0)
                {
                    StartCoroutine(TimerCountdown());
                }
                
                break;
            case "moving":

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
                if (clientId == data["clientId"].ToString())
                {
                    //Debug.Log("  moving position data   ==========  " + player.transform.position);
                    //player.GetComponent<Mover>().SetVelocityFromServer(posVeclocity);
                    player.GetComponent<AdvancedWalkerController>().moving_h = h;
                    player.GetComponent<AdvancedWalkerController>().moving_v = v;
                }
                else
                {
                    if(otherPlayers.Count > 0)
                    {
                        if (otherPlayers.ContainsKey(data["clientId"].ToString()))
                        {
                            //Debug.Log("  moving position data other hhhhhhhhhhh =================  " + h);
                            //Debug.Log("  moving position data other vvvvvvvvvvvvvvv =================  " + v);
                            //Debug.Log("  moving position data other   ==========  " + otherPlayers[data["clientId"].ToString()].transform.position);
                            //otherPlayers[data["clientId"].ToString()].GetComponent<AdvancedWalkerController>().SetInputMovementVelocity(posVeclocity);
                            otherPlayers[data["clientId"].ToString()].GetComponent<OtherPlayer>().SetVelocity(posVeclocity);
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
            case "responseTarget":
                Debug.Log("  responseTarget data ==========  " + data);
                int _target = int.Parse(data["target"].ToString());
                int _ran1 = int.Parse(data["ran1"].ToString());
                int _ran2 = int.Parse(data["ran2"].ToString());
                int _ran3 = int.Parse(data["ran3"].ToString());

                CubeManager.instance.PerformCube(_target, _ran1, _ran2, _ran3);


                break;
            case "cubeFall":
                Debug.Log("  cubeFall data ==========  " + data);
                CubeManager.instance.PerformCubeFall();
                break;

            case "cubeReset":
                Debug.Log("  cubeReset data ==========  " + data);
                CubeManager.instance.PerformCubeReset();
                break;
            case "playerDie":
                Debug.Log("  playerDie data ==========  " + data);

                if (clientId == data["clientId"].ToString())
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
                //if (otherPlayers.ContainsKey(data["clientId"].ToString()))
                //{
                //    Destroy(otherPlayers[data["clientId"].ToString()]);
                //}

                break;
            case "playerWin":
                Debug.Log("  playerWin data ==========  " + data);
                if (clientId == data["clientId"].ToString())
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
            case "endGame":
                Debug.Log("  endGame data ==========  " + data);
                players = JArray.Parse(data["players"].ToString());
                JArray sortedJArray = new JArray(players.OrderByDescending(j => j["timeWin"]));
                Debug.Log("  sortedJArray data ==========  " + sortedJArray);
                int indexPlayerEnd = 0;
                if (isSpectator)
                {
                    // code spectator screen here
                    for (int i = 0; i < sortedJArray.Count; i++)
                    {
                        if (sortedJArray[i]["isSpectator"].ToString() == "0")
                        {
                            indexPlayerEnd++;
                            //player.GetComponent<PlayerMovement>().AddPlayerResult(players[i]["playerName"].ToString(), players[i]["playerStatus"].ToString(), i);
                        }

                    }

                }
                else
                {
                    for (int i = 0; i < sortedJArray.Count; i++)
                    {
                        if(sortedJArray[i]["isSpectator"].ToString() == "0")
                        {
                           
                            indexPlayerEnd++;
                            //player.GetComponent<PlayerMovement>().AddPlayerResult(players[i]["playerName"].ToString(), players[i]["playerStatus"].ToString(), i);
                        }

                    }

                }
                
                break;
            case "playerLeaveRoom":
                string playerLeaveId = data["clientId"].ToString();


                for (int i = 0; i < players.Count; i++)
                {
                    //Debug.Log(" players player leave ==   " + players[i].ToString());
                    if (playerLeaveId == players[i]["id"].ToString())
                    {
                        if (player == null)
                        {
                            if (players[i]["isSpectator"].ToString() == "0")
                            {
                                MainMenu.instance.RemovePlayerJoinRoomByAvatar(playerLeaveId);
                            }
                            MainMenu.instance.ShowTotalPlayers(players.Count);
                        }

                        players.RemoveAt(i);
                        Debug.Log(" players playerLeaveRoom 222222222222222  " + playerLeaveId);

                    }
                }
                if (otherPlayers.ContainsKey(data["clientId"].ToString()))
                {
                    Destroy(otherPlayers[data["clientId"].ToString()]);
                }

                // check new host 
                string checkNewHost = data["newHost"].ToString();
                
                if (checkNewHost != "" && checkNewHost == clientId)
                {
                    isHost = true;

                    if (player != null)
                    {
                        Debug.Log(" client is new host -----------    " );
                        LevelManager.instance.CheckHost();
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
            player.GetComponent<AnimationControl>().PlayerStunned(pos);
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
        Debug.Log("  MainMenu.instance.isSpectator OnRequestRoom =================  " + MainMenu.instance.isSpectator);
        string room = MainMenu.instance.roomId;
        JObject jsData = new JObject();
        jsData.Add("meta", "requestRoom");
        jsData.Add("playerLen", 8);
        jsData.Add("room", room);
        jsData.Add("host", MainMenu.instance.isHost);
        jsData.Add("isSpectator", MainMenu.instance.isSpectator);
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
        jsData.Add("meta", "joinLobby");
        jsData.Add("room", ROOM);
        jsData.Add("isHost", MainMenu.instance.isHost);
        jsData.Add("gender", MainMenu.instance.gender);
        jsData.Add("isSpectator", MainMenu.instance.isSpectator);
        jsData.Add("playerName", playerName);
        jsData.Add("userAppId", MainMenu.instance.userAppId);
        jsData.Add("avatar", MainMenu.instance.userAvatar);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnGotoGame()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "gotoGame");
        jsData.Add("room", ROOM);
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
        jsData.Add("meta", "join");
        jsData.Add("room", ROOM);
        jsData.Add("isHost", MainMenu.instance.isHost);
        jsData.Add("playerName", playerName);
        jsData.Add("characterIndex", MainMenu.instance.selectedCharacter);
        jsData.Add("pos", clientPosStart.ToString());
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnStartGame()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "startGame");
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnHitEnemy(Vector3 hitPos, string enemyName)
    {
        if (!LevelManager.instance.isStartGame) return;

        //string enemyId = enemyName.Replace("otherplayer-","");
        //Debug.Log("enemyName ==================  " + enemyName + "    , sub  = " + enemyId);
        if (isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", "hitEnemy");
        jsData.Add("room", ROOM);
        jsData.Add("hitPos", hitPos.ToString());
        jsData.Add("enemyId", enemyName);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnStunnedByEnemy(Vector3 hitPos, string enemyName)
    {
        if (!LevelManager.instance.isStartGame) return;

        //string enemyId = enemyName.Replace("otherplayer-", "");
        //Debug.Log("enemyName ==================  " + enemyName + "    , sub  = " + enemyId);
        if (isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", "stunned");
        jsData.Add("room", ROOM);
        jsData.Add("hitPos", hitPos.ToString());
        jsData.Add("enemyId", enemyName);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnRoundAlready()
    {
        if (!isHost || isEndGame) return;
        Debug.Log(" OnCountDown =================  ");
        JObject jsData = new JObject();
        jsData.Add("meta", "roundAlready");
        jsData.Add("room", ROOM);
        jsData.Add("ready", "1");
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnRoundPass(int round)
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "roundPass");
        jsData.Add("room", ROOM);
        jsData.Add("round", round);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnCountDown()
    {
        if (!isHost || isEndGame) return;
        Debug.Log(" OnCountDown =================  ");
        JObject jsData = new JObject();
        jsData.Add("meta", "countDown");
        jsData.Add("room", ROOM);
        jsData.Add("timer", "");
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnMoving(Vector3 _velocity, float _h, float _v)
    {
        if (isEndGame || !player) return;
        clientPosStart = player.transform.position;
        //Quaternion rot = player.transform.rotation;
        JObject jsData = new JObject();
        jsData.Add("meta", "moving");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        jsData.Add("velocity", _velocity.ToString());
        jsData.Add("h", _h);
        jsData.Add("v", _v);
        jsData.Add("pos", clientPosStart.ToString());
        //jsData.Add("rot", rot.ToString());
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnRequestRandomTarget(int _target, int _ran1, int _ran2, int _ran3)
    {
        if (!isHost || isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", "requestTarget");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        jsData.Add("target", _target);
        jsData.Add("ran1", _ran1);
        jsData.Add("ran2", _ran2);
        jsData.Add("ran3", _ran3);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    
    public void OnCubeFall()
    {
        if (!isHost || isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", "cubeFall");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnCubeReset()
    {
        if (!isHost || isEndGame) return;
        JObject jsData = new JObject();
        jsData.Add("meta", "cubeReset");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnPlayerDie()
    {
        Debug.Log(" ======================== OnPlayerDie() ======================================");
        JObject jsData = new JObject();
        jsData.Add("meta", "playerDie");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnPlayerWin()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "playerWin");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnEndGame()
    {
        if (!isHost) return;
        JObject jsData = new JObject();
        jsData.Add("meta", "endGame");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnLeaveRoom()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "leave");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public async void OnCloseConnectSocket()
    {
        clientId = "";
        ROOM = "";
        isEndGame = true;

        await webSocket.Close();
    }
    public void OnDisconnect()
    {
        clientId = "";
        ROOM = "";
        isEndGame = true;
        if (player)
        {
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
