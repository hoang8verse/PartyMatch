using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
public class GameManager : Singleton<GameManager>
{
    // Start is called before the first frame update
    [SerializeField] TextMeshPro m_debugInfo;
    
    void Start()
    {
       // SocketClient.instance.OnJoinRoom();
    }
    public void ShowDebugInfo(string info)
    {
        m_debugInfo.text += info;
    }    
}
