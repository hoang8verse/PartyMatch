using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CameraController : Singleton<CameraController>
{
    private Camera m_cam;
    private float m_defaultFieldOfView;
    private Vector3 m_defaultLocalPosition;
    private Quaternion m_defaultLocalRotation;
    // Start is called before the first frame update
    public bool IsInitialized { set; get; } = false;
    void Start()
    {
        m_cam = GetComponent<Camera>();
        m_defaultFieldOfView = m_cam.fieldOfView;
        m_defaultLocalPosition = m_cam.transform.localPosition;
        m_defaultLocalRotation = m_cam.transform.localRotation;
        IsInitialized = true;
    }

    public void OnSetupSpectatorViewMode()
    {
        m_cam.fieldOfView = 90;
        m_cam.transform.localPosition = new Vector3(-34.9f, 24f, -32f);
        m_cam.transform.localEulerAngles = new Vector3(65f, 0, 0);
    }    

    public void OnSetupDefaultViewMode()
    {
        m_cam.fieldOfView = m_defaultFieldOfView;
        m_cam.transform.localPosition = m_defaultLocalPosition;
        m_cam.transform.localRotation = m_defaultLocalRotation;
    }    
    // Update is called once per frame
    void Update()
    {
        
    }
}
