using IdeaForge.Domain.Entities;
using IdeaForge.Domain.Enums;

namespace IdeaForge.Application.Interfaces;

public interface IIdeaRepository
{
    Task AddAsync(Idea idea, CancellationToken cancellationToken = default);
    Task<Idea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Idea>> GetAllRankedAsync(Department? department, IdeaStatus? status, string? search, CancellationToken cancellationToken = default);
    Task UpdateAsync(Idea idea, CancellationToken cancellationToken = default);
}
