using DG.Tweening;
using TMPro;
using UnityEngine;

public class WinAltar : MonoBehaviour
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
         if (other.gameObject.CompareTag("Player"))
        {
            interactText.enabled = true;
            interactText.transform.DOMoveY(interactText.transform.position.y + interactDistance, interactTimeTween).SetEase(Ease.InOutSine);
            Debug.Log("Player reached the Win Altar!");
            // GameManager.Instance.ChangeGameState(GameState.MainMenu);
        }
    }
    
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interactText.transform.DOMoveY(interactText.transform.position.y - interactDistance, interactTimeTween).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                interactText.enabled = false;
            });
        }
    }
}
