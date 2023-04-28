using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject m_playerVisualPrefab;
    public int IndexPlayer { set; get; } = -1;
    private GameObject m_playerVisual;

    public void OnInitialize(GameObject parent, string playerName, bool isSpectatorMode)
    {
        m_playerVisual = Instantiate(m_playerVisualPrefab, parent.transform);
        m_playerVisual.GetComponent<PlayerVisual>().OnInitialize(playerName, parent, isSpectatorMode);
    }
    private void OnDestroy()
    {
        m_playerVisual = null;
    }
}
