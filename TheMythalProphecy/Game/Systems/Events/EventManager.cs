using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Systems.Events;

/// <summary>
/// Central event manager implementing publish/subscribe pattern for decoupled communication
/// </summary>
public class EventManager
{
    private readonly Dictionary<Type, List<Delegate>> _listeners = new();

    /// <summary>
    /// Subscribe to an event type with a handler
    /// </summary>
    public void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var eventType = typeof(T);

        if (!_listeners.ContainsKey(eventType))
        {
            _listeners[eventType] = new List<Delegate>();
        }

        _listeners[eventType].Add(handler);
    }

    /// <summary>
    /// Unsubscribe from an event type
    /// </summary>
    public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        var eventType = typeof(T);

        if (_listeners.ContainsKey(eventType))
        {
            _listeners[eventType].Remove(handler);
        }
    }

    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    public void Publish<T>(T gameEvent) where T : GameEvent
    {
        var eventType = typeof(T);

        if (_listeners.ContainsKey(eventType))
        {
            foreach (var listener in _listeners[eventType])
            {
                if (listener is Action<T> action)
                {
                    action.Invoke(gameEvent);
                }
            }
        }
    }

    /// <summary>
    /// Clear all listeners for a specific event type
    /// </summary>
    public void ClearListeners<T>() where T : GameEvent
    {
        var eventType = typeof(T);
        if (_listeners.ContainsKey(eventType))
        {
            _listeners[eventType].Clear();
        }
    }

    /// <summary>
    /// Clear all event listeners
    /// </summary>
    public void ClearAllListeners()
    {
        _listeners.Clear();
    }
}
