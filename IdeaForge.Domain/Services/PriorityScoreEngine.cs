namespace IdeaForge.Domain.Services;

public static class PriorityScoreEngine
{
    public static decimal Calculate(int value, int feasibility, int urgency, int risk)
    {
        var raw = (decimal)(value * 2.0 + urgency * 1.5 + feasibility * 1.0 - risk * 1.5);

        // Scores are clamped at zero: a leaderboard never shows negatives.
        return Math.Max(0, raw);
    }
}
