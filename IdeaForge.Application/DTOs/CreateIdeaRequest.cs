using IdeaForge.Domain.Enums;

namespace IdeaForge.Application.DTOs;

public class CreateIdeaRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Department Department { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public int ValueScore { get; set; }
    public int FeasibilityScore { get; set; }
    public int UrgencyScore { get; set; }
    public int RiskScore { get; set; }
}
