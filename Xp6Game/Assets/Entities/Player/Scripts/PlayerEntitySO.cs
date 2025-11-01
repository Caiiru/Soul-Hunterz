
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Entity/Player Data")]
public class PlayerEntitySO : EntitySO
{

    public float dashCooldown = 2f;
    public float InvencibilityTime = 1;
}