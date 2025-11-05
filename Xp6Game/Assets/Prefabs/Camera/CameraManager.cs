using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;

    [SerializeField] CinemachineCamera _cinemachineCamera;

    //Events
    EventBinding<GameReadyToStartEvent> m_onGameReadyToStart;


    public bool m_isDebug = false;
    void Start()
    {
        if (m_isDebug)
        {
            _mainCamera = this.GetComponentInChildren<Camera>();
            _cinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

            _cinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;
        }

        BindEvents();


    }

    void BindEvents()
    {
        m_onGameReadyToStart = new EventBinding<GameReadyToStartEvent>(HandleGameReadyToStart);
        EventBus<GameReadyToStartEvent>.Register(m_onGameReadyToStart);
    }

    private void HandleGameReadyToStart(GameReadyToStartEvent arg0)
    {
        _mainCamera = this.GetComponentInChildren<Camera>();
        _cinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

        _cinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
