using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UIElements;

public class MainMenu : MonoBehaviour
{
	public static MainMenu instance;

	// rlgl
	[SerializeField]
	private GameObject homeScreen;
	[SerializeField]
	private GameObject createRoomScreen;
	[SerializeField]
	private GameObject joinRoomScreen;
	[SerializeField]
	private GameObject lobbyScreen;
	[SerializeField]
	private GameObject hostButtonJoinGame;
	[SerializeField]
	private TMPro.TextMeshProUGUI RoomId;

	[SerializeField] TMPro.TextMeshProUGUI m_notificationText;

	[SerializeField]
	AudioSource bg_Music;
	[SerializeField]
	private AudioSource bg_Win;
	[SerializeField]
	private AudioSource bg_Die;
	[SerializeField]
	private AudioSource vfx_click;
	[SerializeField]
	private TMPro.TMP_InputField inputRoomId;

	public static string deepLinkZaloApp = "https://zalo.me/s/543482719351051682/";
	public string userAppId = "";
	public string userAvatar = "https://h5.zdn.vn/static/images/avatar.png";
	public string playerName = "anonymous";
	public string roomId = "";
	public string isHost = "0";
	public string gender = "0";
	public string isSpectator = "0";
	//public Dictionary<string, GameObject> listPlayers;
	public Dictionary<string, Texture2D> listPlayerAvatars;

	//private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
	private const string CHARS = "0123456789";
	public int length = 6;

	TouchScreenKeyboard myKeyboard;
	private TMPro.TextMeshProUGUI currentInput;
	// end

	public GameObject[] characters;
	public int selectedCharacter = 0;
	public GameObject Starts;
	public GameObject Unlock;
	public AudioClip Click;
	public AudioClip StartSound;
	public AudioSource AudioSource;
    private void Awake()
    {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}
    private void Start()
    {
		//SocketClient.instance.OnConnectWebsocket();
		listPlayerAvatars = new Dictionary<string, Texture2D>();
		StartCoroutine(WaitingReceiver());
	}

    public string Generate()
    {
        string result = "";
        System.Random rand = new System.Random();
        while (result.Length < 6)
        {
            result += CHARS[rand.Next(0, CHARS.Length)];
        }
        return result;
        //if (IsUnique(result))
        //{
        //    return result;
        //}
        //else
        //{
        //    return Generate();
        //}
    }
    IEnumerator WaitingReceiver()
    {
        //roomId = "roomid";// test already have room id
        Debug.Log("  SocketClient.IS_FIRST_JOIN =================  " + SocketClient.IS_FIRST_JOIN);
        yield return new WaitForSeconds(0.5f);
        if (roomId != "" && SocketClient.IS_FIRST_JOIN)
        {
            homeScreen.SetActive(false);
            createRoomScreen.SetActive(false);
            joinRoomScreen.SetActive(true);
            lobbyScreen.SetActive(false);

            inputRoomId.text = roomId;
            //JoinRoom();
            gameObject.GetComponent<JoinGameScreen>().SetTextInputRoomId(roomId);
            UserJoinRoom();
        }
        else
        {
            homeScreen.SetActive(true);
            createRoomScreen.SetActive(false);
            joinRoomScreen.SetActive(false);
            lobbyScreen.SetActive(false);
        }

        //bg_Music.Play(0);
    }
    private void Update()
    {
        if (myKeyboard != null && myKeyboard.status == TouchScreenKeyboard.Status.Done)
        {
            currentInput.text = myKeyboard.text;

            if (joinRoomScreen.activeSelf)
            {
                inputRoomId.text = myKeyboard.text;
                roomId = myKeyboard.text;
                Debug.Log("Input roomId: " + roomId);
            }

            myKeyboard = null;

        }
       
    }
    public void PlayerNameChange(TMPro.TextMeshProUGUI inputPlayerName)
    {
        playerName = inputPlayerName.text;
    }
    public void InputRoomId(TMPro.TextMeshProUGUI inputRoomId)
    {
        roomId = inputRoomId.text;
    }
    public void OnSelectedInput(TMPro.TextMeshProUGUI _currentInput)
    {
        currentInput = _currentInput;
        //TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        if (myKeyboard == null)
        {
            myKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }


    }
    public void CheckTheHost()
    {
        if (isHost == "1")
        {
            hostButtonJoinGame.SetActive(true);
        }
        else
        {
            hostButtonJoinGame.SetActive(false);
        }
    }
    public void JoinRoom()
    {
        OnClickVfx();

        if (joinRoomScreen.activeSelf)
        {
            roomId = inputRoomId.text;
        }
        RoomId.text = roomId;

        if (playerName.Length <= 1)
        {
            playerName = "anonymous";
        }

        SocketClient.instance.OnConnectWebsocket();
        //createRoomScreen.SetActive(true);
        //homeScreen.SetActive(false);
        //joinRoomScreen.SetActive(false);
        //CheckTheHost();
    }
    public void ShowPlayerJoinRoom(string playerName)
    {
        lobbyScreen.GetComponent<LobbyScreen>().ShowPlayerJoinRoom(playerName);
    }
    public void ShowTotalPlayers(int player)
    {
        lobbyScreen.GetComponent<LobbyScreen>().SetTotalPlayer(player.ToString());
    }
    public void ShowLobby()
    {
        createRoomScreen.SetActive(false);
        homeScreen.SetActive(false);
        joinRoomScreen.SetActive(false);
        lobbyScreen.SetActive(true);
        CheckTheHost();
    }
    public void JoinTheGame()
    {
        //SceneManager.LoadScene("Game");
        SocketClient.instance.OnGotoGame();
    }
    public void GotoGame()
    {
        OnClickVfx();

        //bg_Music.Stop();
        SceneManager.LoadScene("Game");
    }
    public void HostCreateNewRoom()
    {
        OnClickVfx();

        roomId = Generate();
        //RoomId.text = "Room ID : " +  roomId;
        isHost = "1";
        //JoinRoom();
        createRoomScreen.SetActive(true);
        homeScreen.SetActive(false);
    }
    public void UserJoinRoom()
    {
        OnClickVfx();

        roomId = inputRoomId.text;
        joinRoomScreen.SetActive(true);
        homeScreen.SetActive(false);
        isHost = "0";
    }
    public void SpectatorJoinRoom()
    {
        OnClickVfx();
        Debug.Log(" ===== SpectatorJoinRoom==== ");
        roomId = inputRoomId.text;
        joinRoomScreen.SetActive(true);
        homeScreen.SetActive(false);
        isHost = "0";
        isSpectator = "1";
    }
    public void ShowFailScreen(string message)
    {
        //failMessage.text = message;
        //homeScreen.SetActive(false);
        //joinRoomScreen.SetActive(false);
        //createRoomScreen.SetActive(false);
        //failJoinRoomScreen.SetActive(true);

        //m_notificationText.text = message;
        m_notificationText.text = "Mã phòng " + RoomId.text + " không tồn tại";
        m_notificationText.gameObject.SetActive(true);

    }

