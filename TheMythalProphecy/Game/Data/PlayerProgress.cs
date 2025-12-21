using System.Collections.Generic;

namespace TheMythalProphecy.Game.Data;

/// <summary>
/// Tracks player progression data (gold, location, flags, etc.)
/// Used for save/load operations
/// </summary>
public class PlayerProgress
{
    public int Gold { get; set; }
    public string CurrentLocation { get; set; } = "Starting Area";
    public int PlayTimeSeconds { get; set; }

    // Story/quest flags
    public Dictionary<string, bool> Flags { get; set; } = new();
    public Dictionary<string, int> Variables { get; set; } = new();

    // Unlocked locations/features
    public HashSet<string> UnlockedLocations { get; set; } = new();
    public HashSet<string> DiscoveredItems { get; set; } = new();

    /// <summary>
    /// Gets or sets a flag value
    /// </summary>
    public bool GetFlag(string flagName) => Flags.TryGetValue(flagName, out bool value) && value;

    /// <summary>
    /// Sets a flag value
    /// </summary>
    public void SetFlag(string flagName, bool value)
    {
        Flags[flagName] = value;
    }

    /// <summary>
    /// Gets a variable value (returns 0 if not found)
    /// </summary>
    public int GetVariable(string variableName) => Variables.TryGetValue(variableName, out int value) ? value : 0;

    /// <summary>
    /// Sets a variable value
    /// </summary>
    public void SetVariable(string variableName, int value)
    {
        Variables[variableName] = value;
    }
}
