using UnityEngine;

public class GameReadyToStartEvent : IEvent
{
    
}
public class GameStartEvent : IEvent
{

}

public class GameWinEvent : IEvent
{

}
public class GameOverEvent : IEvent
{

}

public class EnemyDiedEvent : IEvent
{
}
public class SpawnSoulEvent : IEvent
{
    public Vector3 spawnPosition;
    public int soulAmount;
}   