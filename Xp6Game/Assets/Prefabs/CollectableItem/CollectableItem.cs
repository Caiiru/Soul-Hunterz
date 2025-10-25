using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CollectableItem : MonoBehaviour, Interactable
{
    [SerializeField]
    Image _itemIcon;
    Transform _iconTransform;
    [SerializeField]
    float offsetY = 10f;

    [SerializeField] ComponentSO componentData;
    void Start()
    {
        if (componentData != null)
            OverrideItem();
        _iconTransform = _itemIcon.transform;
    }

    void OverrideItem()
    {
        _itemIcon.sprite = componentData.Icon;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _itemIcon.color = new Color(255, 255, 255, 255);

            _iconTransform.SetPositionAndRotation(transform.position, Quaternion.identity);
            _iconTransform.DOMoveY(transform.position.y + offsetY, 0.25f).SetEase(Ease.InOutSine);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _itemIcon.color = new Color(255, 255, 255, 0);
            _iconTransform.position = transform.position;
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {

        Debug.Log($"Interact with {componentData.ComponentName}");
    
    }

}
