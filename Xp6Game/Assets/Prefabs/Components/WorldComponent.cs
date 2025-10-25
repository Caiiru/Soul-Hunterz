using DG.Tweening;
using StarterAssets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldComponent : MonoBehaviour, Interactable
{
    [SerializeField]
    TextMeshProUGUI interactText;

    [SerializeField]
    private float interactDistance = 3f;

    [SerializeField]
    private float interactTimeTween = 0.5f;

    private bool _canInteract = true;
    public bool CanInteract()
    {
        return _canInteract;
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        Debug.Log("Interact called");
    }

    void Start()
    {
        interactText.transform.position = new Vector3(interactText.transform.position.x, interactText.transform.position.y - interactDistance, interactText.transform.position.z);
        interactText.enabled = false;
        _canInteract = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }
        ActivatePopup();
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DesactivatePopup();
        }
    }

    void ActivatePopup()
    {

        interactText.enabled = true;
        interactText.alpha = 1f;
        interactText.transform.DOMoveY(interactText.transform.position.y + interactDistance, interactTimeTween).SetEase(Ease.InOutSine);

        interactText.text = $"Press {StarterAssetsInputs.Instance.GetInteractAction().action.GetBindingDisplayString(0)} to interact";
        if (!CanInteract())
        {
            interactText.alpha = 0.5f;
        }
    }
    void DesactivatePopup()
    {
        interactText.transform.DOMoveY(interactText.transform.position.y - interactDistance, interactTimeTween).SetEase(Ease.InOutSine).OnComplete(() =>
             {
                 interactText.enabled = false;
             });
    }
}
