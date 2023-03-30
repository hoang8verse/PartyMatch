using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static GamManager instance;

    public Material red,green,blue;
    public CubeManager cubemanager;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        
    }
    void setCube()
    {


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
