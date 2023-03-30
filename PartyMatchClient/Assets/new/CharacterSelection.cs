using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
	public GameObject[] characters;
	public int selectedCharacter = 0;
	public GameObject Starts;
	public GameObject Unlock;
	public AudioClip Click;
	public AudioClip StartSound;
	public GameObject NoAds;
	public AudioSource AdSource;
    private void Start()
    {

	}
    public void NextCharacter()
	{
		characters[selectedCharacter].SetActive(false);
		selectedCharacter = (selectedCharacter + 1) % characters.Length;
		characters[selectedCharacter].SetActive(true);
		if(selectedCharacter!=0)
        {
			Starts.SetActive(false);
			Unlock.SetActive(true);

		}else
        {
			Starts.SetActive(true);
			Unlock.SetActive(false);
		}
		AdSource.PlayOneShot(Click);
	}

	public void PreviousCharacter()
	{
		AdSource.PlayOneShot(Click);

		characters[selectedCharacter].SetActive(false);
		selectedCharacter--;
		if (selectedCharacter < 0)
		{
			selectedCharacter += characters.Length;
		}
		characters[selectedCharacter].SetActive(true);
		if (selectedCharacter != 0)
		{
			Starts.SetActive(false);
			Unlock.SetActive(true);

		}
		else
		{
			Starts.SetActive(true);
			Unlock.SetActive(false);
		}
	}
	public void Unlocking()
    {
	//	Reward.instance.ShowAd();
		//IronSourceDemoScript.instance.ShowRewardedAd((value) =>
		//{
		//	if (value)
		//	{
		//		AdSource.PlayOneShot(StartSound);

		//		PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
		//		SceneManager.LoadScene("start");
		//	}

		//});

		if(Advertisements.Instance.IsRewardVideoAvailable())
        {
			Advertisements.Instance.ShowRewardedVideo(CompleteMethods);

        }
#if UNITY_EDITOR
		AdSource.PlayOneShot(StartSound);

		PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
		SceneManager.LoadScene("Game");
#endif
	}
	private void CompleteMethods(bool completed, string advertiser)
	{
		Debug.Log("Closed rewarded from: " + advertiser + " -> Completed " + completed);
		if (completed == true)
		{
            //give the reward

            AdSource.PlayOneShot(StartSound);

            PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
            SceneManager.LoadScene("Game");
        }
		else
		{
			//no reward
		}
	}

	
	public void StartGame()
	{
		AdSource.PlayOneShot(StartSound);

		PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
		SceneManager.LoadScene("Game");
	}
}
