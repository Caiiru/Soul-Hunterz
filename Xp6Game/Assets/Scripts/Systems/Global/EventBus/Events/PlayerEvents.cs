public class OnInteractEnterEvent : IEvent
{
    public string InteractableName;
    public InteractableType interactableType;
}

public class OnInteractLeaveEvent : IEvent
{

}

public class OnDropComponent : IEvent
{
    public ComponentSO data;
}

public class OnCollectComponent : IEvent
{
    public ComponentSO data;
}
public class OnUpdateSouls : IEvent
{
    public int amount;
}
public class OnUpdateSoulsUI : IEvent
{
    public int currentSouls;
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

