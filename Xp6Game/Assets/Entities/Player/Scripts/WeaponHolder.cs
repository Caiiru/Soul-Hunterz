using System;
using Cysharp.Threading.Tasks;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHolder : MonoBehaviour
{
    [Header("Attack Input")]
    public InputActionReference attackActionReference;
    StarterAssetsInputs m_StarterAssetsInputs;

    [Header("Weapons")]
    public AbstractWeapon currentWeapon;
    public GameObject currentWeaponGO;
    public Transform firePoint; // Point from where the weapon fires

    private bool _IsInventoryOpen = true;

    //Events
    EventBinding<OnGameStart> m_OnGameStartEventBinding;
    EventBinding<OnInventoryInputEvent> onInventoryToggleBinding;

    EventBinding<OnPlayerChangeState> m_OnPlayerChangeStateBinding;

    EventBinding<OnPlayerDied> m_OnPlayerDiedBinding;
    EventBinding<OnGameWin> m_OnGameWinBinding;




    //Animator
    [SerializeField] Animator m_Animator;



    int m_ShootingLayerIndex;
    int m_ShootingAnimID;

    void Start()
    {
        BindObjects();
        BindAnims();
        BindEvents();
        Initialize();



    }


    void BindEvents()
    {
        onInventoryToggleBinding = new EventBinding<OnInventoryInputEvent>(HandleInventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryToggleBinding);

        m_OnPlayerChangeStateBinding = new EventBinding<OnPlayerChangeState>(HandlePlayerChangeState);
        EventBus<OnPlayerChangeState>.Register(m_OnPlayerChangeStateBinding);

        m_OnGameStartEventBinding = new EventBinding<OnGameStart>(HandleGameStart);
        EventBus<OnGameStart>.Register(m_OnGameStartEventBinding);

        m_OnPlayerDiedBinding = new EventBinding<OnPlayerDied>(() =>
        {
            ResetGame();
        });
        EventBus<OnPlayerDied>.Register(m_OnPlayerDiedBinding);

        m_OnGameWinBinding = new EventBinding<OnGameWin>(() =>
        {
            ResetGame();
        });
        EventBus<OnGameWin>.Register(m_OnGameWinBinding);
    }


    void BindObjects()
    {
        m_Animator = GetComponentInChildren<Animator>();

        m_StarterAssetsInputs = StarterAssetsInputs.Instance;

    }

    void BindAnims()
    {
        m_ShootingAnimID = Animator.StringToHash("Shooting");
        m_ShootingLayerIndex = m_Animator.GetLayerIndex("Shooting");
    }

    void Initialize()
    {
        _IsInventoryOpen = false;
    }

    async void Update()
    {
        if (CanFire())
        {
            await FireWeapon();
        }


    }

    bool CanFire()
    {
        // return Input.GetButtonDown("Fire1") && currentWeapon != null && _canFire;

        return m_StarterAssetsInputs.attack && currentWeapon != null && _IsInventoryOpen;
    }

    public async UniTask FireWeapon()
    {
        if (currentWeapon.m_CanAttack)
        {
            if (m_Animator != null)
            {
                m_Animator.SetLayerWeight(m_ShootingLayerIndex, 1);
                m_Animator.SetTrigger(m_ShootingAnimID);
            }
            await UniTask.Delay(1);
            currentWeapon.Attack();
        }
        await UniTask.CompletedTask;
    }



    internal void HoldWeapon(GameObject weapon)
    {
        currentWeaponGO = weapon;
        currentWeapon = weapon.GetComponent<AbstractWeapon>();
        currentWeaponGO.transform.SetParent(firePoint.transform);
        currentWeaponGO.transform.localPosition = Vector3.zero;
        currentWeaponGO.transform.localRotation = Quaternion.identity;

        //When player equip the weapon, update the ui
        EventBus<OnAmmoChanged>.Raise(new OnAmmoChanged
        {
            currentAmmo = currentWeapon.m_CurrentAmmo,
            maxAmmo = currentWeapon.m_maxAmmo
        });

    }
    private void HandleInventoryToggle(OnInventoryInputEvent eventdata)
    {
        _IsInventoryOpen = !eventdata.isOpen;
    }


    private void HandlePlayerChangeState(OnPlayerChangeState arg0)
    {
        if (arg0.newState == PlayerStates.PreCombat || arg0.newState == PlayerStates.Combat)
        {
            m_Animator.SetLayerWeight(m_ShootingLayerIndex, 1);
        }
        if (arg0.newState == PlayerStates.Exploring)
        {
            m_Animator.SetLayerWeight(m_ShootingLayerIndex, 0);
        }

    }
    UniTask ResetGame()
    {
        _IsInventoryOpen = false;
        Destroy(currentWeaponGO);
        currentWeapon = null;
        currentWeaponGO = null;
        return UniTask.CompletedTask;
    }

    void OnDisable()
    {
        // UnbindEvents();
    }

    void UnbindEvents()
    {
        EventBus<OnInventoryInputEvent>.Unregister(onInventoryToggleBinding);
        EventBus<OnPlayerChangeState>.Unregister(m_OnPlayerChangeStateBinding);
        EventBus<OnGameStart>.Unregister(m_OnGameStartEventBinding);
    }

    private void HandleGameStart()
    {
        _IsInventoryOpen = true;
        // currentWeaponGO.SetActive(true);

    }

}
