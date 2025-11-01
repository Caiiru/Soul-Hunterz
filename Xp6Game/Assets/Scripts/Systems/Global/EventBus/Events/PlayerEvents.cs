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


public class OnPlayerTakeDamage : IEvent
{

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

