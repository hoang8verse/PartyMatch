using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private void Awake()
    {
        Time.timeScale = 0;
        //Advertisements.Instance.ShowBanner(BannerPosition.TOP, BannerType.Adaptive);
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public List<GameObject>  AI;
    public bool winbool;
    public GameObject winpanel;
    public GameObject Loosepanel;
    public GameObject RigObject;
    public GameObject JoystickControl;
    bool isMoving = false;
    bool isStartGame = false;
    // Start is called before the first frame update
    private void Start()
    {
        RigObject.SetActive(false);
        JoystickControl.transform.position = new Vector3(-5000, -5000, 0);
    }
    public void startGame()
    {
        Time.timeScale = 1;
        isStartGame = true;
        RigObject.SetActive(true);
    }
    public void SetLooseScreen()
    {
        Loosepanel.SetActive(true);
        isStartGame = false;
        RigObject.SetActive(false);
    }
    private void Update()
    {
        if (!isStartGame) return;

        for (int i = AI.Count - 1; i > -1; i--)
        {
            if (AI[i] == null)
            {
                AI.RemoveAt(i);
            }
        }

        if (AI.Count<=0 )
        {
            if(winbool)
            {

            }else
            {
                Debug.Log("win");

                winbool = true;
                winpanel.SetActive(true);
                RigObject.SetActive(false);
            }
          
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Touch began, do something
                isMoving = true;
                JoystickControl.transform.position = new Vector3(touch.position.x, touch.position.y, 0);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // Touch moved, do something else
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Touch ended, do another thing
                isMoving = false;
            }
        }

        if (Input.GetMouseButtonDown(0)) // 0 : left , 1 : right, 2 : wheel
        {
            Debug.Log(" ======================== GetMouseButtonDown ===========  ");
            isMoving = true;
            JoystickControl.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

        }
        else
        if (Input.GetMouseButton(0)) // 0 : left , 1 : right, 2 : wheel
        {
            isMoving = true;
            
            //anim.Play("Walk");            
            //Debug.Log(" ======================== GetMouseButton movingggggggggggggggggggggggggg ===========  ");
        }
        else
        if (Input.GetMouseButtonUp(0))
        {

            isMoving = false;
            Debug.Log(" ======================== GetMouseButtonUp dasdadadadad ===========  ");
        }

        if (isMoving)
        {

            
        }
        else
        {
            JoystickControl.transform.position = new Vector3(-5000, -5000, 0);
        }
    }

    public void restart()
    {
        GameObject level = GameObject.Find("LevelManager");

        Destroy(level);
        //Advertisements.Instance.ShowInterstitial();
        SceneManager.LoadScene("Menu");
    }
}
