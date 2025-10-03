using UnityEngine;
using UnityEngine.UI;

public class ConcreteComponent : MonoBehaviour
{
    public ComponentSO componentData;

    public string ComponentName;
    public string Description;
    public int Rarity;  
    public Sprite Icon;


    void ReadFromData()
    {
        if (componentData == null)
            return;

        ComponentName = componentData.ComponentName;
        Description = componentData.Description;
        int Rarity = componentData.Rarity;
        Icon = componentData.Icon;
 

    }
    void Start()
    {
        ReadFromData();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
