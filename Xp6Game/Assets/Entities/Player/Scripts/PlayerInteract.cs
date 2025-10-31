using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public int interactRadius = 4;
    public bool isDebugging = false;
    StarterAssetsInputs _playerInput;

    [SerializeField] Collider[] interactColliders = new Collider[3];

    private bool interactIsPressed = false;
    void Start()
    {
#if ENABLE_INPUT_SYSTEM 
        _playerInput = GetComponent<StarterAssetsInputs>();

        GetComponent<BoxCollider>().size = new Vector3(interactRadius, interactRadius, interactRadius);


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
        else
        {
            if (interactIsPressed)
                interactIsPressed = false;
        }

    }

    void CastInteract()
    {
        if (interactIsPressed) return;

        interactIsPressed = true;

        interactColliders = new Collider[3];
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
            // Debug.Log("Enter");
            EventBus<OnInteractEnterEvent>.Raise(new OnInteractEnterEvent
            {
                InteractableName = other.name,
                interactableType = other.GetComponent<Interactable>().GetInteractableType()
            });
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            // Debug.Log("Leave");
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
