using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class load : MonoBehaviour
{
    public string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        //Advertisements.Instance.Initialize();
        //Advertisements.Instance.ShowBanner(BannerPosition.TOP, BannerType.Adaptive);
        Invoke("loadscenes", 4);
    }
    void loadscenes()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
