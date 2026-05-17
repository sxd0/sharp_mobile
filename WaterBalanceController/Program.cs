using Avalonia;
using System;
using System.Linq;

namespace WaterBalanceController;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        if (args.Contains("--help"))
        {
            Console.WriteLine("WaterBalanceController");
            Console.WriteLine("GUI: dotnet run");
            Console.WriteLine("CLI: dotnet run -- --add 250");
            Console.WriteLine("CLI: dotnet run -- --status");
            return 0;
        }

        if (args.Contains("--status") || args.Contains("--add"))
        {
            var store = new WaterStorage();
            var tracker = store.Load();
            var addIndex = Array.IndexOf(args, "--add");
            if (addIndex >= 0 && int.TryParse(args.ElementAtOrDefault(addIndex + 1), out var amount))
            {
                tracker.AddWater(amount);
                store.Save(tracker);
                Console.WriteLine($"Добавлено: {amount} мл");
            }

            Console.WriteLine($"Сегодня: {tracker.TodayTotal} / {tracker.Settings.DailyGoal} мл ({tracker.ProgressPercent:0}%)");
            return 0;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        return 0;
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
