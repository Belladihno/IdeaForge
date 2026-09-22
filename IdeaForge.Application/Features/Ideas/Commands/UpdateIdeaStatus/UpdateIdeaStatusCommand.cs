using IdeaForge.Application.DTOs;
using MediatR;

namespace IdeaForge.Application.Features.Ideas.Commands.UpdateIdeaStatus;

public record UpdateIdeaStatusCommand(Guid Id, UpdateIdeaStatusRequest Request) : IRequest;
