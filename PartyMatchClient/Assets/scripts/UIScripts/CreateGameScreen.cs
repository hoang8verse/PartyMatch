using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace UIElements
{
    public class CreateGameScreen : Screen
    {
        [Header("ChoosePlayMode")]
        [SerializeField] Button m_menPlayerButton;
        [SerializeField] Button m_womenPlayerButton;
        [SerializeField] Image m_chooseRingButton;

        [Header("AdjustPlayerNumber")]
        [SerializeField] TMP_InputField m_inputField;
        [SerializeField] Button m_increaseButton;
        [SerializeField] Button m_decreaseButton;

        private TouchScreenKeyboard m_touchScreenKeyboard;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region MainEventButton
        public void OnCreateRoom()
        {
            MainMenu.instance.JoinRoom();
        }
        public void OnExitScreen()
        {
            MainMenu.instance.FailToJoinRoom();
        }
        #endregion

        #region ChoosePlayMode
        public void OnChoosePlayerMode(bool isMen)
        {
            if (isMen)
            {
                MainMenu.instance.gender = "0";
                m_chooseRingButton.gameObject.SetActive(true);
                m_chooseRingButton.transform.position = m_menPlayerButton.transform.position;
            }
            else
            {
                MainMenu.instance.gender = "1";
                m_chooseRingButton.gameObject.SetActive(true);
                m_chooseRingButton.transform.position = m_womenPlayerButton.transform.position;
            }
        }
        #endregion

        #region PlayerAmountInputEvent
        public void OnInputFieldSelected()
        {
            m_touchScreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
        }
        public void OnPlayerAmountChanged()
        {
            // Validate the text only for number
            ValidateNumberInputField();

            // Limit the value of player amount
            LimitValueInputField(m_inputField.text);
        }
        public void OnIncreaseButtonPressed()
        {
            int amount = int.Parse(m_inputField.text) + 1;
            m_inputField.text = amount.ToString();
        }
        public void OnDecreaseButtonPressed()
        {
            int amount = int.Parse(m_inputField.text) - 1;
            m_inputField.text = amount.ToString();
        }

        private void ValidateNumberInputField()
        {
            string pattern = "^[0-9]*$";
            if (!Regex.IsMatch(m_inputField.text, pattern))
            {
                m_inputField.text = Regex.Replace(m_inputField.text, "[^0-9]", "");
            }
        }
        private void LimitValueInputField(string value)
        {
            int amount = int.Parse(value);
            if (amount >= 30)
            {
                m_inputField.text = "30";
                m_increaseButton.interactable = false;
            }
            else
            if (amount <= 0)
            {
                m_inputField.text = "0";
                m_decreaseButton.interactable = false;
            }
            else
            {
                m_increaseButton.interactable = true;
                m_decreaseButton.interactable = true;
            }
        }
        #endregion
    }
}