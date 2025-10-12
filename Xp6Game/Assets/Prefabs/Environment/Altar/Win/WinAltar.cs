using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WinAltar : MonoBehaviour, Interactable
{
    [SerializeField]
    TextMeshProUGUI interactText;

    [SerializeField]
    private float interactDistance = 3f;

    [SerializeField]
    private float interactTimeTween = 0.5f;

    void Start()
    {
        interactText.transform.position = new Vector3(interactText.transform.position.x, interactText.transform.position.y - interactDistance, interactText.transform.position.z);
        interactText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }



    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }
        ActivatePopup();

        // GameManager.Instance.ChangeGameState(GameState.MainMenu);

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
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        // Debug.Log("Player Interacted with Win Altar");
        GameManager.Instance.WinGame();
    }
}
