using TaskManagement.Api.Domain.Entities;
using TaskStatus = TaskManagement.Api.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Application.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<TaskItem> Items, int TotalCount)> GetPagedAsync(
        TaskStatus? status,
        string? responsible,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    void Remove(TaskItem task);
}
