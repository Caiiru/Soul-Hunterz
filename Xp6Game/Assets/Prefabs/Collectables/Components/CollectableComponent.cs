using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CollectableComponent : Collectable, Interactable
{

    [Header("Data")]
    [SerializeField] ComponentSO componentData;

    [Header("Icon")]
    [SerializeField]
    Image _itemIcon;
    [SerializeField] Transform _iconHolder;
    [SerializeField] Transform _mesh;

    [Header("Offset")]
    [SerializeField] float animDuration = 0.25f;
    [SerializeField] float offsetY = 10f;
    Vector3 _startSize;




    public async void Start()
    {

        BindObjects();
        await DesactivateIcon();
    }

    void BindObjects()
    {

        if (componentData != null)
            OverrideItem();
        this.name = componentData.ComponentName;
        _iconHolder = this.transform.GetChild(0).transform;
        _startSize = _iconHolder.localScale;
        _mainCamera = Camera.main;

        m_Rigidbody = GetComponentInChildren<Rigidbody>();

        // if (enterVFX)
        // {
        //     _onEnterVFX = Instantiate(enterVFX, this.transform);
        //     _onEnterVFX.SetActive(false);
        // }
        // if (interactVFX)
        // {
        //     _onInteractVFX = Instantiate(interactVFX, this.transform);
        //     _onInteractVFX.SetActive(false);
        // }
        _onEnterVFX.SetActive(false);
        // _onInteractPrefabVFX.SetActive(false);
    }

    void Update()
    {
        // if (transform.gameObject.activeSelf)
        // LookAtCamera();
    }

    void LookAtCamera()
    {
        _iconHolder.LookAt(_mainCamera.transform);
    }

    void OverrideItem()
    {
        _itemIcon.sprite = componentData.Icon;
        _itemIcon.color = new Color(255, 255, 255, 255);

        transform.name = componentData.ComponentName;

    }

    public override async void OnTriggerEnter(Collider other)
    {
        // return;
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            await PopupIcon();

        }
    }

    public override async void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
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

        if (_onEnterVFX)
        {
            _onEnterVFX.SetActive(true);
            _onEnterVFX.GetComponent<ParticleSystem>().Play();
        }

        return UniTask.CompletedTask;
    }

    async Task<UniTask> DesactivateIcon()
    {

        _iconHolder.DOMove(transform.position, animDuration);
        _iconHolder.DOScale(0, animDuration);

        if (_onEnterVFX)
        {
            _onEnterVFX.GetComponent<ParticleSystem>().Stop();
            await UniTask.Delay(5000);
            if (!isPlayerInRange)
                _onEnterVFX.SetActive(false);
        }
        await UniTask.Delay(10);

        if (!isPlayerInRange)
            _itemIcon.color = new Color(255, 255, 255, 0);

        return UniTask.CompletedTask;
    }

    public override void Interact()
    {
        base.Interact();
        //Play VFX before desactivate
        if (_onInteractPrefabVFX)
        {
            var _interactVFX = Instantiate(_onInteractPrefabVFX, transform.position, Quaternion.identity);
            Destroy(_interactVFX, 5f);
        }
        EventBus<OnCollectComponent>.Raise(new OnCollectComponent
        {
            data = componentData,
        });

        EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
        DesactiveItem();

    }

    void DesactiveItem()
    {
        transform.DOScale(0, animDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            _mesh.transform.position = new Vector3(0, 0, 0);
            m_Rigidbody.useGravity = false;
            this.transform.gameObject.SetActive(false);
        });
    }

    public void SpawnOnPosition(Vector3 position, Vector3 force)
    {
        if (m_Rigidbody == null)
        {
            Debug.Log("Rigidbody is null");
            return;
        }
        transform.position = position;
        m_Rigidbody.AddForce(force, ForceMode.Impulse);
        m_Rigidbody.useGravity = true;


        OverrideItem();
    }


    public void SetComponentData(ComponentSO data)
    {
        this.componentData = data;
    }
}
