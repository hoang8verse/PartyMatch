using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class EndGameScreen : Screen
    {
        [SerializeField] ScrollRect m_playerResultList;
        [SerializeField] GameObject m_playerResultPrefab;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddPlayerResult(Texture2D playerAvatar, string playerName, string playerStatus, int index)
        {
            var playerResult = Instantiate(m_playerResultPrefab, m_playerResultList.content).GetComponent<PlayerResult>();
            playerResult.SetPlayerName(playerName);
            playerResult.SetPlayerAvatar(playerAvatar);            
            playerResult.SetPlayerResult((playerStatus == "die") ? false : true);
        }
        public void OnExitScreen()
        {
        
        }
    }
}