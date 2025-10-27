using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]Camera _mainCamera;
    
    [SerializeField]CinemachineCamera _cinemachineCamera;
    void Start()
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
