using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    public static ImageLoader instance;
    //public string imageUrl;
    public Texture2D textureImageUrl;
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    IEnumerator LoadImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            textureImageUrl = null;
        }
        else
        {
            textureImageUrl = ((DownloadHandlerTexture)request.downloadHandler).texture;
            // use the texture here
            Debug.Log(" textureImageUrl aaaaa  " + textureImageUrl);
        }
    }

    public Texture2D GetTextureByLink(string url)
    {
        StartCoroutine(LoadImage(url));
        Debug.Log(" textureImageUrl ====================  " + textureImageUrl);
        return textureImageUrl;
    }
}
