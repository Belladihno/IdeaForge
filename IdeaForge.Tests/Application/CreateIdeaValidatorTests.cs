using FluentAssertions;
using IdeaForge.Application.DTOs;
using IdeaForge.Application.Features.Ideas.Commands.CreateIdea;
using IdeaForge.Domain.Enums;

namespace IdeaForge.Tests.Application;

public class CreateIdeaValidatorTests
{
    private readonly CreateIdeaValidator _validator = new();

    private static CreateIdeaCommand ValidCommand() => new(new CreateIdeaRequest
    {
        Title = "AI chatbot for HR",
        Description = "Answer common HR questions",
        Department = Department.HR,
        SubmittedBy = "Jane Smith",
        ValueScore = 4,
        FeasibilityScore = 4,
        UrgencyScore = 3,
        RiskScore = 2
    });

    [Fact]
    public async Task Validate_EmptyTitle_ReturnsValidationError()
    {
        var command = ValidCommand();
        command.Request.Title = string.Empty;

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Title"));
    }

    [Fact]
    public async Task Validate_ScoreAboveFive_ReturnsValidationError()
    {
        var command = ValidCommand();
        command.Request.ValueScore = 6;

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("ValueScore"));
    }

    [Fact]
    public async Task Validate_ValidRequest_PassesValidation()
    {
        var result = await _validator.ValidateAsync(ValidCommand());

        result.IsValid.Should().BeTrue();
    }
}
