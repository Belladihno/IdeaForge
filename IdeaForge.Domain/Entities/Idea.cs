using IdeaForge.Domain.Enums;

namespace IdeaForge.Domain.Entities;

public class Idea
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Department Department { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public int ValueScore { get; set; }
    public int FeasibilityScore { get; set; }
    public int UrgencyScore { get; set; }
    public int RiskScore { get; set; }
    public decimal PriorityScore { get; set; }
    public IdeaStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
