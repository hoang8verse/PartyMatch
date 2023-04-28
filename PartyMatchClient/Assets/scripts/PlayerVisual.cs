using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Player Name")]
    [SerializeField] TMPro.TextMeshProUGUI m_playerName;
    private GameObject m_player;
    private bool m_isInitialized = false;
    private Vector3 m_defaultPosition = new Vector3(0f, 2.182f, -0.436f);
    private Vector3 m_spectatortPosition = new Vector3(0f, 0.85f, -2.2f);
    private bool m_isSpectatorMode = false;
    public void OnInitialize(string name, GameObject player, bool isSpectatorMode)
    {
        m_playerName.text = name;
        m_player = player;
        m_isSpectatorMode = isSpectatorMode;
        m_playerName.transform.localPosition = isSpectatorMode ? m_spectatortPosition : m_defaultPosition;

        m_isInitialized = true;
    }    
    private void LateUpdate()
    {
        if (m_isInitialized)
        {
            var cameraRotation = Camera.main.transform.rotation;
            transform.rotation = cameraRotation;
            if (m_player != null)
            {
                var playerPos = m_player.transform.position;
                transform.position = new Vector3(playerPos.x, transform.position.y, playerPos.z);
            }
        }
    }

    private void OnDestroy()
    {
        m_isInitialized = false;
        m_player = null;
        m_playerName = null;
    }
}
