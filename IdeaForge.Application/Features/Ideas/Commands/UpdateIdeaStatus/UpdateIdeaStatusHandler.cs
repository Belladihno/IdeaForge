using IdeaForge.Application.Interfaces;
using IdeaForge.Domain.Enums;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Commands.UpdateIdeaStatus;

public class UpdateIdeaStatusHandler : IRequestHandler<UpdateIdeaStatusCommand>
{
    private readonly IIdeaRepository _repository;

    public UpdateIdeaStatusHandler(IIdeaRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateIdeaStatusCommand command, CancellationToken cancellationToken)
    {
        var idea = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (idea is null)
            throw new KeyNotFoundException($"Idea with id {command.Id} was not found.");

        var newStatus = command.Request.Status;

        if (!IsValidTransition(idea.Status, newStatus))
            throw new InvalidOperationException($"Cannot move idea from {idea.Status} to {newStatus}.");

        idea.Status = newStatus;
        idea.RejectionReason = newStatus == IdeaStatus.Rejected
            ? command.Request.RejectionReason?.Trim()
            : null;
        idea.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(idea, cancellationToken);
    }

    private static bool IsValidTransition(IdeaStatus current, IdeaStatus next)
    {
        if (current == next)
            return true;

        // Rejected is allowed from any non-terminal stage
        if (next == IdeaStatus.Rejected)
            return current is IdeaStatus.Captured or IdeaStatus.Evaluated or IdeaStatus.InBuild;

        return current switch
        {
            IdeaStatus.Captured => next == IdeaStatus.Evaluated,
            IdeaStatus.Evaluated => next == IdeaStatus.InBuild,
            IdeaStatus.InBuild => next == IdeaStatus.Live,
            // Live and Rejected are terminal
            _ => false
        };
    }
}
