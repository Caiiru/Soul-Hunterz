using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using Unity.Cinemachine;
using Unity.Cinemachine.Editor;


public class cameraShakeManager : MonoBehaviour
{
    public static cameraShakeManager instance;
    [SerializeField] private float globalShakeForce = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }

    public void CameraShake(CinemachineImpulseSource Source)
    {
        Debug.Log("Camera Shake");
        Source.GenerateImpulseWithForce(globalShakeForce);

    }

}
