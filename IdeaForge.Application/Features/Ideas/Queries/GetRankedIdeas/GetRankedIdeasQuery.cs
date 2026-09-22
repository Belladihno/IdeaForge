using IdeaForge.Application.DTOs;
using IdeaForge.Domain.Enums;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Queries.GetRankedIdeas;

public record GetRankedIdeasQuery(
    Department? Department,
    IdeaStatus? Status,
    string? Search) : IRequest<IReadOnlyList<IdeaResponse>>;
