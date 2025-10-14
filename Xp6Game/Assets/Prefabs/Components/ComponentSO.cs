using UnityEngine;
using UnityEngine.UI;

// [CreateAssetMenu(fileName = "New Component", menuName = "Components/New Component")]
public abstract class ComponentSO : ScriptableObject
{
    public string ComponentName;
    public string Description;
    public int Rarity;
    
    [PreviewSprite]
    public Sprite Icon;

    public abstract void Execute(GameObject target, Transform firePoint, int slotindex);
}
