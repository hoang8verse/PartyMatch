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
        [SerializeField] TextMeshProUGUI m_roomID;
        [SerializeField] RawImage m_qrImage;
        [SerializeField] Texture2D m_avatarDefault;
        [SerializeField] Dictionary<string, int> m_playerAvatarsList;
        [SerializeField] TextMeshProUGUI m_TotalPlayer;
        [SerializeField] TextMeshProUGUI m_playerJoinRoom;
        float fadeTime = 1f; // Set the time it takes to fade in and out
        public List<PlayerAvatar> avatarsLists;
        int[] listAvatarAvailable;

        public string RoomID => m_roomID.text;
        public RawImage QRImage => m_qrImage;
        
        // Start is called before the first frame update
        void Start()
        {
            m_playerAvatarsList = new Dictionary<string, int>();
            listAvatarAvailable = new int[avatarsLists.Count];
            for (int i = 0; i < listAvatarAvailable.Length; i++)
            {
                listAvatarAvailable[i] = 0;
                avatarsLists[i].gameObject.GetComponent<RawImage>().texture = m_avatarDefault;
            }
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
            m_playerJoinRoom.text = text;
            //Debug.Log("m_playerJoinRoom.text==============  " + m_playerJoinRoom.text);
        }
        public void SetTotalPlayer(string _totalPlayer)
        {
            if (_totalPlayer == "") return;
            string text = "Tổng số người đã tham gia: " + _totalPlayer.ToString() + " người";
            m_TotalPlayer.text = text;
            //Debug.Log("_totalPlayer==============  " + m_TotalPlayer.text);
        }

        IEnumerator FadeIn()
        {
            m_playerJoinRoom.gameObject.SetActive(true);
            while (m_playerJoinRoom.alpha < 1)
            {
                m_playerJoinRoom.alpha += Time.deltaTime / fadeTime;
                yield return null;
            }
            yield return new WaitForSeconds(3f); // Wait for 2 seconds before fading out
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            while (m_playerJoinRoom.alpha > 0)
            {
                m_playerJoinRoom.alpha -= Time.deltaTime / fadeTime;
                yield return null;
            }
            m_playerJoinRoom.alpha = 1f;
            m_playerJoinRoom.gameObject.SetActive(false);
        }
        int GetIndexAvatarValid()
        {
            for (int i = 0; i < listAvatarAvailable.Length; i++)
            {
                if( listAvatarAvailable[i] == 0)
                {
                    listAvatarAvailable[i] = 1;
                    return i;
                }
            }
            return -1;
        }

        public void SetAvatarForPlayer(Texture2D avatarImage, string playerId)
        {
            int index = GetIndexAvatarValid();
            if (index == -1) return;
            m_playerAvatarsList[playerId] = index;
            avatarsLists[index].gameObject.GetComponent<RawImage>().texture = avatarImage;
        }
        public void RemoveAvatarForPlayer(string playerId)
        {
            int index = m_playerAvatarsList[playerId];
            listAvatarAvailable[index] = 0;
            avatarsLists[index].gameObject.GetComponent<RawImage>().texture = m_avatarDefault;
        }

        public void ResetAvatarList()
        {
            for (int i = 0; i < avatarsLists.Count; i++)
            {
                listAvatarAvailable[i] = 0;
                avatarsLists[i].gameObject.GetComponent<RawImage>().texture = m_avatarDefault;
            }

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