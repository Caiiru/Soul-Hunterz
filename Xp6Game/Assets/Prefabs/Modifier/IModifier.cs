using UnityEngine;

public interface IModifier
{
    public string ModifierName { get; }
    public string Description { get; }
    public int Rarity { get; }
    public Sprite Icon { get; }

    void ApplyModifier(GameObject target);
}

[System.Serializable]
public struct ModifierData
{
    public string ModifierName;
    public string Description;
    public int Rarity;
    public Sprite Icon;

    public void ApplyModifier(GameObject target)
    {
        // Implementation of how the modifier affects the target
    }
}