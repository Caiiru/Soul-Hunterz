using System;
using UnityEngine.Events;


/// <summary>
/// Internal interface defining the contract for an event binding.
/// </summary>
/// <typeparam name="T">The type of event this binding is for, constrained to IEvent.</typeparam>
public interface IEventBinding<T>
{
    /// <summary>
    /// Action that is invoked when the event is fired, passing the event data.
    /// </summary>
    public UnityAction<T> OnEvent { get; set; }
    /// <summary>
    /// Action that is invoked when the event is fired, without passing any event data.
    /// </summary>
    public UnityAction OnEventNoArgs { get; set; }
}

/// <summary>
/// A class that manages listeners for a specific event type.
/// </summary>
/// <typeparam name="T">The type of event this binding manages, constrained to IEvent.</typeparam>
public class EventBinding<T> : IEventBinding<T> where T : IEvent
{
    public UnityAction<T> OnEvent { get; set; } = _ => { };
    public UnityAction OnEventNoArgs { get; set; } = () => { };

    UnityAction<T> IEventBinding<T>.OnEvent
    {
        get => OnEvent;
        set => OnEvent = value;
    }

    UnityAction IEventBinding<T>.OnEventNoArgs
    {
        get => OnEventNoArgs;
        set => OnEventNoArgs = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBinding{T}"/> class with a listener that accepts event data.
    /// </summary>
    /// <param name="onEvent">The action to execute when the event is fired.</param>
    public EventBinding(UnityAction<T> onEvent) => this.OnEvent = onEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBinding{T}"/> class with a listener that does not accept event data.
    /// </summary>
    /// <param name="onEventNoArgs">The action to execute when the event is fired.</param>
    public EventBinding(UnityAction onEventNoArgs) => this.OnEventNoArgs = onEventNoArgs;

    /// <summary>
    /// Adds a listener that does not accept event data.
    /// </summary>
    /// <param name="onEvent">The action to add.</param>
    public void Add(UnityAction onEvent) => OnEventNoArgs += onEvent;
    /// <summary>
    /// Removes a listener that does not accept event data.
    /// </summary>
    /// <param name="onEvent">The action to remove.</param>
    public void Remove(UnityAction onEvent) => OnEventNoArgs -= onEvent;
    /// <summary>
    /// Adds a listener that accepts event data.
    /// </summary>
    /// <param name="onEvent">The action to add.</param>
    public void Add(UnityAction<T> onEvent) => this.OnEvent += onEvent;
    /// <summary>
    /// Removes a listener that accepts event data.
    /// </summary>
    /// <param name="onEvent">The action to remove.</param>
    public void Remove(UnityAction<T> onEvent) => this.OnEvent -= onEvent;
}