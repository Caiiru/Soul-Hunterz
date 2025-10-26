using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public int interactRadius = 4;
    public bool isDebugging = false;
    StarterAssetsInputs _playerInput;

    Collider[] interactColliders = new Collider[3];


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
            CastInteract();
        }

    }

    void CastInteract()
    {

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, interactRadius, interactColliders, 1 << 10);
        if (hitCount == 0)
            return;

        foreach (var obj in interactColliders)
        {
            if (obj.TryGetComponent<Interactable>(out Interactable comp))
            {
                if (!comp.CanInteract())
                    continue;

                comp.Interact();
                break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name);
        if (other.CompareTag("Interactable"))
        {
            EventBus<OnInteractEnterEvent>.Raise(new OnInteractEnterEvent{InteractableName = other.name});
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
        }
    }

    void OnDrawGizmosSelected()
    {
        //Sphere Gizmos to Interact Radius
        if (!isDebugging) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }


}
