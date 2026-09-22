using FluentValidation;

namespace IdeaForge.Application.Features.Ideas.Commands.CreateIdea;

public class CreateIdeaValidator : AbstractValidator<CreateIdeaCommand>
{
    public CreateIdeaValidator()
    {
        RuleFor(x => x.Request.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Request.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Request.SubmittedBy)
            .NotEmpty().WithMessage("Submitter name is required.")
            .MaximumLength(100).WithMessage("Submitter name must not exceed 100 characters.");

        RuleFor(x => x.Request.Department)
            .IsInEnum().WithMessage("Department must be a valid value.");

        RuleFor(x => x.Request.ValueScore)
            .InclusiveBetween(1, 5).WithMessage("Value score must be between 1 and 5.");

        RuleFor(x => x.Request.FeasibilityScore)
            .InclusiveBetween(1, 5).WithMessage("Feasibility score must be between 1 and 5.");

        RuleFor(x => x.Request.UrgencyScore)
            .InclusiveBetween(1, 5).WithMessage("Urgency score must be between 1 and 5.");

        RuleFor(x => x.Request.RiskScore)
            .InclusiveBetween(1, 5).WithMessage("Risk score must be between 1 and 5.");
    }
}
