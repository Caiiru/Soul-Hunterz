using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField]
    Transform _inventoryTransform;
    [SerializeField]
    Transform _interactTransform;


    bool _isHovering = false;
    // Events
    EventBinding<OnInteractEnterEvent> onInteractEnterBinding;
    EventBinding<OnInteractLeaveEvent> onInteractLeaveBinding;
    void OnEnable()
    {
        onInteractEnterBinding = new EventBinding<OnInteractEnterEvent>(OnInteractEnter);
        EventBus<OnInteractEnterEvent>.Register(onInteractEnterBinding);
        onInteractLeaveBinding = new EventBinding<OnInteractLeaveEvent>(OnInteractLeave);
        EventBus<OnInteractLeaveEvent>.Register(onInteractLeaveBinding);

        _interactTransform.gameObject.SetActive(false);
    }
    void OnDisable()
    {
        EventBus<OnInteractLeaveEvent>.Unregister(onInteractLeaveBinding);
        EventBus<OnInteractEnterEvent>.Unregister(onInteractEnterBinding);
    }


    void OnInteractEnter(OnInteractEnterEvent eventData)
    {
        if (_isHovering) return;
        Debug.Log($"Hovering {eventData.InteractableName}");
        _isHovering = true;
        _interactTransform.gameObject.SetActive(true);
        _interactTransform.GetComponentInChildren<TextMeshProUGUI>().text = $"Press F to interact with {eventData.InteractableName}";
        _interactTransform.DOPunchScale(Vector3.one, 0.2f, 2, 0.5f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            // _interactTransform.DOScale(Vector3.one - new Vector3(0.1f, 0.1f, 0.1f), 0.5f).SetEase(Ease.InSine).Flip();
            _interactTransform.transform.localScale = Vector3.one;
        });
    }

    void OnInteractLeave()
    {
        if (!_isHovering) return;
        _interactTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            _isHovering = false;
            _interactTransform.gameObject.SetActive(false);
        });
    }
}
