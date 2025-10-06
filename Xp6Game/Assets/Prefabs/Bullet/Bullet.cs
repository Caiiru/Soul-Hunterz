using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Bullet : MonoBehaviour
{
    Rigidbody _rigidbody;
    Collider _collider;

    public Vector3 Direction;
    public float Speed = 10f;
    public float LifeTime = 1f;

    public int Damage = 10;

    protected bool wasInstancied = false;
    protected virtual void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        if (!_rigidbody)
        {
            Debug.LogError("No rigidbody attached to the bullet");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Initialize()
    {
        wasInstancied = true;
        _rigidbody.linearVelocity = Direction.normalized * Speed;
        Destroy(gameObject, LifeTime);
    }
}
