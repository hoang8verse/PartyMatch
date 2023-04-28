using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Player Name")]
    [SerializeField] TMPro.TextMeshProUGUI m_playerName;
    private GameObject m_player;
    private bool m_isInitialized = false;

    public void OnInitialize(string name, GameObject player)
    {
        m_playerName.text = name;
        m_player = player;
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
