using TheMythalProphecy.Game.Data.Definitions.Databases;

namespace TheMythalProphecy.Game.Data;

/// <summary>
/// Central container for all runtime game data
/// Accessible globally via GameServices.GameData
/// </summary>
public class GameData
{
    public PartyManager Party { get; }
    public Inventory Inventory { get; }
    public PlayerProgress Progress { get; }
    public ItemDatabase ItemDatabase { get; }
    public EquipmentDatabase EquipmentDatabase { get; }
    public CharacterDatabase CharacterDatabase { get; }

    public GameData()
    {
        Party = new PartyManager();
        Inventory = new Inventory();
        Progress = new PlayerProgress();

        // Initialize databases
        ItemDatabase = new ItemDatabase();
        EquipmentDatabase = new EquipmentDatabase();
        CharacterDatabase = new CharacterDatabase();
    }

    /// <summary>
    /// Resets all game data to initial state
    /// </summary>
    public void Reset()
    {
        Party.Clear();
        Inventory.Clear();
        Progress.Gold = 0;
        Progress.CurrentLocation = "Starting Area";
        Progress.PlayTimeSeconds = 0;
        Progress.Flags.Clear();
        Progress.Variables.Clear();
        Progress.UnlockedLocations.Clear();
        Progress.DiscoveredItems.Clear();
    }
}
