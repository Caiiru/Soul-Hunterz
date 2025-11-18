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
    public string enemyID;
    public Vector3 deathPosition;
}
public class SpawnSoulEvent : IEvent
{
    public Vector3 spawnPosition;
    public int soulAmount;
}

public class OnAltarActivated : IEvent
{
    public int m_AltarActivatedIndex;
}

public class WaveStartEvent : IEvent
{
    public int waveIndex;
}
public class WaveEndEvent : IEvent
{
    public int waveIndex;
}
public class OnFinalAltarActivated : IEvent
{

}

