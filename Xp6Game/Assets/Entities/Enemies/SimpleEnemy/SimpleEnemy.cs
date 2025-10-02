using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SimpleEnemy : Enemy
{
    
    override protected void TakeDamage(int damage)
    {
        base.TakeDamage(damage); 
    }   
}
