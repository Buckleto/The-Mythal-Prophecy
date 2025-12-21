using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Data.Definitions.Databases;

/// <summary>
/// Centralized database for character templates
/// Used for creating playable and NPC characters
/// </summary>
public class CharacterDatabase
{
    private readonly Dictionary<string, CharacterDefinition> _characters = new();

    public IReadOnlyDictionary<string, CharacterDefinition> Characters => _characters;

    /// <summary>
    /// Register a character definition
    /// </summary>
    public void Register(CharacterDefinition character)
    {
        if (string.IsNullOrEmpty(character.Id))
            throw new ArgumentException("Character ID cannot be null or empty");

        _characters[character.Id] = character;
    }

    /// <summary>
    /// Get character definition by ID
    /// </summary>
    public CharacterDefinition Get(string characterId)
    {
        return _characters.TryGetValue(characterId, out var ch) ? ch : null;
    }

    /// <summary>
    /// Check if character exists
    /// </summary>
    public bool Exists(string characterId)
    {
        return _characters.ContainsKey(characterId);
    }

    /// <summary>
    /// Clear all definitions (for reinitialization)
    /// </summary>
    public void Clear()
    {
        _characters.Clear();
    }
}
