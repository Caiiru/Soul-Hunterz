using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    InputAction _fireAction;

    void Start()
    {
        _fireAction = InputSystem.actions.FindAction("Attack"); 
    }

    private void Fire()
    {
        Debug.Log("Fire");
        var bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    }

    // Update is called once per frame
    void Update()
    {
        if (_fireAction.IsPressed())
        {
            Fire();
        }
        
    }
}
