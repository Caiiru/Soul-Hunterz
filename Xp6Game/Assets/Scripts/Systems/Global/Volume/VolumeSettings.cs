using UnityEngine;

[CreateAssetMenu(fileName = "Volume Settings", menuName = "Settings/Volume Settings")]
public class VolumeSettings : ScriptableObject
{
    public float LerpDuration = 5f;
    [Header("Wave Clear Settings")]
    public float waveClearedLerpDuration = 1;
    public float waveClearedDelay = 0.5f;

}
