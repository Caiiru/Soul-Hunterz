using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] CinemachineCamera m_CinemachineCamera;
    [SerializeField] Transform m_DrivenCameraTransform;

    //Events
    EventBinding<OnGameStart> m_OnGameStart;


    public bool m_isDebug = false;
    void Start()
    {
        if (m_isDebug)
        {
            m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

            m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;
        }

        BindEvents();


    }

    void BindEvents()
    {
        m_OnGameStart = new EventBinding<OnGameStart>(HandleGameReadyToStart);
        EventBus<OnGameStart>.Register(m_OnGameStart);
    }

    private void HandleGameReadyToStart(OnGameStart arg0)
    {
        m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

        m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;

        for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
        {
            m_DrivenCameraTransform.GetChild(i).GetComponent<CinemachineCamera>().Target.TrackingTarget = m_CinemachineCamera.Target.TrackingTarget;


        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
