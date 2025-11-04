using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTextManager : MonoBehaviour
{
    public int m_PoolSize = 20;
    public GameObject popupTextPrefab;
    public List<GameObject> m_PoopupPool = new List<GameObject>();


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

        for (int i = 0; i < m_PoolSize; i++)
        {
            GameObject popup = Instantiate(popupTextPrefab, _canvasTransform);
            m_PoopupPool.Add(popup);

            m_PoopupPool[i].SetActive(false);
        }
    }

    private GameObject GetPopup()
    {
        foreach (var popup in m_PoopupPool)
        {
            if (!popup.activeInHierarchy)
            {
                return popup;
            }
        }
        GameObject newPopup = Instantiate(popupTextPrefab, _canvasTransform);
        newPopup.SetActive(false);
        m_PoopupPool.Add(newPopup);

        return newPopup;
    }

    public void ShowPopupText(string text, Vector3 position, Color color)
    {
        ShowPopupText(text, position, color, new Vector3(1, 1, 1));
    }


    public void ShowPopupText(string text, Vector3 position, Color color, Vector3 scale)
    {
        GameObject popup = GetPopup();
        popup.SetActive(true);
        popup.transform.position = position;
        popup.transform.localScale = scale;
        var popupText = popup.GetComponent<PopupText>();
        if (popupText != null)
        {
            popupText.SetText(text, color, scale);
        }
    }
}

