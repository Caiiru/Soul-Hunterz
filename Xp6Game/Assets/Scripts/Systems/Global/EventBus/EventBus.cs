using System.Collections.Generic;


/// <summary>
/// A marker interface for all event types.
/// </summary>
public interface IEvent { }

/// <summary>
/// A static, generic event bus for a specific event type.
/// </summary>
/// <typeparam name="T">The type of event to be handled by this bus, constrained to IEvent.</typeparam>
public static class EventBus<T> where T : IEvent
{
    static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();

    /// <summary>
    /// Registers an event binding to this event bus.
    /// </summary>
    /// <param name="binding">The event binding to register.</param>
    public static void Register(EventBinding<T> binding) => bindings.Add(binding);

    /// <summary>
    /// Unregisters an event binding from this event bus.
    /// </summary>
    /// <param name="binding">The event binding to unregister.</param>
    public static void Unregister(EventBinding<T> binding) => bindings.Remove(binding);

    /// <summary>
    /// Raises an event, notifying all registered listeners.
    /// </summary>
    /// <param name="event">The event data to pass to the listeners.</param>
    public static void Raise(T @event)
    {
        foreach(var binding in bindings)
        {
            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }
}
