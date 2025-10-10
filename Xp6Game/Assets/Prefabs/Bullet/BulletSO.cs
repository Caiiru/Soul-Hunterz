using UnityEngine;

[CreateAssetMenu(fileName = "BulletSO", menuName = "Bullets/BulletSO")]
public class BulletSO : ScriptableObject
{
    public string BulletName;
    public float Speed = 20f;
    public float LifeTime = 2f;
    public int BaseDamage = 10;
    public int CritChance = 0;
    public int CritMultiplier = 2;

    [PreviewSprite]
    public Sprite Icon;

    [Header("VFX")]

    public GameObject hitVFX;
    public GameObject trailVFX;

    public delegate void Execute(GameObject target);
}
