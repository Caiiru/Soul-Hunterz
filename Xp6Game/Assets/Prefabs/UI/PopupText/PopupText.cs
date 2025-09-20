using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public TextMeshProUGUI _textMeshPro;

    [Space]
    [Header("Move Settings")]
    public float MoveLifeTime = 0.3f;
    public float MoveUpDistance = 1f;

    public float RandomRangeX = 1f;
    public float RandomRangeY = 0.75f;


    [Space]
    [Header("Scale Settings")]
    public float ScaleLifeTime = 0.15f;



    void Start()
    {
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string text, Color color)
    {
        if (_textMeshPro == null) return;

        _textMeshPro.color = color;
        _textMeshPro.text = text;

        Activate();
    }

    void Activate()
    {
        transform.localScale = Vector3.zero;
        RandomRangeX = Random.Range(-RandomRangeX, RandomRangeX);
        RandomRangeY = Random.Range(0, RandomRangeY);

        // Add some random range to the x position
        transform.position = new Vector3(transform.position.x + RandomRangeX, transform.position.y, transform.position.z);
        
        TweenMoveY();
        TweenScale();
    }

    void TweenMoveY()
    { 
        transform.DOMoveY(transform.position.y + MoveUpDistance + RandomRangeY, MoveLifeTime).SetEase(Ease.InBack);
    }
    void TweenScale()
    {
        transform.DOScale(Vector3.one, ScaleLifeTime).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.0f, () =>
            {
                transform.DOScale(Vector3.zero, ScaleLifeTime).SetEase(Ease.InBack).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        });
    }

}
