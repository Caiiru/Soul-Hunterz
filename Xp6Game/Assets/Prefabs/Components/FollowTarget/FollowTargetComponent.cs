using UnityEngine;

[CreateAssetMenu(fileName = "Follow Target Component", menuName = "Components/FollowTarget")]
public class FollowTargetComponent : ComponentSO
{
    [Header("Buffs")]
    [Tooltip("Component range to look for a new enemy")]
    public float m_Radius;
    [Tooltip("Rate to turn towards the enemy")]
    public float m_TurnRate = 5.0f;
    // [Tooltip("Life time in seconds")]
    // [Range(0, 2)]
    // public float m_LifeTime = 0.4f;

    // [Header("Debuff")]
    // [Tooltip("Speed amout to remove from bullet (become more slow)")]
    // [Range(0, 100)]
    // public float m_SpeedPercentage = 20f;

    private Transform m_BulletTransform;
    Collider[] m_EnemiesNearby;

    public LayerMask m_EnemyLayer;

    private Transform m_TargetTransform;
    public override void ComponentUpdate(Bullet bullet)
    {
        m_BulletTransform = bullet.transform;

        //If dont have any target, find a new one
        if (m_TargetTransform == null)
        {
            var cols = Physics.OverlapSphere(m_BulletTransform.position, m_Radius, m_EnemyLayer);

            // if (m_count == 0) return;
            m_EnemiesNearby = cols;
            if (m_EnemiesNearby.Length == 0) return;

            m_TargetTransform = m_EnemiesNearby[0].transform;
            float closest = Vector3.Distance(m_BulletTransform.position, m_TargetTransform.position);


            foreach (var enemy in m_EnemiesNearby)
            {
                float distance = Vector3.Distance(m_BulletTransform.position, enemy.transform.position);
                if (distance < closest)
                {
                    m_TargetTransform = enemy.transform;
                    closest = distance;
                }
            }
        }
        //Go to target 
        Vector3 targetDirection = m_TargetTransform.position - m_BulletTransform.position;
        targetDirection.y = 0;
        Vector3 desiredDirection = targetDirection.normalized;

        bullet.Direction = Vector3.RotateTowards(bullet.Direction, desiredDirection, m_TurnRate * Time.deltaTime, 0);





    }

    public override BulletPayload Execute(BulletPayload payload, Transform firePoint, int slotIndex)
    {
        payload = base.Execute(payload, firePoint, slotIndex);
        payload.UpdatePayload.Add(Instantiate(this));
        // payload.SpeedMultiplier -= m_SpeedPercentage / 100;
        // payload.FlatLifeTime += m_LifeTime;
        return payload;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_BulletTransform.position, m_Radius);
    }

}
