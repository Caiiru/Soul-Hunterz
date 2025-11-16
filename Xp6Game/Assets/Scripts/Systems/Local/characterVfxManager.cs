using System.Collections.Generic;
using UnityEngine;

public class CharacterVFX : MonoBehaviour
{
    [System.Serializable]
    public class NamedVFX
    {
        public string key;                 // Nome que a animação vai usar
        public ParticleSystem particle;    // Referência da partícula
    }

    [SerializeField] private List<NamedVFX> vfxList = new List<NamedVFX>();
    private Dictionary<string, ParticleSystem> vfxDict;

    void Awake()
    {
        vfxDict = new Dictionary<string, ParticleSystem>();

        foreach (var entry in vfxList)
        {
            if (!vfxDict.ContainsKey(entry.key) && entry.particle != null)
                vfxDict.Add(entry.key, entry.particle);
        }
    }

    /// <summary>
    /// Método que a ANIMATION chama.
    /// </summary>
    public void PlayVFX(string vfxName)
    {
        if (vfxDict.TryGetValue(vfxName, out ParticleSystem ps))
        {
            ps.gameObject.SetActive(true);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
        else
        {
            Debug.LogWarning($"VFX '{vfxName}' não encontrado no objeto {gameObject.name}");
        }
    }
}
