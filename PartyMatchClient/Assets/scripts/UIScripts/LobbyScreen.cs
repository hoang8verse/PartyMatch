using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ZXing;
using ZXing.QrCode;
using System.Collections;

namespace UIElements
{
    public class LobbyScreen : MonoBehaviour
    {
        [Header("Player List")]
        [SerializeField] TextMeshProUGUI m_totalPlayerAmountText;
        [SerializeField] RectTransform m_playerAvatarsHolder;
        [SerializeField] GameObject m_playerAvatarPrefab;
        [SerializeField] float m_defaultAvatarSize;
        [SerializeField] int m_avatarsCountLimit;

        [Header("Room Information")]
        [SerializeField] RawImage m_qrImage;
        [SerializeField] TextMeshProUGUI m_roomID;
        [SerializeField] TextMeshProUGUI m_playerJoinRoomNotification;


        float fadeTime = 1f; // Set the time it takes to fade in and out                
        float defaultHolderSize = 800f;
        private List<GameObject> avatarsLists = new List<GameObject>();
        private Dictionary<string, int> m_playerAvatarsDict = new Dictionary<string, int>();

        public string RoomID => m_roomID.text;
        public RawImage QRImage => m_qrImage;
        
        // Start is called before the first frame update
        void Start()
        {   
            m_roomID.text = MainMenu.instance.roomId;

            string qrCoreGen = MainMenu.deepLinkZaloApp + "?roomId="+ MainMenu.instance.roomId;
            m_qrImage.texture = GetQRCodeTexture(qrCoreGen, 256, 256);
            SetTotalPlayer("");
            SetPlayerJoin("");
        }
        public void ShowPlayerJoinRoom(string _playerName)
        {
            SetPlayerJoin(_playerName);
            StartCoroutine(FadeIn());
        }
        void SetPlayerJoin(string _playerName)
        {
            if (_playerName == "") return;
            string text = _playerName.ToString() + " đã tham gia";
            m_playerJoinRoomNotification.text = text;
            //Debug.Log("m_playerJoinRoom.text==============  " + m_playerJoinRoom.text);
        }
        public void SetTotalPlayer(string _totalPlayer)
        {
            if (_totalPlayer == "")
                return;

            string text = "Tổng số người đã tham gia: " + _totalPlayer.ToString();
            m_totalPlayerAmountText.text = text;

            if (int.Parse(_totalPlayer) > m_avatarsCountLimit)
            {
                float sizeAfterIncrease = (m_playerAvatarsHolder.childCount - m_avatarsCountLimit) * m_defaultAvatarSize + m_playerAvatarsHolder.sizeDelta.x;
                m_playerAvatarsHolder.sizeDelta = new Vector2(sizeAfterIncrease, m_playerAvatarsHolder.sizeDelta.y);

                m_avatarsCountLimit = m_playerAvatarsHolder.childCount;
            }
        }

        IEnumerator FadeIn()
        {
            m_playerJoinRoomNotification.gameObject.SetActive(true);
            while (m_playerJoinRoomNotification.alpha < 1)
            {
                m_playerJoinRoomNotification.alpha += Time.deltaTime / fadeTime;
                yield return null;
            }
            yield return new WaitForSeconds(3f); // Wait for 2 seconds before fading out
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            while (m_playerJoinRoomNotification.alpha > 0)
            {
                m_playerJoinRoomNotification.alpha -= Time.deltaTime / fadeTime;
                yield return null;
            }
            m_playerJoinRoomNotification.alpha = 1f;
            m_playerJoinRoomNotification.gameObject.SetActive(false);
        }

        public void SetAvatarForPlayer(Texture2D avatarImage, string playerId)
        {
            var avatar = Instantiate(m_playerAvatarPrefab, m_playerAvatarsHolder);
            avatar.GetComponent<PlayerAvatar>().SetAvatarImage(avatarImage);

            avatarsLists.Add(avatar);
            m_playerAvatarsDict[playerId] = avatarsLists.IndexOf(avatar);
        }
        public void RemoveAvatarForPlayer(string playerId)
        {
            int index = m_playerAvatarsDict[playerId];

            if (index < avatarsLists.Count)
            {
                Destroy(avatarsLists[index]);

                m_playerAvatarsDict.Remove(playerId);
                avatarsLists.RemoveAt(index);
            }        
        }

        public void ResetAvatarList()
        {
            for (int i = 0; i < avatarsLists.Count; i++)
            {
                Destroy(avatarsLists[i]);
            }
            avatarsLists.Clear();
            m_playerAvatarsDict.Clear();
            m_playerAvatarsHolder.sizeDelta = new Vector2(defaultHolderSize, m_playerAvatarsHolder.sizeDelta.y);
        }
        public void SetRoomID(string roomID)
        {
            m_roomID.text = roomID;
        }
        public void OnCopyRoomID()
        {
            
        }
        public void OnStartGame()
        {
            MainMenu.instance.JoinTheGame();
        }
        public void OnExitScreen()
        {
             MainMenu.instance.BackToMainMenu();
        }

        private Texture2D GetQRCodeTexture(string text, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            var result = writer.Write(text);
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels32(result);
            texture.Apply();
            return texture;
        }
    }


}