using Avalonia;
using System;
using System.Linq;

namespace FractionTrainer;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        if (args.Contains("--help"))
        {
            Console.WriteLine("FractionTrainer");
            Console.WriteLine("GUI: dotnet run");
            Console.WriteLine("CLI: dotnet run -- --task easy|medium|hard");
            return 0;
        }

        var taskIndex = Array.IndexOf(args, "--task");
        if (taskIndex >= 0)
        {
            var difficulty = args.ElementAtOrDefault(taskIndex + 1) ?? "medium";
            var settings = GameSettings.FromDifficultyName(difficulty);
            var game = new FractionGame(settings);
            Console.WriteLine($"Задание: соберите дробь {game.CurrentTask.Target}");
            Console.WriteLine($"Подсказка: выберите {game.CurrentTask.Target.Numerator} долей из {game.CurrentTask.Target.Denominator}");
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
