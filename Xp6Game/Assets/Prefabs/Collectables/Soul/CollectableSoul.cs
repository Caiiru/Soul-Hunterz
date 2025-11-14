using DG.Tweening;
using UnityEngine;

public class CollectableSoul : Collectable
{
    [Header("Soul Settings")]
    [SerializeField] private int soulValue = 1;
    [Header("Animation Settings")] 
    [SerializeField] private float m_AnimDuration = 0.1f;
 


    void Start()
    {
        this.name = soulValue > 1 ? $"Souls ({soulValue})" : "Soul";
    }
    public override void Interact()
    { 
        // base.Interact();
        m_CanInteract = false;

        // Raise Event

        

    }
    void Collect()
    {
        EventBus<OnCollectSouls>.Raise(new OnCollectSouls { amount = soulValue });

        gameObject.SetActive(false);
    }

    override public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.DOMove(other.transform.position, m_AnimDuration).SetEase(Ease.InSine).OnComplete(() =>
            {
                Collect();
            });
            // Interact();
        }
    
    }

    

    public void SetCanInteract(bool val)
    {
        m_CanInteract = val;
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
