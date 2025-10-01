using UnityEngine;

public class SimpleComponentModifier : MonoBehaviour, IComponent
{
    public string ModifierName => throw new System.NotImplementedException();

    public string Description => throw new System.NotImplementedException();

    public int Rarity => throw new System.NotImplementedException();

    public Sprite Icon => throw new System.NotImplementedException();

    public void ApplyModifier(GameObject target)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
