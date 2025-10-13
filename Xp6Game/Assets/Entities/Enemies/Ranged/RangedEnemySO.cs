using UnityEngine;

[CreateAssetMenu(fileName = "Ranged Enemy Data", menuName = "Entity/Enemies/Ranged/Enemy Data")]
public class RangedEnemySO : EnemySO
{
    public float timeBetweenShots = 1f;
    public float projectileLifeTime = 5f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;


    

}
