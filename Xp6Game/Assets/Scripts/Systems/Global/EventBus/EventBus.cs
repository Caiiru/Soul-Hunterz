using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


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
        var listenersCopy = new List<IEventBinding<T>>(bindings);
        UnityEngine.Debug.Log($"Event '{typeof(T).Name}' raised.");
        foreach (var binding in listenersCopy)
        {

            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }

    public static void Clear()
    {
        bindings.Clear();
    }
}


/// <summary>
/// A utility class for finding types within predefined Unity assemblies.
/// </summary>
public static class PredefinedAssemblyUtil
{
    /// <summary>
    /// Enum representing the predefined Unity assemblies.
    /// </summary>
    enum AssemblyType
    {
        AssemblyCSharp,
        AssemblyCSharpEditor,
        AssemblyCsharpEditorFirstPass,
        AssemblyCsharpFirstPass,

    }
    /// <summary>
    /// Gets the assembly type from the assembly name.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly.</param>s
    /// <returns>The corresponding AssemblyType, or null if not found.</returns>
    static AssemblyType? GetAssemblyType(string assemblyName)
    {
        return assemblyName switch
        {
            "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
            "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
            "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCsharpEditorFirstPass,
            "Assembly-Csharp-FirstPass" => AssemblyType.AssemblyCsharpFirstPass,
            _ => null,
        };


    }

    /// <summary>
    /// Gets all types that implement a specific interface from predefined assemblies.
    /// </summary>
    /// <param name="interfaceType">The interface type to search for.</param>
    /// <returns>A list of types that implement the interface.</returns>
    public static List<Type> GetTypes(Type interfaceType)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
        List<Type> types = new List<Type>();
        for (int i = 0; i < assemblies.Length; i++)
        {
            AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);
            if (assemblyType != null)
            {
                assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
            }
        }
        AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], types, interfaceType);
        AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCsharpEditorFirstPass], types, interfaceType);
        return types;
    }

    /// <summary>
    /// Adds types from a specific assembly to a collection if they implement a given interface.
    /// </summary>
    /// <param name="assembly">The array of types from an assembly.</param>
    /// <param name="types">The collection to add the found types to.</param>
    ///<param name="interfaceType">The interface type to check for implementation.</param>

    static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
    {
        if (assembly == null) return;

        for (int i = 0; i < assembly.Length; i++)
        {
            Type type = assembly[i];
            if (type != interfaceType && interfaceType.IsAssignableFrom(type))
            {
                types.Add(type);
            }

        }
    }

}