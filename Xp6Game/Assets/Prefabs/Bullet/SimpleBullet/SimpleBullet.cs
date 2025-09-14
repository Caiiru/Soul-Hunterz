using UnityEngine;

public class SimpleBullet : Bullet
{

    protected override void Start()
    {
        Direction = transform.forward;
        base.Start();
        
    } 

    // Update is called once per frame
    void Update()
    {
        
    }
}
