using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A utility class for managing the lifecycle of the event bus system.
/// It handles initialization of all event buses and clears them when exiting play mode in the editor.
/// </summary>
public static class EventBusUtil
{
    /// <summary>
    /// Gets a read-only list of all types that implement the <see cref="IEvent"/> interface.
    /// </summary>
    public static IReadOnlyList<Type> EventTypes { get; set; }
    /// <summary>
    /// Gets a read-only list of all initialized <see cref="EventBus{T}"/> types.
    /// </summary>
    public static IReadOnlyList<Type> EventBusTypes { get; set; }


#if UNITY_EDITOR
    /// <summary>
    /// Gets the current state of the Unity Editor's play mode.
    /// This is used to detect when the editor is exiting play mode.
    /// </summary>
    public static PlayModeStateChange PlayModeState { get; set; }

    /// <summary>
    /// Initializes the editor-specific functionality for the event bus. 
    /// This method is automatically called once when the Unity editor loads due to the <see cref="InitializeOnLoadMethodAttribute"/>.
    /// It subscribes to the <see cref="EditorApplication.playModeStateChanged"/> event to clear all event buses when exiting play mode,
    /// preventing potential memory leaks and stale references in the editor.
    /// </summary>
    [InitializeOnLoadMethod]
    public static void InitializeEditor()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    
    /// <summary>
    /// Called when the Unity Editor's play mode state changes.
    /// </summary>
    /// <param name="change">The new play mode state.</param>
    private static void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        PlayModeState = change;
        if(change == PlayModeStateChange.ExitingPlayMode)
        {
            ClearAllBuses();
        }
    }

#endif

    /// <summary>
    /// Initializes the event bus system at runtime. 
    /// This method is decorated with <see cref="RuntimeInitializeOnLoadMethodAttribute"/>, causing it to be called
    /// automatically after assemblies are loaded but before the first scene loads (<see cref="RuntimeInitializeLoadType.BeforeSceneLoad"/>).
    /// It discovers all event types using reflection and then creates a corresponding static <see cref="EventBus{T}"/> for each,
    /// preparing the event system for use throughout the application's lifecycle.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        EventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
        EventBusTypes = InitializeAllBuses();
    }

    /// <summary>
    /// Initializes all event buses based on the discovered event types.
    /// </summary>
    /// <returns>A list of all created <see cref="EventBus{T}"/> types.</returns>
    static List<Type> InitializeAllBuses()
    {
        List<Type> eventBusType = new List<Type>();

        var typedef = typeof(EventBus<>);

        foreach (var eventType in EventTypes)
        {
            var busType = typedef.MakeGenericType(eventType);
            eventBusType.Add(busType);
            // Debug.Log($"Initialized EventBus <{eventType.Name}>");
        }
        return eventBusType;
    }

    /// <summary>
    /// Clears all registered bindings from all event buses.
    /// This is typically called when exiting play mode to prevent memory leaks.
    /// </summary>
    public static void ClearAllBuses()
    {
        Debug.Log("Clearing all buses...");
        for (int i = 0; i < EventBusTypes.Count; i++)
        {
            var busType = EventBusTypes[i];
            var clearMethod = busType.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);

        }
    }

}