using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WinAltar : MonoBehaviour, Interactable
{

    public int m_RequiredSouls = 10;
    public int m_CurrentSouls = 0;
    public int m_SoulsPerInteraction = 5;

    [Header("Text Settigs")]
    [SerializeField]
    TextMeshProUGUI m_soulsText;

    [SerializeField]
    private float m_DistanceOffsetY = 1f;

    [SerializeField]
    private float interactTimeTween = 0.5f;

    private bool _canInteract = true;

    PlayerInventory m_playerInventory;

    void Start()
    {
        transform.name = "???";
        _canInteract = true;
        if (m_soulsText == null) return;

        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        m_soulsText.enabled = false;
    }

    void OnTriggerExit(Collider collision)
    {

        if (collision.CompareTag("Player"))
        {
            DesactivatePopup();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivatePopup();
            if (m_playerInventory == null)
                m_playerInventory = other.GetComponent<PlayerInventory>();
        }
    }

    void ActivatePopup()
    {

        m_soulsText.enabled = true;
        m_soulsText.alpha = 1f;
        m_soulsText.transform.DOMoveY(m_soulsText.transform.position.y + m_DistanceOffsetY, interactTimeTween).SetEase(Ease.InOutSine);

        // m_soulsText.text = $"Press {StarterAssetsInputs.Instance.GetInteractAction().action.GetBindingDisplayString(0)} to interact";
        if (!CanInteract())
        {
            m_soulsText.alpha = 0.5f;
        }
    }

    void DesactivatePopup()
    {
        m_soulsText.transform.DOMoveY(m_soulsText.transform.position.y - m_DistanceOffsetY, interactTimeTween).SetEase(Ease.InOutSine);
        m_soulsText.DOFade(0f, interactTimeTween).OnComplete(() => m_soulsText.enabled = false);
    }
    public bool CanInteract()
    {
        return _canInteract;
    }

    public void Interact()
    {
        // Debug.Log("Player Interacted with Win Altar");
        // _canInteract = false;
        if (m_playerInventory.GetCurrency() >= m_SoulsPerInteraction)
            m_playerInventory.RemoveCurrency(m_SoulsPerInteraction);
        else
            return;
        AddSouls(m_SoulsPerInteraction);
        // GameManager.Instance.WinGame();
    }


    void AddSouls(int amount)
    {
        m_CurrentSouls += amount;
        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        if (m_CurrentSouls >= m_RequiredSouls)
        {

            _canInteract = false;
            EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
            Debug.Log("Win");
        }
    }
    public InteractableType GetInteractableType()
    {
        return InteractableType.Interactable;
    }
}
