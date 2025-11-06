using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class AlwaysOnTop : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (spriteRenderer != null && spriteRenderer.material != null)
            spriteRenderer.material.renderQueue = 5000; // Sempre no topo
    }
}
