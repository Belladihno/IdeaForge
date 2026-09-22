using IdeaForge.Application.DTOs;
using IdeaForge.Application.Interfaces;
using Mapster;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Queries.GetRankedIdeas;

public class GetRankedIdeasHandler : IRequestHandler<GetRankedIdeasQuery, IReadOnlyList<IdeaResponse>>
{
    private readonly IIdeaRepository _repository;

    public GetRankedIdeasHandler(IIdeaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<IdeaResponse>> Handle(GetRankedIdeasQuery query, CancellationToken cancellationToken)
    {
        var ideas = await _repository.GetAllRankedAsync(
            query.Department,
            query.Status,
            query.Search,
            cancellationToken);

        return ideas.Adapt<IReadOnlyList<IdeaResponse>>();
    }
}
