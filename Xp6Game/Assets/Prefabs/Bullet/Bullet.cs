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
    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        if (!_rigidbody)
        {
            Debug.LogError("No rigidbody attached to the bullet");
        }
        _rigidbody.linearVelocity = Direction.normalized * Speed;
        Destroy(gameObject, LifeTime);
    }

    // Update is called once per frame
    void Update()
    {

    } 
}
