using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
public class MessageBox : Singleton<MessageBox>
{
    [SerializeField] TextMeshProUGUI m_messageText;
    [SerializeField] GameObject m_panel;
    [SerializeField] GameObject m_messageBox;
    [SerializeField] GameObject m_loadingBar;
    [SerializeField] List<Image> m_loadingItems;

    private bool m_isShowLoading = false;
    private int m_currentSpriteIndex;
    private float m_timer;
    private float m_delayTime = 0.15f;

    private void Start()
    {
        OnHide();
    }
    public void OnShowPopup(string text = "")
    {
        m_panel.SetActive(true);
        m_messageBox.SetActive(true);
        m_loadingBar.SetActive(false);
        m_isShowLoading = false;

        if (!string.IsNullOrEmpty(text))
            m_messageText.text = text;
    }   

    public void OnShowLoadingScreen()
    {
        m_panel.SetActive(true);
        m_messageBox.SetActive(false);
        m_loadingBar.SetActive(true);
        m_isShowLoading = true;
        m_timer = 0;
        m_currentSpriteIndex = 0;
        OnHideItemLoading();
    }    

    public void OnHide()
    {
        m_panel.SetActive(false);
        m_isShowLoading = false;
    }    
    
    public void OnClick()
    {
        OnHide();
    }

    void OnHideItemLoading()
    {
        for (int i = 0; i < m_loadingItems.Count; i++)
        {
            if (i != m_currentSpriteIndex)
                m_loadingItems[i].color = Color.gray;
        }
    }    
    private void Update()
    {
        if(m_isShowLoading)
        {
            m_timer += Time.deltaTime;

            if (m_timer >= m_delayTime)
            {
                m_currentSpriteIndex = (m_currentSpriteIndex + 1) % m_loadingItems.Count;
                m_loadingItems[m_currentSpriteIndex].color = Color.white;
                OnHideItemLoading();
                m_timer = 0f;
            }
        }    
    }

}
