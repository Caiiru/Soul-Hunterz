using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    StarterAssetsInputs _playerInput;

    void Start()
    {
#if ENABLE_INPUT_SYSTEM 
        _playerInput = GetComponent<StarterAssetsInputs>();
#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput.interact)
        {
            Debug.Log("Interact");
        }

    }

    
}
