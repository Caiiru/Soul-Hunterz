using System;
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
    Animator m_Animator;

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

    void Update()
    {
        if (CanFire())
        {
            FireWeapon();
        }

    }

    bool CanFire()
    {
        return Input.GetButton("Fire1") && currentWeapon != null && _canFire;
    }

    public void FireWeapon()
    {
        currentWeapon.Attack();
        EventBus<OnPlayerAttack>.Raise(new OnPlayerAttack());
        if (m_Animator == null) return;
        m_Animator.SetLayerWeight(m_ShootingLayerIndex, 1);
        m_Animator.SetTrigger(m_ShootingAnimID);
        
    }

    void OnEnable()
    {

        // PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
    }

    void OnDisable()
    {
        // PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;   
        EventBus<OnInventoryInputEvent>.Unregister(onInventoryToggleBinding);
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

}
