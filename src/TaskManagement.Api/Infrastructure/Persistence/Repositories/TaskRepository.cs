using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Application.Repositories;
using TaskManagement.Api.Domain.Entities;
using TaskStatus = TaskManagement.Api.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository(ApplicationDbContext dbContext) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Tasks.SingleOrDefaultAsync(task => task.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<TaskItem> Items, int TotalCount)> GetPagedAsync(
        TaskStatus? status,
        string? responsible,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tasks.AsNoTracking().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(task => task.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(responsible))
        {
            query = query.Where(task => task.Responsible == responsible);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(task => task.DueDateUtc)
            .ThenBy(task => task.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task AddAsync(TaskItem task, CancellationToken cancellationToken = default) =>
        dbContext.Tasks.AddAsync(task, cancellationToken).AsTask();

    public void Remove(TaskItem task) => dbContext.Tasks.Remove(task);
}
