using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RechargeTime : MonoBehaviour
{
    private Image m_ReloadImage;

    //Events

    private EventBinding<OnUpdatedRechargeTime> m_OnUpdatedRechargeTime;
    private EventBinding<OnEndedRechargeTime> m_OnEndedRechargeTime;


    void Start()
    {
        if (m_ReloadImage == null)
            m_ReloadImage = GetComponent<Image>();

        if (m_ReloadImage == null)
        {
            Debug.LogError("Recharge Time Image not Found");
            return;
        }

        m_ReloadImage.DOFade(0, 0);

        BindEvents();
    }

    void BindEvents()
    {
        m_OnUpdatedRechargeTime = new EventBinding<OnUpdatedRechargeTime>(HandlerRechargeTime);
        EventBus<OnUpdatedRechargeTime>.Register(m_OnUpdatedRechargeTime);

        m_OnEndedRechargeTime = new EventBinding<OnEndedRechargeTime>(HandlerEndedRechargeTime);
        EventBus<OnEndedRechargeTime>.Register(m_OnEndedRechargeTime);
    }

    private void HandlerEndedRechargeTime(OnEndedRechargeTime arg0)
    {
        m_ReloadImage.DOFade(0, 0.1f);
    }


    private void HandlerRechargeTime(OnUpdatedRechargeTime arg0)
    {
        if (m_ReloadImage.color.a == 0)
        {
            m_ReloadImage.DOFade(1, 0.1f);
        }
        m_ReloadImage.fillAmount = arg0.time / arg0.maxTime;
    }


    void OnDisable()
    {
        UnbindEvents();
    }
    void OnDestroy()
    {
        UnbindEvents();
    }
    void UnbindEvents()
    {
        EventBus<OnUpdatedRechargeTime>.Unregister(m_OnUpdatedRechargeTime);
        EventBus<OnEndedRechargeTime>.Unregister(m_OnEndedRechargeTime);
    }
}
