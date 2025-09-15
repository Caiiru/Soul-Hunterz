using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SimpleEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    override protected void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        Debug.Log("SimpleEnemy took " + damage + " damage.");
    }   
}
