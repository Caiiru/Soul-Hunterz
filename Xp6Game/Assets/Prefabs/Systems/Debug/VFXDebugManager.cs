using System.Collections.Generic;
using UnityEngine;

public class VFXDebugManager : MonoBehaviour
{
    public List<KeyCode> InputKeys;

    public delegate void InputPress(int key);
    public static event InputPress OnInputPressed;
    void Start()
    {

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
