using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR"), Conditional("USE_GAMELOG")]
    public void ShowDebugInfo(string info)
    {
        m_debugInfo.text += info;
    }    
}
