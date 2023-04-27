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

        public void OnShowResult()
        {
            //for (int i = 0; i < 7; i++)
            //{
            //    GameResultMgr.Instance.OnAddPlayerData((i + 1).ToString(), "haiahai", i % 2 == 0);
            //}
            GameResultMgr.Instance.GameResultData.Sort((p1, p2) => p1.IsWinner.CompareTo(p2.IsWinner));

            foreach (var item in GameResultMgr.Instance.GameResultData)
            {
                OnAddPlayerResult(item.PlayerID, item.PlayerName, item.IsWinner);
            }    
        }    
        // Update is called once per frame
        void Update()
        {

        }

        public void OnAddPlayerResult(string playerId, string playerName, bool isWinner)
        {
            var avatar = MainMenu.instance.SpriteAvatarPlayers[playerId];
            var playerResult = Instantiate(m_playerResultPrefab, m_playerResultList.content).GetComponent<PlayerResult>();
            playerResult.SetPlayerName(playerName);
            playerResult.SetPlayerAvatar(avatar);            
            playerResult.SetPlayerResult(isWinner);
        }
        public void OnExitScreen()
        {
        
        }

        private void OnResetData()
        {
            for (int i = 0; i < m_playerResultList.content.childCount; i++)
            {
                var entry = m_playerResultList.content.GetChild(i).gameObject;                
                Destroy(entry);
            }
        }

        private void OnDestroy()
        {
            OnResetData();
        }

        private void OnDisable()
        {
            OnResetData();
        }
    }
}