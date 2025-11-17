using System;
using UnityEngine;
[CreateAssetMenu(fileName = "New DropTable", menuName = "Drop/New DropTable")]
///
/// This data holds the components that enemies can drop
/// 
public class DropTable : ScriptableObject
{
    public EnemySO m_enemy;
    public DropEntry[] m_PossibleDrops;
    [Tooltip("Chance to drop anything")]
    public float m_BaseDropChance;
}

[Serializable]
public struct DropEntry
{
    public ComponentSO m_ComponentToDrop;

    [Tooltip("0 - 100")]
    [Range(0, 100)]
    public float m_DropChance;
}


