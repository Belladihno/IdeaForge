using IdeaForge.Application.DTOs;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Commands.CreateIdea;

public record CreateIdeaCommand(CreateIdeaRequest Request) : IRequest<Guid>;
