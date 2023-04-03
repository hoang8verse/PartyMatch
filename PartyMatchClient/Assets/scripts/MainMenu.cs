using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public static MainMenu instance;

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
