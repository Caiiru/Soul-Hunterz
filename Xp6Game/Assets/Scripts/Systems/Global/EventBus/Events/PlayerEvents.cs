
using UnityEngine;

public class OnInteractEnterEvent : IEvent
{
    public string InteractableName;
    public InteractableType interactableType;
}
public class OnInteractUpdateEvent : IEvent
{
    public string InteractableName;
    public InteractableType interactableType;
}

public class OnInteractLeaveEvent : IEvent
{

}

public class OnDropComponent : IEvent
{
    public bool isFromPlayer;
    public ComponentSO data;
    public Vector3 position;
}

public class OnCollectComponent : IEvent
{
    public ComponentSO data;
}
public class OnCollectSouls : IEvent
{
    public int amount;
}
public class OnUpdateSouls : IEvent
{
    public int amount;
}


public class OnPlayerTakeDamage : IEvent
{
    public int value;
}
public class OnSetPlayerHealthEvent : IEvent
{
    public int maxHealth;
    public int currentHealth;
}
public class OnPlayerAttack : IEvent
{

}

public class OnAmmoChanged : IEvent
{
    public int currentAmmo;
    public int maxAmmo;
}

public class OnUpdatedRechargeTime : IEvent
{
    public float time;
    public float maxTime;
}
public class OnEndedRechargeTime : IEvent
{

}

public class OnPlayerChangeState : IEvent
{
    public PlayerStates newState;
}

public class OnPlayerDash : IEvent
{

}

public class OnPlayerDied : IEvent
{

}


public class OnDisplayMessage : IEvent
{
    public string m_Message;
}
