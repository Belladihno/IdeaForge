using IdeaForge.Application.DTOs;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Queries.GetIdeaById;

public record GetIdeaByIdQuery(Guid Id) : IRequest<IdeaResponse>;
