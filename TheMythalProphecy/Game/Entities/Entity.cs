using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheMythalProphecy.Game.Entities.Components;

namespace TheMythalProphecy.Game.Entities;

/// <summary>
/// Base class for all game entities (characters, NPCs, enemies)
/// Uses component-based architecture for flexibility
/// </summary>
public class Entity
{
    private readonly List<IComponent> _components = new();
    private readonly Dictionary<Type, IComponent> _componentCache = new();

    public string Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; } = true;

    public Entity(string id, string name = "")
    {
        Id = id;
        Name = string.IsNullOrEmpty(name) ? id : name;
    }

    /// <summary>
    /// Add a component to this entity
    /// </summary>
    public T AddComponent<T>(T component) where T : IComponent
    {
        var componentType = typeof(T);

        // Don't add duplicate component types
        if (_componentCache.ContainsKey(componentType))
        {
            throw new InvalidOperationException($"Entity {Id} already has a component of type {componentType.Name}");
        }

        component.Owner = this;
        _components.Add(component);
        _componentCache[componentType] = component;
        component.Initialize();

        return component;
    }

    /// <summary>
    /// Get a component of the specified type
    /// </summary>
    public T GetComponent<T>() where T : class, IComponent
    {
        var componentType = typeof(T);

        if (_componentCache.TryGetValue(componentType, out var component))
        {
            return component as T;
        }

        return null;
    }

    /// <summary>
    /// Check if this entity has a component of the specified type
    /// </summary>
    public bool HasComponent<T>() where T : IComponent
    {
        return _componentCache.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Remove a component from this entity
    /// </summary>
    public void RemoveComponent<T>() where T : IComponent
    {
        var componentType = typeof(T);

        if (_componentCache.TryGetValue(componentType, out var component))
        {
            _components.Remove(component);
            _componentCache.Remove(componentType);
        }
    }

    /// <summary>
    /// Update all components
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!Active) return;

        foreach (var component in _components)
        {
            if (component.Enabled)
            {
                component.Update(gameTime);
            }
        }
    }

    /// <summary>
    /// Get all components of this entity
    /// </summary>
    public IEnumerable<IComponent> GetAllComponents()
    {
        return _components;
    }
}
