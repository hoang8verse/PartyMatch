﻿using UnityEngine;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Collections;
using Utility;
public class JavaScriptInjected : Singleton<JavaScriptInjected>
{    
    public bool IsInitialized { set; get; } = false;
    [System.Obsolete]
    void Start()
    {     
        //Hello();

        //HelloString("This is a string.");

        //SendMessageToParent("Hello, parent window!");

        GetParamUrl();
        IsInitialized = true;
    }

    [System.Obsolete]
    public void SendMessageToParent(string message)
    {
        // Send a message to the parent window
        //string message = "Hello, parent window!";
        Application.ExternalEval("window.parent.postMessage('" + message + "', '*')");
    }
    public void ReceivedMessage(string getMessage)
    {
        Debug.Log(" ReceivedMessage --------------------------   " + getMessage);
    }

    //[System.Obsolete]
    public void SendRequestShareRoom()
    {
        Debug.Log(" -----------------------   SendRequestShareRoom --------------------------   ");
        JObject jsData = new JObject();
        jsData.Add("event", "SHARE_LINK");
        jsData.Add("data", MainMenu.instance.roomId);

        SendMessageToParent(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }

    void GetParamUrl()
    {
        // Get the current URL
        string url = Application.absoluteURL;

        if (url == "")
        {
            url = "https://rlgl2.brandgames.vn/?roomId=&userAppId=3368637342326461234&userName=User%20Name&userAvatar=https://h5.zdn.vn/static/images/avatar.png&phoneNumber=&followedOA=0&deepLink=";
            Debug.Log("url: =================================   " + url);
        }
        // Parse the URL parameters
        string[] parts = url.Split('?');

        //MainMenu.instance.deepLinkZaloApp = parts[0];
        if (parts.Length > 1)
        {
            string[] urlParams = parts[1].Split('&');
            foreach (string param in urlParams)
            {
                string[] keyValue = param.Split('=');
                if (keyValue.Length > 1)
                {
                    string key = keyValue[0];
                    string value = keyValue[1];

                    // Do something with the key and value
                    Debug.Log("Parameter: " + key + " = " + value);

                    //baseUrl, roomId, userAppId, userName, userAvatar
                    if(key == "roomId")
                    {
                        if (value != null)
                        {
                            MainMenu.instance.roomId = value;
                        }
                    }
                    if (key == "userAppId")
                    {
                        MainMenu.instance.userAppId = value;
                    }
                    if (key == "userName")
                    {
                        MainMenu.instance.playerName = System.Uri.UnescapeDataString(value);
                    }
                    if (key == "userAvatar")
                    {
                        //https://h5.zdn.vn/static/images/avatar.png
                        MainMenu.instance.userAvatar = value;
                    }
                    if (key == "phoneNumber")
                    {
                        MainMenu.instance.phoneNumber = value;
                    }
                    if (key == "followedOA")
                    {
                        MainMenu.instance.followedOA = value;
                    }

                    if (key == "deepLink")
                    {
                        if (value != "")
                        {
                            MainMenu.deepLinkZaloApp = value;
                        }

                    }
                }
            }
        }
    }

}