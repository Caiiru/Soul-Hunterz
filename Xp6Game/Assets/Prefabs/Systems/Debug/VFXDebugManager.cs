using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class VFXDebugManager : MonoBehaviour
{
    public bool m_DebugMode;
    public List<KeyCode> InputKeys;

    public delegate void InputPress(int key);
    public static event InputPress OnInputPressed;


    public CinemachineCamera _cam;

    //Events

    EventBinding<OnPlayerDied> m_OnGameOverEventBinding;


    void Start()
    {
        if (m_DebugMode)
        {
            m_OnGameOverEventBinding = new EventBinding<OnPlayerDied>(() =>
            {
                Debug.Log("Game over debug mode");
                var copy = GameObject.FindGameObjectWithTag("Player").gameObject;
                var _instance = Instantiate(copy, Vector3.zero, Quaternion.identity);
                Destroy(copy); 
                _cam.Target.TrackingTarget = _instance.transform;
            });
            EventBus<OnPlayerDied>.Register(m_OnGameOverEventBinding);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int _index = 0;
        foreach (var input in InputKeys)
        {
            if (Input.GetKeyDown(input))
            {
                OnInputPressed?.Invoke(_index);
                Debug.Log($"Invoke {_index}");
            }
            _index++;
        }
    }

    //Singleton
    public static VFXDebugManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
        
    }
}
