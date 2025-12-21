using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TheMythalProphecy.Game.Entities;

/// <summary>
/// Manages all entities in the game
/// Handles entity creation, updates, and cleanup
/// </summary>
public class EntityManager
{
    private readonly List<Entity> _entities = new();
    private readonly Dictionary<string, Entity> _entityCache = new();
    private readonly List<Entity> _entitiesToAdd = new();
    private readonly List<Entity> _entitiesToRemove = new();

    /// <summary>
    /// Add an entity to be managed
    /// </summary>
    public void AddEntity(Entity entity)
    {
        _entitiesToAdd.Add(entity);
    }

    /// <summary>
    /// Remove an entity from management
    /// </summary>
    public void RemoveEntity(Entity entity)
    {
        _entitiesToRemove.Add(entity);
    }

    /// <summary>
    /// Remove an entity by ID
    /// </summary>
    public void RemoveEntity(string id)
    {
        var entity = GetEntity(id);
        if (entity != null)
        {
            RemoveEntity(entity);
        }
    }

    /// <summary>
    /// Get an entity by ID
    /// </summary>
    public Entity GetEntity(string id)
    {
        _entityCache.TryGetValue(id, out var entity);
        return entity;
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public IEnumerable<Entity> GetAllEntities()
    {
        return _entities;
    }

    /// <summary>
    /// Get all entities with a specific component
    /// </summary>
    public IEnumerable<Entity> GetEntitiesWithComponent<T>() where T : class, Components.IComponent
    {
        return _entities.Where(e => e.HasComponent<T>());
    }

    /// <summary>
    /// Update all entities
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // Add pending entities
        if (_entitiesToAdd.Count > 0)
        {
            foreach (var entity in _entitiesToAdd)
            {
                _entities.Add(entity);
                _entityCache[entity.Id] = entity;
            }
            _entitiesToAdd.Clear();
        }

        // Update all active entities
        foreach (var entity in _entities)
        {
            entity.Update(gameTime);
        }

        // Remove pending entities
        if (_entitiesToRemove.Count > 0)
        {
            foreach (var entity in _entitiesToRemove)
            {
                _entities.Remove(entity);
                _entityCache.Remove(entity.Id);
            }
            _entitiesToRemove.Clear();
        }
    }

    /// <summary>
    /// Clear all entities
    /// </summary>
    public void Clear()
    {
        _entities.Clear();
        _entityCache.Clear();
        _entitiesToAdd.Clear();
        _entitiesToRemove.Clear();
    }
}
