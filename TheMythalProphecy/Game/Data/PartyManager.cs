using System;
using System.Collections.Generic;
using System.Linq;
using TheMythalProphecy.Game.Entities;
using TheMythalProphecy.Game.Systems.Events;

namespace TheMythalProphecy.Game.Data;

/// <summary>
/// Manages party composition with active and reserve members
/// Maximum 4 active party members, unlimited reserves
/// </summary>
public class PartyManager
{
    private readonly List<Entity> _activeParty = new();
    private readonly List<Entity> _reserveParty = new();

    public const int MaxActivePartySize = 4;

    /// <summary>
    /// Event raised when the party composition changes
    /// </summary>
    public event Action OnPartyChanged;

    /// <summary>
    /// Raises the party changed event and publishes to EventManager
    /// </summary>
    private void RaisePartyChanged()
    {
        OnPartyChanged?.Invoke();
        Core.GameServices.Events?.Publish(new PartyChangedEvent());
    }

    /// <summary>
    /// Gets the active party members (read-only)
    /// </summary>
    public IReadOnlyList<Entity> ActiveParty => _activeParty.AsReadOnly();

    /// <summary>
    /// Gets the reserve party members (read-only)
    /// </summary>
    public IReadOnlyList<Entity> ReserveParty => _reserveParty.AsReadOnly();

    /// <summary>
    /// Gets the number of active party members
    /// </summary>
    public int ActivePartyCount => _activeParty.Count;

    /// <summary>
    /// Gets the number of reserve party members
    /// </summary>
    public int ReservePartyCount => _reserveParty.Count;

    /// <summary>
    /// Adds a character to the active party (if space available) or reserves
    /// </summary>
    public bool AddToParty(Entity character)
    {
        if (character == null)
            return false;

        // Don't add if already in either party
        if (_activeParty.Contains(character) || _reserveParty.Contains(character))
            return false;

        // Add to active party if space available
        if (_activeParty.Count < MaxActivePartySize)
        {
            _activeParty.Add(character);
            RaisePartyChanged();
            return true;
        }

        // Otherwise add to reserves
        _reserveParty.Add(character);
        RaisePartyChanged();
        return true;
    }

    /// <summary>
    /// Removes a character from the party entirely
    /// </summary>
    public bool RemoveFromParty(Entity character)
    {
        if (character == null)
            return false;

        bool removed = _activeParty.Remove(character) || _reserveParty.Remove(character);

        if (removed)
        {
            RaisePartyChanged();
        }

        return removed;
    }

    /// <summary>
    /// Swaps two active party members
    /// </summary>
    public bool SwapActiveMembers(int index1, int index2)
    {
        if (index1 < 0 || index1 >= _activeParty.Count ||
            index2 < 0 || index2 >= _activeParty.Count)
            return false;

        (_activeParty[index1], _activeParty[index2]) = (_activeParty[index2], _activeParty[index1]);

        RaisePartyChanged();
        return true;
    }

    /// <summary>
    /// Moves an active party member to reserves
    /// </summary>
    public bool MoveToReserve(int activeIndex)
    {
        if (activeIndex < 0 || activeIndex >= _activeParty.Count)
            return false;

        var character = _activeParty[activeIndex];
        _activeParty.RemoveAt(activeIndex);
        _reserveParty.Add(character);

        RaisePartyChanged();
        return true;
    }

    /// <summary>
    /// Moves a reserve member to active party (if space available)
    /// </summary>
    public bool MoveToActive(int reserveIndex)
    {
        if (reserveIndex < 0 || reserveIndex >= _reserveParty.Count)
            return false;

        if (_activeParty.Count >= MaxActivePartySize)
            return false;

        var character = _reserveParty[reserveIndex];
        _reserveParty.RemoveAt(reserveIndex);
        _activeParty.Add(character);

        RaisePartyChanged();
        return true;
    }

    /// <summary>
    /// Swaps an active member with a reserve member
    /// </summary>
    public bool SwapActiveWithReserve(int activeIndex, int reserveIndex)
    {
        if (activeIndex < 0 || activeIndex >= _activeParty.Count ||
            reserveIndex < 0 || reserveIndex >= _reserveParty.Count)
            return false;

        var activeChar = _activeParty[activeIndex];
        var reserveChar = _reserveParty[reserveIndex];

        _activeParty[activeIndex] = reserveChar;
        _reserveParty[reserveIndex] = activeChar;

        RaisePartyChanged();
        return true;
    }

    /// <summary>
    /// Gets the party leader (first active member)
    /// </summary>
    public Entity GetLeader()
    {
        return _activeParty.Count > 0 ? _activeParty[0] : null;
    }

    /// <summary>
    /// Gets all party members (active + reserves)
    /// </summary>
    public IEnumerable<Entity> GetAllMembers()
    {
        return _activeParty.Concat(_reserveParty);
    }

    /// <summary>
    /// Clears the entire party
    /// </summary>
    public void Clear()
    {
        _activeParty.Clear();
        _reserveParty.Clear();
        RaisePartyChanged();
    }
}
