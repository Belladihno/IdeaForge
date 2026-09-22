using IdeaForge.Application.Interfaces;
using IdeaForge.Domain.Entities;
using IdeaForge.Domain.Enums;
using IdeaForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdeaForge.Infrastructure.Repositories;

public class IdeaRepository : IIdeaRepository
{
    private readonly AppDbContext _context;

    public IdeaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Idea idea, CancellationToken cancellationToken = default)
    {
        await _context.Ideas.AddAsync(idea, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<Idea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Ideas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Idea>> GetAllRankedAsync(
        Department? department,
        IdeaStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Ideas.AsNoTracking().AsQueryable();

        if (department.HasValue)
            query = query.Where(x => x.Department == department.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Title.Contains(search));

        return await query
            .OrderByDescending(x => x.PriorityScore)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Idea idea, CancellationToken cancellationToken = default)
    {
        _context.Ideas.Update(idea);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
