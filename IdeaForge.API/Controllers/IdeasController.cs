using IdeaForge.API.Common;
using IdeaForge.Application.DTOs;
using IdeaForge.Application.Features.Ideas.Commands.CreateIdea;
using IdeaForge.Application.Features.Ideas.Commands.UpdateIdeaStatus;
using IdeaForge.Application.Features.Ideas.Queries.GetIdeaById;
using IdeaForge.Application.Features.Ideas.Queries.GetRankedIdeas;
using IdeaForge.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdeaForge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdeasController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdeasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create(
        [FromBody] CreateIdeaRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateIdeaCommand(request), cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id },
            ApiResponse<object>.Ok(new { id }, "Idea submitted successfully"));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<IdeaResponse>>>> GetAll(
        [FromQuery] string? department,
        [FromQuery] string? status,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        Department? dept = null;
        if (!string.IsNullOrWhiteSpace(department) && Enum.TryParse<Department>(department, true, out var d))
            dept = d;

        IdeaStatus? st = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<IdeaStatus>(status, true, out var s))
            st = s;

        var ideas = await _mediator.Send(new GetRankedIdeasQuery(dept, st, search), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<IdeaResponse>>.Ok(ideas, "Ideas retrieved successfully"));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<IdeaResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var idea = await _mediator.Send(new GetIdeaByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<IdeaResponse>.Ok(idea, "Idea retrieved successfully"));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        Guid id,
        [FromBody] UpdateIdeaStatusRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateIdeaStatusCommand(id, request), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, $"Status updated to {request.Status}"));
    }
}
