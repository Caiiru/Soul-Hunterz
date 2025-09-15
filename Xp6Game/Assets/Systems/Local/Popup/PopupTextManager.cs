using UnityEngine;

public class PopupTextManager : MonoBehaviour
{
    public GameObject popupTextPrefab;
    public GameObject[] popupPool = new GameObject[10];


    [SerializeField]
    private Transform _canvasTransform;

    #region Singleton
    public static PopupTextManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    #endregion
    void Start()
    {
        InitializePool();
        _canvasTransform = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void InitializePool()
    {
        for (int i = 0; i < popupPool.Length; i++)
        {
            GameObject popup = Instantiate(popupTextPrefab, _canvasTransform);
            popupPool[i] = popup;
            popupPool[i].SetActive(false);
        }
    }

    public void ShowPopupText(string text, Vector3 position, Color color)
    {
        foreach (var popup in popupPool)
        {
            if (!popup.activeInHierarchy)
            {
                popup.SetActive(true);
                popup.transform.position = position;
                var popupText = popup.GetComponent<PopupText>();
                if (popupText != null)
                {
                    popupText.SetText(text, color);
                }
                break;
            }
        }
    }
}
