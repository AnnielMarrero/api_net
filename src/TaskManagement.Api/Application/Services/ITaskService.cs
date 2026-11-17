using TaskManagement.Api.Application.DTOs;

namespace TaskManagement.Api.Application.Services;

public interface ITaskService
{
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);

    Task<PagedResponse<TaskResponse>> GetPagedAsync(
        TaskQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
