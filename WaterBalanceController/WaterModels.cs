using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterBalanceController;

public sealed class WaterEntry
{
    public WaterEntry()
    {
    }

    public WaterEntry(int milliliters, DateTime time)
    {
        Milliliters = milliliters;
        Time = time;
    }

    public int Milliliters { get; set; }
    public DateTime Time { get; set; }
}

public sealed class WaterSettings
{
    public int DailyGoal { get; set; } = 2000;
    public int QuickSmall { get; set; } = 200;
    public int QuickMedium { get; set; } = 300;
    public int QuickLarge { get; set; } = 500;
}

public sealed class WaterTracker
{
    public event EventHandler? Changed;

    public WaterSettings Settings { get; set; } = new();
    public List<WaterEntry> Entries { get; set; } = [];

    public int TodayTotal => Entries
        .Where(entry => entry.Time.Date == DateTime.Today)
        .Sum(entry => entry.Milliliters);

    public double ProgressPercent => Settings.DailyGoal <= 0
        ? 0
        : Math.Min(100, TodayTotal * 100.0 / Settings.DailyGoal);

    public IReadOnlyList<WaterEntry> TodayEntries => Entries
        .Where(entry => entry.Time.Date == DateTime.Today)
        .OrderByDescending(entry => entry.Time)
        .ToList();

    public void AddWater(int milliliters)
    {
        if (milliliters <= 0)
        {
            return;
        }

        Entries.Add(new WaterEntry(milliliters, DateTime.Now));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateGoal(int milliliters)
    {
        Settings.DailyGoal = Math.Clamp(milliliters, 500, 6000);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void ResetToday()
    {
        Entries.RemoveAll(entry => entry.Time.Date == DateTime.Today);
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
