using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class MapCollectable : Collectable, Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Visual")]
    public VisualEffect m_VisualEffect;
    void Start()
    {

    }

    void OnEnable()
    {

        Vector3 _endPosition = transform.position;
        _endPosition.y = 0;
        transform.DOMove(_endPosition, 2f).SetEase(Ease.OutBack);

        transform.name = "Map";
    }

    public override async void Interact()
    {

        if (!CanInteract()) return;
        EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());


        var _myInteractVFX = Instantiate(_onInteractPrefabVFX, transform.position, Quaternion.identity);
        _myInteractVFX.transform.localScale = new Vector3(2f, 2f, 2f);
        Destroy(_myInteractVFX, 3f);
        await CollectMap();

    }

    async UniTask CollectMap()
    {
        m_VisualEffect.SetFloat("Item Size", 0);
        EventBus<OnMapCollected>.Raise(new OnMapCollected());
        Destroy(this.gameObject);
    }

}
