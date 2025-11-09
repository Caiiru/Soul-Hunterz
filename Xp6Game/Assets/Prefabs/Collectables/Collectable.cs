using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour, Interactable
{


    [Header("VFX")]
    // public GameObject enterVFX;
    // public GameObject interactVFX;
    [SerializeField] protected GameObject _onEnterVFX;
    [SerializeField] protected GameObject _onInteractPrefabVFX;

    //Player
    public bool isPlayerInRange = false;
    protected bool m_CanInteract = true;

    public Rigidbody m_Rigidbody;
    public Camera _mainCamera;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void BindObjects()
    {
        m_Rigidbody = GetComponentInChildren<Rigidbody>();
        _mainCamera = Camera.main;
        _onEnterVFX.SetActive(false);
    }
    public virtual async void OnTriggerEnter(Collider other)
    {
        // return;
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            await UniTask.CompletedTask;
        }
    }
    public virtual async void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            await UniTask.CompletedTask;
        }
    }

    public bool CanInteract()
    {
        return m_CanInteract;
    }

    public virtual InteractableType GetInteractableType()
    {
        return InteractableType.Collectable;
    }

    public virtual void Interact()
    {
        if (!CanInteract()) return;
        var _myInteractVFX = Instantiate(_onInteractPrefabVFX, transform.position, Quaternion.identity);
        Destroy(_myInteractVFX, 3f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
