using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class GameResultMgr : Singleton<GameResultMgr>
{
    public class PlayerResultData
    {        
        public bool IsWinner;
        public string PlayerName;
        public string PlayerID;

        public PlayerResultData(string playerId, string name, bool isWinner)
        {
            IsWinner = isWinner;
            PlayerName = name;
            PlayerID = playerId;
        }
    }    
    // Start is called before the first frame update
    private List<PlayerResultData> m_gameResultData = new List<PlayerResultData>();
    public List<PlayerResultData> GameResultData => m_gameResultData;
    public void OnAddPlayerData(string playerId, string name, bool isWinner)
    {
        PlayerResultData data = new PlayerResultData(playerId, name, isWinner);

        if (isWinner)
            m_gameResultData.Insert(0, data);
        else
            m_gameResultData.Add(data);
    }    
}
