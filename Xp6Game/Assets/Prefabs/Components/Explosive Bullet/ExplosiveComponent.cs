using UnityEngine;

[CreateAssetMenu(fileName = "Explosive Component", menuName = "Components/Explosive")]

public class ExplosiveComponent : ComponentSO
{
    public int m_Damage;
    public int m_ExplosiveRadius;
    public int m_ExplosiveForce;

    public override BulletPayload Execute(BulletPayload payload, Transform firePoint, int slotIndex)
    {
        return base.Execute(payload, firePoint, slotIndex);
    }

    public override void ComponentUpdate(Bullet bullet)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
