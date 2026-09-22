using IdeaForge.Domain.Enums;

namespace IdeaForge.Application.DTOs;

public class UpdateIdeaStatusRequest
{
    public IdeaStatus Status { get; set; }
    public string? RejectionReason { get; set; }
}
