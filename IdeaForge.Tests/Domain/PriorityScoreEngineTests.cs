using FluentAssertions;
using IdeaForge.Domain.Services;

namespace IdeaForge.Tests.Domain;

public class PriorityScoreEngineTests
{
    [Fact]
    public void Calculate_MaxScoresWithMinRisk_ReturnsHighestPossibleScore()
    {
        var score = PriorityScoreEngine.Calculate(value: 5, feasibility: 5, urgency: 5, risk: 1);

        score.Should().Be(21m);
    }

    [Fact]
    public void Calculate_MinScoresWithMaxRisk_ClampsAtZero()
    {
        var score = PriorityScoreEngine.Calculate(value: 1, feasibility: 1, urgency: 1, risk: 5);

        score.Should().Be(0m);
    }

    [Fact]
    public void Calculate_HighRisk_ReducesPriorityScore()
    {
        var lowRisk = PriorityScoreEngine.Calculate(value: 4, feasibility: 4, urgency: 4, risk: 1);
        var highRisk = PriorityScoreEngine.Calculate(value: 4, feasibility: 4, urgency: 4, risk: 5);

        highRisk.Should().BeLessThan(lowRisk);
    }

    [Fact]
    public void Calculate_KnownInputs_ReturnsExpectedScore()
    {
        // TRD example: Value 5, Feasibility 3, Urgency 4, Risk 1
        var score = PriorityScoreEngine.Calculate(value: 5, feasibility: 3, urgency: 4, risk: 1);

        score.Should().Be(17.5m);
    }
}
