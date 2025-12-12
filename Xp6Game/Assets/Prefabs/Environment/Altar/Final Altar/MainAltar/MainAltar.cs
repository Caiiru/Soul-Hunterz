using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class MainAltar : MonoBehaviour, Interactable
{
    public bool m_isActivated;

    public int m_altarsActivatedAlready = 0;

    public int m_lastAltarActivation = 0;

    public bool _canInteract = true;




    [Header("VFX")]
    public VisualEffect m_MainAltar;
    public VisualEffect[] m_MiniAltares;


    PlayerInventory m_playerInventory;


    private bool m_isFinalForm = false;


    #region Events
    EventBinding<OnGameStart> m_OnGameStartBinding;
    EventBinding<OnPlayerDied> m_OnPlayerDiedBinding;
    EventBinding<OnGameWin> m_OnGameWinBinding;

    EventBinding<OnWaveClearedEvent> m_OnNewAltarActivated;
    EventBinding<OnAltarActivated> m_OnAltarActivated;



    #endregion
    #region Start & Bind
    void Start()
    {
        BindEvents();

        Initialize();

    }

    void BindEvents()
    {


        m_OnNewAltarActivated = new EventBinding<OnWaveClearedEvent>(HandleNewAltarActivation);
        EventBus<OnWaveClearedEvent>.Register(m_OnNewAltarActivated);

        m_OnAltarActivated = new EventBinding<OnAltarActivated>(HandleAltarActivation);
        EventBus<OnAltarActivated>.Register(m_OnAltarActivated);


    }



    UniTask UnbindEvents()
    {
        EventBus<OnGameStart>.Unregister(m_OnGameStartBinding);
        EventBus<OnPlayerDied>.Unregister(m_OnPlayerDiedBinding);
        EventBus<OnGameWin>.Unregister(m_OnGameWinBinding);
        EventBus<OnWaveClearedEvent>.Unregister(m_OnNewAltarActivated);

        return UniTask.CompletedTask;
    }

    void Initialize()
    {
        transform.name = "Portal";


        foreach (VisualEffect _miniAltar in m_MiniAltares)
        {
            _miniAltar.SetBool("Active", false);
        }

        m_MainAltar.SetBool("Active", false);

        m_isFinalForm = false;

        //Find player inventory
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_playerInventory = player.GetComponent<PlayerInventory>();

        _canInteract = false;
    }

    #endregion 
    #region Altar Activated 

    private void HandleAltarActivation(OnAltarActivated arg0)
    {
        m_lastAltarActivation = arg0.m_AltarActivatedIndex;

    }
    private void HandleNewAltarActivation()
    {
        ActivateMiniAltar(m_lastAltarActivation);
        m_altarsActivatedAlready++;

        if (m_altarsActivatedAlready > 3)
        {
            _canInteract = true;
        }

    }

    void ActivateMiniAltar(int index)
    {
        m_MiniAltares[index].SetBool("Active", true);
    }
    #endregion


    #region Interact
    public bool CanInteract()
    {
        return _canInteract;
    }

    public void Interact()
    {
        m_MainAltar.SetBool("Active", true);
        EventBus<OnFinalAltarActivated>.Raise(new OnFinalAltarActivated());
        _canInteract = false;

        EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
        return;

    }


    void ActivateAltar()
    {
        // m_MainAltar.SetBool("Active", true);
        // ActivateMiniAltar(0);

        EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());

        m_isActivated = false;
        _canInteract = false;


    }
    #endregion

    public InteractableType GetInteractableType()
    {
        return InteractableType.Interactable;
    }

}
