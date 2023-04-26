using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class LevelManager : Singleton<LevelManager>
{
    //public List<GameObject>  AI;
    public bool winbool;
    public TMPro.TextMeshProUGUI textRound;
    public TMPro.TextMeshProUGUI textStartGame;
    public GameObject buttonReadyPlay;
    public GameObject startObject;
    public GameObject winpanel;
    public GameObject Loosepanel;
    public GameObject RigObject;
    public GameObject JoystickControl;
    bool isMoving = false;
    public bool isStartGame = false;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => (CubeManager.Instance != null && CubeManager.Instance.IsInitialized));

        CheckDevice();
        RigObject.SetActive(false);
        JoystickControl.transform.position = new Vector3(-5000, -5000, 0);

        SocketClient.instance.OnJoinRoom();
        //SocketClient.instance.OnJoinLobbyRoom();
        StartCoroutine(CheckAlreadyPlay());
    }
    bool isRunOnMobile;
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
    public IEnumerator CheckAlreadyPlay()
    {
        //SocketClient.instance.OnJoinRoom();
        yield return new WaitForSeconds(0.1f);

        if (SocketClient.instance.m_isHost)
        {
            
            buttonReadyPlay.SetActive(true);
            textStartGame.text = "";

        } 
        else
        {
            buttonReadyPlay.SetActive(false);
            textStartGame.text = "Chờ chủ phòng bắt đầu !!!";
        }
        Debug.Log(" SocketClient.instance.isHost------------------------ " + SocketClient.instance.m_isHost);
    }
    public void CheckHost()
    {
        if (!isStartGame)
        {
            buttonReadyPlay.SetActive(true);
            textStartGame.text = "";
        }

    }
    public void RequestStartGame()
    {
        SocketClient.instance.OnStartGame();
    }
    public void startGame()
    {
        //Time.timeScale = 1;
        isStartGame = true;
        RigObject.SetActive(true);
        startObject.SetActive(false);
        StartCoroutine(CountDownTarget());
    }
    IEnumerator CountDownTarget()
    {
        yield return new WaitForSeconds(3f);
        CubeManager.Instance.SendRequestRandomTarget();
    }
    public void SetLooseScreen()
    {
        Loosepanel.SetActive(true);
        isStartGame = false;
        RigObject.SetActive(false);
        SocketClient.instance.OnPlayerDie();
    }
    private void Update()
    {
        if (!isStartGame) return;

        //for (int i = AI.Count - 1; i > -1; i--)
        //{
        //    if (AI[i] == null)
        //    {
        //        AI.RemoveAt(i);
        //    }
        //}

        //if (AI.Count<=0 )
        //{
        //    if(winbool)
        //    {

        //    }else
        //    {
        //        Debug.Log("win");

        //        winbool = true;
        //        winpanel.SetActive(true);
        //        RigObject.SetActive(false);
        //    }

        //}
        if (isRunOnMobile)
        {

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
        }
        else
        {
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
        }
       

        if (isMoving)
        {

            
        }
        else
        {
            JoystickControl.transform.position = new Vector3(-5000, -5000, 0);
        }
    }

    public void SetPlayerWin()
    {
        Debug.Log("win");

        winbool = true;
        winpanel.SetActive(true);
        RigObject.SetActive(false);
    }
    public void restart()
    {
        //SocketClient.instance.OnCloseConnectSocket();
        GameObject level = GameObject.Find("LevelManager");
        level.SetActive(false);
        //Destroy(level);
        //Advertisements.Instance.ShowInterstitial();
        SceneManager.LoadScene("MainMenu");
    }
}
