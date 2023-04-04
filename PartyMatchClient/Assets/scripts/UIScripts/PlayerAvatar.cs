using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAvatar : MonoBehaviour
{
    [SerializeField] RawImage m_avatarImage;
    [SerializeField] string m_playerID;

    public void SetAvatarImage(Texture2D avatar)
    {
        m_avatarImage.texture = avatar;
    }
}
