using TaskManagement.Api.Application.DTOs;
using TaskManagement.Api.Application.Exceptions;
using TaskManagement.Api.Application.Repositories;
using TaskManagement.Api.Domain.Entities;

namespace TaskManagement.Api.Application.Services;

public sealed class TaskService(
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork) : ITaskService
{
    public async Task<TaskResponse> CreateAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = new TaskItem(
            request.Title.Trim(),
            request.Description?.Trim(),
            request.Status,
            request.Responsible.Trim(),
            request.DueDateUtc);

        await taskRepository.AddAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(task);
    }

    public async Task<PagedResponse<TaskResponse>> GetPagedAsync(
        TaskQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var result = await taskRepository.GetPagedAsync(
            query.Status,
            query.Responsible?.Trim(),
            query.Page,
            query.PageSize,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)query.PageSize);
        var items = result.Items.Select(ToResponse).ToArray();

        return new PagedResponse<TaskResponse>(
            items,
            query.Page,
            query.PageSize,
            result.TotalCount,
            totalPages);
    }

    public async Task<TaskResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);

        return ToResponse(task);
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);

        task.Update(
            request.Title.Trim(),
            request.Description?.Trim(),
            request.Status,
            request.Responsible.Trim(),
            request.DueDateUtc);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);

        taskRepository.Remove(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static TaskResponse ToResponse(TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status,
        task.Responsible,
        task.DueDateUtc,
        task.CreatedAtUtc,
        task.UpdatedAtUtc);
}
