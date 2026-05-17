using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Linq;

namespace WaterBalanceController;

public partial class MainWindow : Window
{
    private readonly WaterStorage storage = new();
    private readonly WaterTracker tracker;

    public MainWindow()
    {
        InitializeComponent();
        tracker = storage.Load();
        tracker.Changed += (_, _) => SaveAndRefresh();
        Refresh();
    }

    private void Refresh()
    {
        TotalText.Text = $"{tracker.TodayTotal} / {tracker.Settings.DailyGoal} мл";
        PercentText.Text = $"{tracker.ProgressPercent:0}%";
        ProgressBar.Value = tracker.ProgressPercent;
        GoalInput.Value = tracker.Settings.DailyGoal;

        SmallButton.Content = $"+{tracker.Settings.QuickSmall}";
        SmallButton.Tag = tracker.Settings.QuickSmall;
        MediumButton.Content = $"+{tracker.Settings.QuickMedium}";
        MediumButton.Tag = tracker.Settings.QuickMedium;
        LargeButton.Content = $"+{tracker.Settings.QuickLarge}";
        LargeButton.Tag = tracker.Settings.QuickLarge;

        HistoryList.ItemsSource = tracker.TodayEntries
            .Select(entry => $"{entry.Time:HH:mm} - {entry.Milliliters} мл")
            .ToList();
    }

    private void SaveAndRefresh()
    {
        storage.Save(tracker);
        Refresh();
    }

    private void QuickAdd_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int amount })
        {
            tracker.AddWater(amount);
            StatusText.Text = $"Добавлено {amount} мл.";
        }
    }

    private void AddCustom_Click(object? sender, RoutedEventArgs e)
    {
        var amount = (int)(AmountInput.Value ?? 0);
        tracker.AddWater(amount);
        StatusText.Text = $"Добавлено {amount} мл.";
    }

    private void SaveGoal_Click(object? sender, RoutedEventArgs e)
    {
        tracker.UpdateGoal((int)(GoalInput.Value ?? tracker.Settings.DailyGoal));
        StatusText.Text = "Цель на день сохранена.";
    }

    private async void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F1)
        {
            var help = new Window
            {
                Title = "Справка",
                Width = 440,
                Height = 240,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new TextBlock
                {
                    Margin = new Avalonia.Thickness(20),
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Text = "Добавляй каждую выпитую порцию воды. Прогресс считается относительно дневной цели.\n\nCLI: dotnet run -- --add 250\nCLI: dotnet run -- --status"
                }
            };

            await help.ShowDialog(this);
        }
    }
}
