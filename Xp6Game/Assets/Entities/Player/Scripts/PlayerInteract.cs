using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public int interactRadius = 4;
    public bool isDebugging = false;
    StarterAssetsInputs _playerInput;

    [SerializeField] Collider[] interactColliders = new Collider[10];
    [SerializeField] Transform _nearbyInteractable;

    private bool interactIsPressed = false;
    private bool m_HasAnyInteractableNearby = false;

    private const int k_InteractableLayerMask = 1 << 10;
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

        _nearbyInteractable = GetNearbyInteractable();
        if (_nearbyInteractable == null) return;

        if (!m_HasAnyInteractableNearby)
        {
            EventBus<OnInteractEnterEvent>.Raise(new OnInteractEnterEvent
            {
                InteractableName = _nearbyInteractable.name,
                interactableType = _nearbyInteractable.GetComponent<Interactable>().GetInteractableType()
            });
        }
        else
        {
            EventBus<OnInteractUpdateEvent>.Raise(new OnInteractUpdateEvent
            {
                InteractableName = _nearbyInteractable.name,
                interactableType = _nearbyInteractable.GetComponent<Interactable>().GetInteractableType()
            });
        }


    }




    void CastInteract()
    {
        if (interactIsPressed) return;

        interactIsPressed = true;
        if (_nearbyInteractable == null)
        {
            m_HasAnyInteractableNearby = false;
            return;

        }
        _nearbyInteractable.GetComponent<Interactable>().Interact();

        return;

    }

    // void OnTriggerEnter(Collider other)
    // {
    //     // Debug.Log(other.name);
    //     if (other.CompareTag("Interactable"))
    //     {
    //         // Debug.Log("Enter");
    //         EventBus<OnInteractEnterEvent>.Raise(new OnInteractEnterEvent
    //         {
    //             InteractableName = other.name,
    //             interactableType = other.GetComponent<Interactable>().GetInteractableType()
    //         });
    //     }
    // }

    Transform GetNearbyInteractable()
    {
        Transform _nearbyInteractable = null;
        float _nearbyDistance = Mathf.Infinity;

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, interactRadius, interactColliders, k_InteractableLayerMask);
        if (hitCount == 0)
        {
            m_HasAnyInteractableNearby = false;
            EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
            return null;
        }


        foreach (var obj in interactColliders)
        {
            if (obj == null) continue;
            if (Vector3.Distance(obj.transform.position, transform.position) < _nearbyDistance)
            {
                _nearbyDistance = Vector3.Distance(obj.transform.position, transform.position);
                if (obj.TryGetComponent<CollectableSoul>(out CollectableSoul _soul))
                {
                    if (!_soul.CanInteract()) continue;
                    _soul.transform.DOMove(transform.position, 0.5f).OnComplete(() =>
                    {
                        _soul.Interact();
                        _soul.SetCanInteract(false);

                    });
                    return null;
                }


                if (obj.TryGetComponent<Interactable>(out Interactable _comp))
                {
                    if (_comp.CanInteract())
                    {
                        _nearbyInteractable = obj.transform;
                    }
                }
            }

        }

        return _nearbyInteractable;
    }
    void OnDrawGizmosSelected()
    {
        //Sphere Gizmos to Interact Radius
        if (!isDebugging) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }


}
