using System;
using Cysharp.Threading.Tasks; 
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public AbstractWeapon currentWeapon;
    public GameObject currentWeaponGO;
    public Transform firePoint; // Point from where the weapon fires

    private bool _canFire = true;


    //Events
    EventBinding<OnInventoryInputEvent> onInventoryToggleBinding;

    EventBinding<OnPlayerChangeState> m_OnPlayerChangeStateBinding;


    //Animator
    [SerializeField] Animator m_Animator;

    int m_ShootingLayerIndex;
    int m_ShootingAnimID;

    void Start()
    {
        BindObjects();
        BindAnims();
        BindEvents();

    }


    void BindEvents()
    {
        onInventoryToggleBinding = new EventBinding<OnInventoryInputEvent>(HandleInventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryToggleBinding);

        m_OnPlayerChangeStateBinding = new EventBinding<OnPlayerChangeState>(HandlePlayerChangeState);
        EventBus<OnPlayerChangeState>.Register(m_OnPlayerChangeStateBinding);
    }

    void BindObjects()
    { 
        m_Animator = GetComponentInChildren<Animator>();
    }

    void BindAnims()
    {
        m_ShootingAnimID = Animator.StringToHash("Shooting");
        m_ShootingLayerIndex = m_Animator.GetLayerIndex("Shooting");
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
        return Input.GetButton("Fire1") && currentWeapon != null && _canFire;
    }

    public async UniTask FireWeapon()
    {
        EventBus<OnPlayerAttack>.Raise(new OnPlayerAttack());
        if (m_Animator != null)
        {
            m_Animator.SetLayerWeight(m_ShootingLayerIndex, 1);
            m_Animator.SetTrigger(m_ShootingAnimID);
        }
        await UniTask.Delay(10);

        currentWeapon.Attack();

        await UniTask.CompletedTask;
    }



    internal void HoldWeapon(GameObject weapon)
    {
        currentWeaponGO = weapon;
        currentWeapon = weapon.GetComponent<AbstractWeapon>();
        currentWeaponGO.transform.SetParent(firePoint.transform);
        currentWeaponGO.transform.localPosition = Vector3.zero;
        currentWeaponGO.transform.localRotation = Quaternion.identity;

    }
    private void HandleInventoryToggle(OnInventoryInputEvent eventdata)
    {
        _canFire = !eventdata.isOpen;
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

    void OnDisable()
    {
        UnbindEvents();
    }

    void UnbindEvents()
    {
        EventBus<OnInventoryInputEvent>.Unregister(onInventoryToggleBinding);
        EventBus<OnPlayerChangeState>.Unregister(m_OnPlayerChangeStateBinding);
    }

}
