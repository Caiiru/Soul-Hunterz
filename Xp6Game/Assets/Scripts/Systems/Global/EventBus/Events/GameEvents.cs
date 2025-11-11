using UnityEngine;

public class OnGameReadyToStart : IEvent
{
    
}
public class OnGameStart : IEvent
{

}

public class OnGameWin : IEvent
{

}
public class OnGameOver : IEvent
{

}

public class OnEnemyDied : IEvent
{
}
public class SpawnSoulEvent : IEvent
{
    public Vector3 spawnPosition;
    public int soulAmount;
}   