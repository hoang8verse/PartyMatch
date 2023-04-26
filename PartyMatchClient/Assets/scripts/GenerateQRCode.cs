using UnityEngine;
using System.Collections;
using ZXing;
using ZXing.QrCode;
using Utility;
public class GenerateQRCode : Singleton<GenerateQRCode>
{
 
    public void OnCreateQRCode(string roomId)
    {
        string qrCoreGen = MainMenu.deepLinkZaloApp + "?roomId=" + roomId;
        Texture2D qrCodeTexture = GenerateQRCodeTexture(qrCoreGen, 256, 256);
        //GetComponent<Renderer>().material.mainTexture = qrCodeTexture;
        GetComponent<UnityEngine.UI.RawImage>().texture = qrCodeTexture;
    }    

    private Texture2D GenerateQRCodeTexture(string text, int width, int height)
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