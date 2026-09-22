using IdeaForge.Application.Interfaces;
using IdeaForge.Domain.Entities;
using IdeaForge.Domain.Enums;
using IdeaForge.Domain.Services;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Commands.CreateIdea;

public class CreateIdeaHandler : IRequestHandler<CreateIdeaCommand, Guid>
{
    private readonly IIdeaRepository _repository;

    public CreateIdeaHandler(IIdeaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateIdeaCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        var priorityScore = PriorityScoreEngine.Calculate(
            req.ValueScore,
            req.FeasibilityScore,
            req.UrgencyScore,
            req.RiskScore);

        var now = DateTime.UtcNow;

        var idea = new Idea
        {
            Id = Guid.NewGuid(),
            Title = req.Title.Trim(),
            Description = req.Description.Trim(),
            Department = req.Department,
            SubmittedBy = req.SubmittedBy.Trim(),
            ValueScore = req.ValueScore,
            FeasibilityScore = req.FeasibilityScore,
            UrgencyScore = req.UrgencyScore,
            RiskScore = req.RiskScore,
            PriorityScore = priorityScore,
            Status = IdeaStatus.Captured,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repository.AddAsync(idea, cancellationToken);

        return idea.Id;
    }
}
