using UnityEngine;

public abstract class S_Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;


    void Start()
    {
        Destroy(gameObject, lifeTime);
        this.Activate();
    }
    public virtual void Activate()
    {
        Debug.Log("Bullet Activated");
    }
}
