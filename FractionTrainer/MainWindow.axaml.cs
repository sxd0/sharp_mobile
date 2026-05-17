using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace FractionTrainer;

public partial class MainWindow : Window
{
    private readonly List<Polygon> sectors = [];
    private FractionGame game;
    private bool waitsForNextRound;

    public MainWindow()
    {
        InitializeComponent();
        game = new FractionGame(ReadSettings());
        game.RoundChecked += Game_RoundChecked;
        UpdateScreen();
    }

    private GameSettings ReadSettings()
    {
        return new GameSettings
        {
            Mode = ModeCombo.SelectedIndex == 1 ? LearningMode.Test : LearningMode.Training,
            Difficulty = DifficultyCombo.SelectedIndex switch
            {
                0 => Difficulty.Easy,
                2 => Difficulty.Hard,
                _ => Difficulty.Medium
            }
        };
    }

    private void SettingsChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ModeCombo is null || DifficultyCombo is null)
        {
            return;
        }

        game = new FractionGame(ReadSettings());
        game.RoundChecked += Game_RoundChecked;
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        waitsForNextRound = false;
        NumeratorText.Text = game.CurrentTask.Target.Numerator.ToString();
        DenominatorText.Text = game.CurrentTask.Target.Denominator.ToString();
        PartsText.Text = game.PartsCount.ToString();
        SelectedText.Text = $"Выбрано: {game.SelectedCount}/{game.PartsCount}";
        ScoreText.Text = $"Счёт: {game.Score}";
        HintPanel.IsVisible = game.Settings.Mode == LearningMode.Training;
        HintText.Text = CreateHint();
        StatusText.Text = "Нажимай на секторы, чтобы собрать нужную дробь.";
        CheckButton.Content = "Проверить";
        DrawSectors();
    }

    private string CreateHint()
    {
        var correct = game.CorrectSectorIndexes.Count;
        if (correct > 0)
        {
            return $"Подсказка: выбери {correct} долей из {game.PartsCount}.";
        }

        return "Подсказка: кнопками измени количество долей, чтобы дробь можно было собрать.";
    }

    private void DrawSectors()
    {
        FractionCanvas.Children.Clear();
        sectors.Clear();

        const double centerX = 330;
        const double centerY = 245;
        const double radius = 190;

        for (var i = 0; i < game.PartsCount; i++)
        {
            var startAngle = 360.0 / game.PartsCount * i - 90;
            var endAngle = 360.0 / game.PartsCount * (i + 1) - 90;
            var sector = CreateSector(centerX, centerY, radius, startAngle, endAngle);
            sector.Tag = i;
            sector.Fill = new SolidColorBrush(game.SelectedParts[i] ? Colors.Orange : Colors.LightBlue);
            sector.Stroke = Brushes.White;
            sector.StrokeThickness = 2;
            sector.PointerPressed += Sector_PointerPressed;

            sectors.Add(sector);
            FractionCanvas.Children.Add(sector);
        }
    }

    private static Polygon CreateSector(double centerX, double centerY, double radius, double startAngle, double endAngle)
    {
        var points = new Avalonia.Collections.AvaloniaList<Avalonia.Point>
        {
            new(centerX, centerY)
        };

        for (var angle = startAngle; angle <= endAngle; angle += 3)
        {
            var rad = Math.PI * angle / 180;
            points.Add(new Avalonia.Point(centerX + radius * Math.Cos(rad), centerY + radius * Math.Sin(rad)));
        }

        var endRad = Math.PI * endAngle / 180;
        points.Add(new Avalonia.Point(centerX + radius * Math.Cos(endRad), centerY + radius * Math.Sin(endRad)));

        return new Polygon { Points = points };
    }

    private void Sector_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (waitsForNextRound)
        {
            return;
        }

        if (sender is not Polygon { Tag: int index })
        {
            return;
        }

        game.TogglePart(index);
        SelectedText.Text = $"Выбрано: {game.SelectedCount}/{game.PartsCount}";
        StatusText.Text = $"Сейчас получилось: {game.SelectedFraction}";
        DrawSectors();
    }

    private void CheckButton_Click(object? sender, RoutedEventArgs e)
    {
        if (waitsForNextRound)
        {
            game.NewRound();
            UpdateScreen();
            return;
        }

        game.Check();
    }

    private void Game_RoundChecked(object? sender, GameRoundCheckedEventArgs e)
    {
        StatusText.Text = e.IsCorrect
            ? $"Правильно: {e.Selected} = {game.CurrentTask.Target}. Нажми «Следующий пример»."
            : $"Пока нет: {e.Selected} не равна {game.CurrentTask.Target}.";

        foreach (var sector in sectors)
        {
            sector.Fill = new SolidColorBrush(e.IsCorrect ? Colors.LightGreen : Colors.LightCoral);
        }

        ScoreText.Text = $"Счёт: {game.Score}";
        waitsForNextRound = e.IsCorrect;
        CheckButton.Content = e.IsCorrect ? "Следующий пример" : "Проверить";
    }

    private void DecreaseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (waitsForNextRound)
        {
            return;
        }

        game.ChangeParts(-1);
        UpdateScreen();
    }

    private void IncreaseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (waitsForNextRound)
        {
            return;
        }

        game.ChangeParts(1);
        UpdateScreen();
    }

    private void NewRoundButton_Click(object? sender, RoutedEventArgs e)
    {
        game.NewRound();
        UpdateScreen();
    }

    private async void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F1)
        {
            await ShowHelp();
        }
    }

    private async System.Threading.Tasks.Task ShowHelp()
    {
        var help = new Window
        {
            Title = "Справка",
            Width = 460,
            Height = 260,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new TextBlock
            {
                Margin = new Avalonia.Thickness(20),
                TextWrapping = TextWrapping.Wrap,
                Text = "Цель: собрать показанную дробь.\n\nКлик по сектору выбирает или отменяет долю. Кнопки '- доля' и '+ доля' меняют количество частей. В режиме обучения показывается подсказка.\n\nCLI: dotnet run -- --task easy"
            }
        };

        await help.ShowDialog(this);
    }
}
