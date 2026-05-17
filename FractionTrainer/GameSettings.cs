namespace FractionTrainer;

public enum LearningMode
{
    Training,
    Test
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public sealed class GameSettings
{
    public LearningMode Mode { get; set; } = LearningMode.Training;
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public int InitialParts { get; set; } = 4;

    public int MaxDenominator => Difficulty switch
    {
        Difficulty.Easy => 6,
        Difficulty.Medium => 10,
        Difficulty.Hard => 16,
        _ => 10
    };

    public static GameSettings FromDifficultyName(string name)
    {
        var difficulty = name.ToLowerInvariant() switch
        {
            "easy" => Difficulty.Easy,
            "hard" => Difficulty.Hard,
            _ => Difficulty.Medium
        };

        return new GameSettings { Difficulty = difficulty };
    }
}
