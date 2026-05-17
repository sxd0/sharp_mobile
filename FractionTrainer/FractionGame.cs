using System;
using System.Collections.Generic;
using System.Linq;

namespace FractionTrainer;

public sealed class FractionTask
{
    public FractionTask(Fraction target)
    {
        Target = target;
    }

    public Fraction Target { get; }
}

public sealed class GameRoundCheckedEventArgs : EventArgs
{
    public GameRoundCheckedEventArgs(bool isCorrect, Fraction selected)
    {
        IsCorrect = isCorrect;
        Selected = selected;
    }

    public bool IsCorrect { get; }
    public Fraction Selected { get; }
}

public sealed class FractionGame
{
    private readonly Random random = new();

    public FractionGame(GameSettings settings)
    {
        Settings = settings;
        PartsCount = Math.Max(2, settings.InitialParts);
        CurrentTask = CreateTask();
        SelectedParts = new bool[PartsCount];
    }

    public event EventHandler<GameRoundCheckedEventArgs>? RoundChecked;

    public GameSettings Settings { get; }
    public FractionTask CurrentTask { get; private set; }
    public int PartsCount { get; private set; }
    public bool[] SelectedParts { get; private set; }
    public int Score { get; private set; }

    public int SelectedCount => SelectedParts.Count(part => part);
    public Fraction SelectedFraction => new(SelectedCount, PartsCount);

    public IReadOnlyList<int> CorrectSectorIndexes
    {
        get
        {
            var result = new List<int>();
            for (var count = 1; count <= PartsCount; count++)
            {
                if (new Fraction(count, PartsCount).Equals(CurrentTask.Target))
                {
                    for (var i = 0; i < count; i++)
                    {
                        result.Add(i);
                    }
                    break;
                }
            }

            return result;
        }
    }

    public void NewRound()
    {
        CurrentTask = CreateTask();
        PartsCount = Math.Max(CurrentTask.Target.Denominator, Settings.InitialParts);
        SelectedParts = new bool[PartsCount];
    }

    public void ChangeParts(int delta)
    {
        PartsCount = Math.Clamp(PartsCount + delta, 2, 20);
        SelectedParts = new bool[PartsCount];
    }

    public void TogglePart(int index)
    {
        if (index < 0 || index >= SelectedParts.Length)
        {
            return;
        }

        SelectedParts[index] = !SelectedParts[index];
    }

    public bool Check()
    {
        var selected = SelectedFraction;
        var isCorrect = selected.Equals(CurrentTask.Target);
        if (isCorrect)
        {
            Score += 10;
        }

        RoundChecked?.Invoke(this, new GameRoundCheckedEventArgs(isCorrect, selected));
        return isCorrect;
    }

    private FractionTask CreateTask()
    {
        var denominator = random.Next(2, Settings.MaxDenominator + 1);
        var numerator = random.Next(1, denominator);
        return new FractionTask(new Fraction(numerator, denominator));
    }
}
