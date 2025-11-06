public class StartGameEvent : IEvent
{

}

public class MainMenuLoadedEvent : IEvent
{

}

public class ChangeMenuStateEvent : IEvent
{
    public MenuState newState;
}