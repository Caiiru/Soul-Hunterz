using System;
using FMODUnity;
using UnityEngine;

// [CreateAssetMenu(fileName = "Entity Data", menuName = "Entity/Entity Data")]
public abstract class EntitySO : ScriptableObject
{
    [Header("Base Entity Settings")]
    public int m_MaxHealth = 100;
    public bool m_CanBeDamaged = true;

    // public GameObject visualPrefab;

    [Header("Sounds"), Space(1)]
    public EntitySounds[] m_SoundsList;
}

[Serializable]
public struct EntitySounds
{
    [SerializeField] string name;
    public EntitySoundType m_SoundType;
    public EventReference m_SoundReference;

}

public enum EntitySoundType
{
    Walk,
    TakeDamage,
    Die,
    Attack,
    DropItem
}