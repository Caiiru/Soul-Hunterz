using UnityEngine;

public class S_SimpleBullet : S_Bullet
{
    private Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();        
    }
    public override void Activate()
    {
        base.Activate();
        _rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }
}
