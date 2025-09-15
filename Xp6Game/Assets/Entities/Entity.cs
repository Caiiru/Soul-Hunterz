using UnityEngine;

public class Entity : MonoBehaviour
{

    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected int currentHealth = 100;

    [SerializeField]
    public bool canBeDamaged = true;
    protected virtual void Start()
    {
        currentHealth = maxHealth;

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
}
