using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Follow Target Component", menuName = "Components/FollowTarget")]
public class FollowTargetComponent : ComponentSO
{
    public float m_Radius;

    private Transform m_BulletTransform;
    Collider[] m_EnemiesNearby;

    public LayerMask m_EnemyLayer;
    public override void ComponentUpdate(Bullet bullet)
    {
        m_BulletTransform = bullet.transform;
        // int m_count = Physics.SphereCastNonAlloc(m_BulletTransform.position, m_Radius, bullet.transform.forward, m_EnemiesNearby, 100, m_EnemyLayer);

        // int m_count = Physics.OverlapSphereNonAlloc(m_BulletTransform.position, m_Radius, m_EnemiesNearby, m_EnemyLayer);

        var cols = Physics.OverlapSphere(m_BulletTransform.position, m_Radius, m_EnemyLayer);
        
        // if (m_count == 0) return;
        m_EnemiesNearby = cols;
        if (m_EnemiesNearby.Length == 0) return;

        Debug.Log(m_EnemiesNearby[0].transform.name);

        Debug.Log("Update Follow Target");

        Transform closestEnemy = m_EnemiesNearby[0].transform;
        float closest = Vector3.Distance(m_BulletTransform.position, closestEnemy.position);


        foreach (var enemy in m_EnemiesNearby)
        {
            float distance = Vector3.Distance(m_BulletTransform.position, enemy.transform.position);
            if (distance < closest)
            {
                closestEnemy = enemy.transform;
                closest = distance;
            }
        }

        Vector3 newDirection = closestEnemy.position - m_BulletTransform.position;
        newDirection.y = 0;
        bullet.Direction = newDirection.normalized;
        // Debug.DrawLine(m_BulletTransform.position, closestEnemy.position, Color.green);



    }

    public override BulletPayload Execute(BulletPayload payload, Transform firePoint, int slotIndex)
    {
        payload.UpdatePayload.Add(this);
        return payload;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_BulletTransform.position, m_Radius);
    }

}
