using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] CinemachineCamera m_CinemachineCamera;
    [SerializeField] Transform m_DrivenCameraTransform;

    //Events
    EventBinding<OnGameStart> m_OnGameStart;
    EventBinding<OnGameOver> m_OnGameOver;
    EventBinding<OnGameWin> m_OnGameWin;

    //

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    //Menu Camera
    Vector3 m_MenuCamStartPosition;
    Quaternion m_MenuCamStartRotation;
    [SerializeField] GameObject m_MenuCam;




    public bool m_isDebug = false;
    void Start()
    {
        if (m_isDebug)
        {
            m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

            m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;
        }


        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

        m_MenuCam = m_DrivenCameraTransform.Find("mainMenuCam").gameObject;
        if (m_MenuCam)
        {
            m_MenuCamStartPosition = m_MenuCam.transform.position;
            m_MenuCamStartRotation = m_MenuCam.transform.rotation;
        }

        BindEvents();


    }

    void BindEvents()
    {
        m_OnGameStart = new EventBinding<OnGameStart>(HandleGameReadyToStart);
        EventBus<OnGameStart>.Register(m_OnGameStart);

        m_OnGameOver = new EventBinding<OnGameOver>(HandleGameOver);
        EventBus<OnGameOver>.Register(m_OnGameOver);

        m_OnGameWin = new EventBinding<OnGameWin>(HandleGameOver);
        EventBus<OnGameWin>.Register(m_OnGameWin);
    }

    private void HandleGameOver()
    {
        for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
        {
            m_DrivenCameraTransform.GetChild(i).GetComponent<CinemachineCamera>().Target.TrackingTarget = null;
        }
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        if (m_MenuCam)
        {
            m_MenuCam.transform.position = m_MenuCamStartPosition;
            m_MenuCam.transform.rotation = m_MenuCamStartRotation;
        }

    }

    private void HandleGameReadyToStart(OnGameStart arg0)
    {
        m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

        m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;

        for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
        {
            // m_DrivenCameraTransform.GetChild(i).GetComponent<CinemachineCamera>().Target.TrackingTarget = m_CinemachineCamera.Target.TrackingTarget;


        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