    public void FailToJoinRoom()
    {
        OnClickVfx();

        homeScreen.SetActive(true);
        joinRoomScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
    }


    public void AddPlayerJoinRoomByAvatar(Texture2D avatar, string playerID)
    {
        if (avatar != null)
        {
            listPlayerAvatars.Add(playerID, avatar);
            lobbyScreen.GetComponent<LobbyScreen>().SetAvatarForPlayer(avatar, playerID);
        }
    }
    public void ResetAvatarList()
    {
        listPlayerAvatars = new Dictionary<string, Texture2D>();

    }
    public void RemovePlayerJoinRoomByAvatar(string playerID)
    {
        lobbyScreen.GetComponent<LobbyScreen>().RemoveAvatarForPlayer(playerID);
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = roomId;
    }
    public void BackToMainMenu()
    {
        homeScreen.SetActive(true);
        joinRoomScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyScreen.GetComponent<LobbyScreen>().ResetAvatarList();
        SocketClient.instance.OnCloseConnectSocket();
    }

    public void ShareLinkToInvite()
    {
        OnClickVfx();
        JavaScriptInjected.instance.SendRequestShareRoom();
    }

    public void OnClickVfx()
    {
        vfx_click.Play();
    }
    public void NextCharacter()
	{
		characters[selectedCharacter].SetActive(false);
		selectedCharacter = (selectedCharacter + 1) % characters.Length;
		characters[selectedCharacter].SetActive(true);
		//if(selectedCharacter!=0)
  //      {
		//	Starts.SetActive(false);
		//	Unlock.SetActive(true);

		//}else
  //      {
		//	Starts.SetActive(true);
		//	Unlock.SetActive(false);
		//}
		AudioSource.PlayOneShot(Click);
	}

	public void PreviousCharacter()
	{
		AudioSource.PlayOneShot(Click);

		characters[selectedCharacter].SetActive(false);
		selectedCharacter--;
		if (selectedCharacter < 0)
		{
			selectedCharacter += characters.Length;
		}
		characters[selectedCharacter].SetActive(true);
		//if (selectedCharacter != 0)
		//{
		//	Starts.SetActive(false);
		//	Unlock.SetActive(true);

		//}
		//else
		//{
		//	Starts.SetActive(true);
		//	Unlock.SetActive(false);
		//}
	}
	public void Unlocking()
    {

#if UNITY_EDITOR
		AudioSource.PlayOneShot(StartSound);

		//PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
		SceneManager.LoadScene("Game");
#endif
		//SocketClient.instance.OnGotoGame();
	}
		
	public void StartGame()
	{
		SocketClient.instance.OnConnectWebsocket();
		//SocketClient.instance.OnGotoGame();
		AudioSource.PlayOneShot(StartSound);

		//PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
		SceneManager.LoadScene("Game");
	}
}
