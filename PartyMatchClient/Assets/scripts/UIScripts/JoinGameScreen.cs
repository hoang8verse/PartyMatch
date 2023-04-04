using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class JoinGameScreen : Screen
    {
        [Header("ChoosePlayMode")]
        [SerializeField] Button m_menPlayerButton;
        [SerializeField] Button m_womenPlayerButton;
        [SerializeField] Button m_spectatorButton;
        [SerializeField] Image m_chooseRingButton;

        [SerializeField] TMP_InputField m_inputField;
        [SerializeField] TextMeshProUGUI m_notificationText;        

        [SerializeField]
        private string m_roomIDEntered;
        public string RoomIDEntered => m_roomIDEntered;
        private TouchScreenKeyboard m_touchScreenKeyboard;
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (m_touchScreenKeyboard != null)
                {

                    if (m_touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done ||
                        m_touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled)
                    {
                        Debug.Log(" m_touchScreenKeyboard.text nullllllllllllllllll  ");
                        m_touchScreenKeyboard = null;
                    }
                    else if (m_touchScreenKeyboard.status == TouchScreenKeyboard.Status.Visible)
                    {

                        m_inputField.text = m_touchScreenKeyboard.text;
                        Debug.Log("m_inputField ================  " + m_inputField.text);
                    }
                }
            }
        }

        public void OnShowNotificationMessage(string notificationText = "Mã phòng không tồn tại*")
        {
            m_notificationText.text = notificationText;
            m_notificationText.gameObject.SetActive(true);
        }
        //public void OnEnteredRoomID(TextMeshProUGUI inputRoomId)
        //{
        //    if (Application.isMobilePlatform)
        //        m_inputField.text = inputRoomId.text;
        //}
        public void OnInputFieldSelected(TextMeshProUGUI _currentInput)
        {

            if (Application.isMobilePlatform)
            {
                if (m_touchScreenKeyboard == null)
                {

                    m_touchScreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
                }
            }

        }
        public void SetTextInputRoomId(string _roomId)
        {
            m_inputField.text = _roomId;
        }
        public void OnJoinRoom()
        {
            MainMenu.instance.JoinRoom();
        }
        public void OnUseQRScan()
        {
            
        }

        public void OnChoosePlayerMode(string mode)
        {
            switch (mode)
            {
                case "0":
                    MainMenu.instance.isSpectator = "0";
                    MainMenu.instance.gender = "0";
                    m_chooseRingButton.gameObject.SetActive(true);
                    m_chooseRingButton.transform.position = m_menPlayerButton.transform.position;
                    break;

                case "1":
                    MainMenu.instance.isSpectator = "0";
                    MainMenu.instance.gender = "1";
                    m_chooseRingButton.gameObject.SetActive(true);
                    m_chooseRingButton.transform.position = m_womenPlayerButton.transform.position;
                    break;

                case "2":
                    MainMenu.instance.isSpectator = "1";
                    m_chooseRingButton.gameObject.SetActive(true);
                    m_chooseRingButton.transform.position = m_spectatorButton.transform.position;
                    break;
            }
        }
        public void OnExitScreen()
        {
            MainMenu.instance.FailToJoinRoom();
        }
    }
}