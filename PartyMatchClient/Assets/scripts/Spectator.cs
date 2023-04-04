using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spectator : MonoBehaviour
{
    float OFFSET_MOVE = 10f;
    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private TextMeshProUGUI GameTimer;
    [SerializeField]
    private GameObject EndGameScreen;
    [SerializeField]
    AudioSource clicked;
    private Transform ResultTransfrom;

    bool isMovingCamera = false;
    bool isRunOnMobile = false;
    // Start is called before the first frame update
    void Start()
    {
        EndGameScreen.SetActive(false);
        GameTimer.text = "";
        CheckDevice();
    }

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) == true || Input.GetKey(KeyCode.A) == true)
        {
            if (camera.transform.position.x > -120f)
                camera.transform.position = new Vector3(camera.transform.position.x - OFFSET_MOVE * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
        }
        if (Input.GetKey(KeyCode.RightArrow) == true || Input.GetKey(KeyCode.D) == true)
        {
            if (camera.transform.position.x < 120f)
                camera.transform.position = new Vector3(camera.transform.position.x + OFFSET_MOVE * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
        }
        if (Input.GetKey(KeyCode.UpArrow) == true || Input.GetKey(KeyCode.W) == true)
        {
            camera.fieldOfView -= OFFSET_MOVE * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow) == true || Input.GetKey(KeyCode.S) == true)
        {
            camera.fieldOfView += OFFSET_MOVE * Time.deltaTime;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            camera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * 6;
        }

        if (isRunOnMobile)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // trying to get the second touch input
                //Debug.Log(" --------------------------------  spectator touch.position.x ------------------------------------  " + touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    isMovingCamera = true;
                    Debug.Log(" ======================== GetTouch mobile  downnnnnn ========== =  ");
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    //Debug.Log(" ================= GetTouch mobile  movingggggggg ===========  ");
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isMovingCamera = false;

                    Debug.Log(" ======================== GetTouch mobile  uppppppppp ===========  ");
                }

                if (isMovingCamera)
                {
                    //Debug.Log(" --------------------------------  spectator move camera------------------------------------  ");
                    if (touch.position.x < 1080 / 2)
                    {
                        if (camera.transform.position.x > -120f)
                            camera.transform.position = new Vector3(camera.transform.position.x - 2 * OFFSET_MOVE * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
                    } 
                    if (touch.position.x > 1080 / 2)
                    {
                        if (camera.transform.position.x < 120f)
                            camera.transform.position = new Vector3(camera.transform.position.x + 2 * OFFSET_MOVE * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
                    }

                    if (touch.position.y < 1920 / 2)
                    {
                        camera.fieldOfView += OFFSET_MOVE * Time.deltaTime;
                    }
                    if (touch.position.y > 1920 / 2)
                    {
                        camera.fieldOfView -= OFFSET_MOVE * Time.deltaTime;
                    }
                }

            }
        }
        //else
        //{
        //    if (Input.GetMouseButtonDown(0)) // 0 : left , 1 : right, 2 : wheel
        //    {
        //        isMovingCamera = true;
        //    }
        //    else if (Input.GetMouseButton(0)) // 0 : left , 1 : right, 2 : wheel
        //    {

        //    }
        //    else if (Input.GetMouseButtonUp(0))
        //    {
        //        isMovingCamera = false;
        //    }
        //    if (isMovingCamera)
        //    {
        //        Debug.Log(" -------------------------------- PC spectator move camera------------------------------------  ");
        //        if (Input.GetAxis("Mouse X") < 1080 / 2)
        //        {
        //            if (camera.transform.position.x > -120f)
        //                camera.transform.position = new Vector3(camera.transform.position.x - 2 * OFFSET_MOVE * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
        //        }
        //        if (Input.GetAxis("Mouse X") > 1080 / 2)
        //        {
        //            if (camera.transform.position.x < 120f)
        //                camera.transform.position = new Vector3(camera.transform.position.x + 2 * OFFSET_MOVE * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
        //        }

        //        if (Input.GetAxis("Mouse Y") < 1920 / 2)
        //        {
        //            camera.fieldOfView += OFFSET_MOVE * Time.deltaTime;
        //        }
        //        if (Input.GetAxis("Mouse Y") > 1920 / 2)
        //        {
        //            camera.fieldOfView -= OFFSET_MOVE * Time.deltaTime;
        //        }
        //    }
        //}

        if (camera.fieldOfView < 50f)
        {
            camera.fieldOfView = 50f;
        } 
        if(camera.fieldOfView > 150f)
        {
            camera.fieldOfView = 150f;
        }
    }

    public void StartGame()
    {

        //ReadyScreen.SetActive(false);
    }
    public void SetTextGameTimer(string timer)
    {
        GameTimer.text = timer;
    }

    public void PlayAgain()
    {

        OnClickVFX();
        SceneManager.LoadScene("MainMenu");
    }
    public void EnableEndGameScreen()
    {
        EndGameScreen.SetActive(true);
        SocketClient.instance.OnCloseConnectSocket();
    }

    public void AddPlayerResult(string playerName, string playerStatus, int index)
    {
        Debug.Log(" index ================   " + index);
        Vector3 pos = new Vector3(0, -100 * index + 200, 0);
        TMPro.TextMeshProUGUI user = ResultTransfrom.gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        string _text = playerName + " is " + playerStatus;

        user.text = _text;
        user.fontSize = 50;
        if (playerStatus == "die")
        {
            user.color = Color.black;
        }
        else
        {
            user.color = Color.green;
        }
        user.gameObject.SetActive(true);
        //user.gameObject.transform.position = pos;
        Debug.Log(" pos ================   " + pos);
        GameObject resultPlayer = Instantiate(user.gameObject, EndGameScreen.transform);
        resultPlayer.transform.localPosition = pos;
        ResultTransfrom.gameObject.SetActive(false);
    }
    public void AddPlayerResult(Texture2D playerAvatar, string playerName, string playerStatus, int index)
    {
        EndGameScreen.GetComponent<UIElements.EndGameScreen>().AddPlayerResult(playerAvatar, playerName, playerStatus, index);
    }
    public void OnClickVFX()
    {
        clicked.Play();
    }

}
