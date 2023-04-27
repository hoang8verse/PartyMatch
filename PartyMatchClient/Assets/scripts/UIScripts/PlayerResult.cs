using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResult : MonoBehaviour
{
    [Header("Player Avatar")]
    [SerializeField] Image m_avatarPlayer;

    [Header("Player Name")]
    [SerializeField] TextMeshProUGUI m_playerName;

    [Header("Result Symbols")]
    [SerializeField] bool m_isWin;
    [SerializeField] Image m_resultPanel;
    [SerializeField] GameObject m_victoryMark;
    [SerializeField] GameObject m_loseMark;
    [SerializeField] Sprite m_winBackground;
    [SerializeField] Sprite m_loseBackground;

    public bool IsWin => m_isWin;

    // Start is called before the first frame update
    void Start()
    {       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerResult(bool isWin)
    {
        m_isWin = isWin;
        var background = isWin ? m_winBackground : m_loseBackground;
        m_resultPanel.sprite = background;

        m_victoryMark.SetActive(m_isWin);
        m_loseMark.SetActive(!m_isWin);        
    }
    public void SetPlayerName(string playerName)
    {
        m_playerName.text = playerName;
    }
    public void SetPlayerAvatar(Sprite avatar)
    {
        m_avatarPlayer.sprite = avatar;
    }    
}
