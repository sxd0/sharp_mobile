using System;
using System.IO;
using System.Text.Json;

namespace WaterBalanceController;

public sealed class WaterStorage
{
    private readonly string filePath;

    public WaterStorage()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WaterBalanceController");
        Directory.CreateDirectory(directory);
        filePath = Path.Combine(directory, "water-balance.json");
    }

    public WaterTracker Load()
    {
        if (!File.Exists(filePath))
        {
            return new WaterTracker();
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<WaterTracker>(json) ?? new WaterTracker();
    }

    public void Save(WaterTracker tracker)
    {
        var json = JsonSerializer.Serialize(tracker, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}
