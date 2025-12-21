using System;
using System.IO;
using System.Text.Json;

namespace TheMythalProphecy.Game.Data;

/// <summary>
/// Game settings (audio, video, controls)
/// Persisted to JSON file
/// </summary>
public class GameSettings
{
    // Audio settings
    public float MasterVolume { get; set; } = 1.0f;
    public float MusicVolume { get; set; } = 0.8f;
    public float SFXVolume { get; set; } = 1.0f;
    public bool MusicEnabled { get; set; } = true;
    public bool SFXEnabled { get; set; } = true;

    // Video settings
    public int ResolutionWidth { get; set; } = 1280;
    public int ResolutionHeight { get; set; } = 720;
    public bool Fullscreen { get; set; } = false;
    public bool VSync { get; set; } = true;

    // File path for settings
    private static readonly string SettingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TheMythalProphecy",
        "settings.json"
    );

    /// <summary>
    /// Save settings to file
    /// </summary>
    public void Save()
    {
        try
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(SettingsFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Serialize to JSON
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Write to file
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    /// <summary>
    /// Load settings from file (or create default if not found)
    /// </summary>
    public static GameSettings Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<GameSettings>(json);
                return settings ?? new GameSettings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
        }

        // Return default settings if load failed
        return new GameSettings();
    }

    /// <summary>
    /// Apply audio settings to AudioManager
    /// </summary>
    public void ApplyAudioSettings()
    {
        var audio = Core.GameServices.Audio;
        if (audio != null)
        {
            audio.MasterVolume = MasterVolume;
            audio.MusicVolume = MusicVolume;
            // Note: AudioManager doesn't have separate SFX volume control yet
            // This is fine for now - can be extended later
        }
    }
}
