using IdeaForge.Application.DTOs;
using IdeaForge.Application.Interfaces;
using Mapster;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Queries.GetIdeaById;

public class GetIdeaByIdHandler : IRequestHandler<GetIdeaByIdQuery, IdeaResponse>
{
    private readonly IIdeaRepository _repository;

    public GetIdeaByIdHandler(IIdeaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IdeaResponse> Handle(GetIdeaByIdQuery query, CancellationToken cancellationToken)
    {
        var idea = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (idea is null)
            throw new KeyNotFoundException($"Idea with id {query.Id} was not found.");

        return idea.Adapt<IdeaResponse>();
    }
}
