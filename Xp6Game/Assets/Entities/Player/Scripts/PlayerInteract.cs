using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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

    //events

    EventBinding<OnStartAltarActivation> m_OnAltarActivated;
    EventBinding<OnAltarActivated> m_OnAltarEndedActivation;


    //VFX

    public VisualEffect m_PlayerSouls;

    void Start()
    {
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<StarterAssetsInputs>();

        GetComponent<BoxCollider>().size = new Vector3(interactRadius, interactRadius, interactRadius);


#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif



        m_PlayerSouls.enabled = false;


        BindEvents();
    }
    #region Events
    void BindEvents()
    {
        m_OnAltarActivated = new EventBinding<OnStartAltarActivation>(HandleAltarActivated);
        EventBus<OnStartAltarActivation>.Register(m_OnAltarActivated);

        m_OnAltarEndedActivation = new EventBinding<OnAltarActivated>(async (data) =>
        {
            await HandleAltarEndedActivation(data);
        });
        EventBus<OnAltarActivated>.Register(m_OnAltarEndedActivation);


    }

    private async UniTask HandleAltarEndedActivation(OnAltarActivated arg0)
    {
        // m_PlayerSouls.enabled = false;
        await UniTask.Delay(3000);
        // m_PlayerSouls.SetVector3("TargetPoint", Vector3.zero);
    }



    #endregion
    #region Update
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
    #endregion
    #region Altar Activated
    private void HandleAltarActivated(OnStartAltarActivation arg0)
    {
        Animator _animator = GetComponentInChildren<Animator>();
        if (_animator == null) return;

        // Debug.Log("Altar Activated");
        // _animator.SetTrigger("altarEventStarted");

    }

    #endregion




    void CastInteract()
    {
        if (interactIsPressed) return;

        interactIsPressed = true;
        if (_nearbyInteractable == null)
        {
            m_HasAnyInteractableNearby = false;
            return;

        }

        if (_nearbyInteractable.TryGetComponent<WinAltar>(out WinAltar _winAltar))
        {
            if(!_winAltar.CanInteract()) return;
            m_PlayerSouls.enabled = true;
            Vector3 _position = _nearbyInteractable.GetChild(0).transform.position;

            m_PlayerSouls.SetVector3("Target Position", _position);
            _winAltar.Interact();
        }
        else{
            _nearbyInteractable.GetComponent<Interactable>().Interact();
        }



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
