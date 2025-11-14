using DG.Tweening;
using TMPro; 
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public TextMeshProUGUI _textMeshPro;

    [Space]
    [Header("Move Settings")]
    public float m_MoveLifeTime = 0.3f;
    public float m_MoveUpDistance = 1f;

    public float m_RandomRangeX = 1f;
    public float m_RandomRangeY = 0.75f;


    [Space]
    [Header("Scale Settings")]
    public float m_ScaleLifeTime = 0.15f;
    public Vector3 m_ScaleMax;



    void Start()
    {
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string text, Color color, Vector3 scale)
    {
        if (_textMeshPro == null) return;

       // _textMeshPro.color = color;
        _textMeshPro.text = text;
        m_ScaleMax = scale;

        Activate();
    }

    void Activate()
    {
        transform.localScale = Vector3.zero;
        m_RandomRangeX = Random.Range(-m_RandomRangeX, m_RandomRangeX);
        m_RandomRangeY = Random.Range(0, m_RandomRangeY);

        // Add some random range to the x position
        transform.position = new Vector3(transform.position.x + m_RandomRangeX, transform.position.y, transform.position.z);

        TweenMoveY();
        TweenScale();
    }

    void TweenMoveY()
    {
        transform.DOMoveY(transform.position.y + m_MoveUpDistance + m_RandomRangeY, m_MoveLifeTime).SetEase(Ease.InBack);
    }
    void TweenScale()
    {
        transform.DOScale(m_ScaleMax, m_ScaleLifeTime).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.0f, () =>
            {
                transform.DOScale(Vector3.zero, m_ScaleLifeTime).SetEase(Ease.InBack).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        });
    }

}
