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

    /// <summary>
    /// Executa a lógica do componente, modificando o payload da bala.
    /// </summary>
    /// <param name="payload">O payload a ser modificado.</param>
    /// <param name="firePoint">O ponto de origem do disparo.</param>
    /// <param name="slotIndex">O índice do slot do componente na arma.</param>
    /// <returns>O payload modificado.</returns>
    public abstract BulletPayload Execute(BulletPayload payload, Transform firePoint, int slotIndex);
    public abstract void ComponentUpdate(Bullet bullet);
}
