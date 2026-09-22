using FluentValidation;
using IdeaForge.Domain.Enums;

namespace IdeaForge.Application.Features.Ideas.Commands.UpdateIdeaStatus;

public class UpdateIdeaStatusValidator : AbstractValidator<UpdateIdeaStatusCommand>
{
    public UpdateIdeaStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Request.Status)
            .IsInEnum().WithMessage("Status must be a valid value.");

        RuleFor(x => x.Request.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required when rejecting an idea.")
            .When(x => x.Request.Status == IdeaStatus.Rejected);
    }
}
