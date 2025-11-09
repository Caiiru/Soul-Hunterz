using DG.Tweening;
using UnityEngine;

public class CollectableSoul : Collectable
{
    [Header("Soul Settings")]
    [SerializeField] private int soulValue = 1;
    [Header("Animation Settings")]
    [SerializeField] private float m_upOffset = 1f;
    [SerializeField] private float m_AnimDuration = 0.5f;


    void Start()
    {
        this.name = soulValue > 1 ? $"Souls ({soulValue})": "Soul";       
    }
    public override void Interact()
    {
        base.Interact();

        // Raise Event

        EventBus<OnCollectSouls>.Raise(new OnCollectSouls { amount = soulValue });

        // Destroy Soul Object
        transform.DOMoveY(transform.position.y + m_upOffset, m_AnimDuration);
        transform.DOScale(Vector3.zero, m_AnimDuration+0.1f).OnComplete(() => Destroy(gameObject));

    }


    public int GetSoulValue()
    {
        return soulValue;
    }

    public void SetSoulValue(int val)
    {
        soulValue = val;
    }
}
