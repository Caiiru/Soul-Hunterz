using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CollectableItem : MonoBehaviour, Interactable
{

    [Header("Icon")]
    [SerializeField]
    Image _itemIcon;
    [SerializeField] Transform _iconHolder;


    [Header("Offset")]
    [SerializeField] float animDuration = 0.25f;
    [SerializeField] float offsetY = 10f;
    Vector3 _startSize;

    [Header("Data")]
    [SerializeField] ComponentSO componentData;

    Camera _mainCamera;

    void Start()
    {
        if (componentData != null)
            OverrideItem();
        this.name = componentData.ComponentName;
        _iconHolder = this.transform.GetChild(0).transform;
        _startSize = _iconHolder.localScale;
        _mainCamera = Camera.main;
    }

    void Update()
    {
        LookAtCamera();
    }

    void LookAtCamera()
    {
        _iconHolder.LookAt(_mainCamera.transform);
    }

    void OverrideItem()
    {
        _itemIcon.sprite = componentData.Icon;
        _itemIcon.color = new Color(255, 255, 255, 255);

    }

    async void OnTriggerEnter(Collider other)
    {
        // return;
        if (other.CompareTag("Player"))
        {
            await PopupIcon();

        }
    }

    async void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            await DesactivateIcon();
        }
    }

    UniTask PopupIcon()
    {

        //Activate Icon Mesh
        _itemIcon.color = new Color(255, 255, 255, 255);
        _iconHolder.gameObject.SetActive(true);
        //Set to position
        //Do lerp move and scale
        _iconHolder.DOScale(_startSize, animDuration);
        _iconHolder.DOMoveY(_iconHolder.transform.position.y + offsetY, 0.25f);

        return UniTask.CompletedTask;
    }

    async Task<UniTask> DesactivateIcon()
    {

        _iconHolder.DOMove(transform.position, animDuration);
        _iconHolder.DOScale(0, animDuration);

        await UniTask.Delay(10);
        _itemIcon.color = new Color(255, 255, 255, 0);
        return UniTask.CompletedTask;
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {

        Debug.Log($"Interact with {componentData.ComponentName}");
        // Destroy(this.gameObject);

        EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
        DesactiveItem();

    }

    void DesactiveItem()
    {
        transform.DOScale(0, animDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            this.transform.gameObject.SetActive(false);
        });
    }

    public InteractableType GetInteractableType()
    {
        return InteractableType.Collectable;
    }
}
