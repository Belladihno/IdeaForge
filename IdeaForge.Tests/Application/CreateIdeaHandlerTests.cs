using FluentAssertions;
using IdeaForge.Application.DTOs;
using IdeaForge.Application.Features.Ideas.Commands.CreateIdea;
using IdeaForge.Application.Interfaces;
using IdeaForge.Domain.Entities;
using IdeaForge.Domain.Enums;
using NSubstitute;

namespace IdeaForge.Tests.Application;

public class CreateIdeaHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryOnceWithScoredEntity()
    {
        // Arrange
        var repository = Substitute.For<IIdeaRepository>();
        var handler = new CreateIdeaHandler(repository);

        var request = new CreateIdeaRequest
        {
            Title = "Automate invoice processing",
            Description = "Use AI to extract invoice data",
            Department = Department.Finance,
            SubmittedBy = "John Doe",
            ValueScore = 5,
            FeasibilityScore = 3,
            UrgencyScore = 4,
            RiskScore = 1
        };

        // Act
        var id = await handler.Handle(new CreateIdeaCommand(request), CancellationToken.None);

        // Assert
        id.Should().NotBeEmpty();
        await repository.Received(1).AddAsync(
            Arg.Is<Idea>(i =>
                i.Id == id &&
                i.Title == request.Title &&
                i.PriorityScore == 17.5m &&
                i.Status == IdeaStatus.Captured),
            Arg.Any<CancellationToken>());
    }
}
