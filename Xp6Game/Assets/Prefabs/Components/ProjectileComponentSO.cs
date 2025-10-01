using UnityEngine;

[CreateAssetMenu(fileName = "new projectile", menuName = "Components/New Projectile")]
public class ProjectileComponentSO : ComponentSO
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float Damage;
    public float Speed;

    public override void Execute(GameObject weapon)
    {
        GameObject newProjectile = Instantiate(projectilePrefab);
        
    }
}
