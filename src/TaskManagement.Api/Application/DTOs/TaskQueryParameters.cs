using TaskStatus = TaskManagement.Api.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Application.DTOs;

public sealed class TaskQueryParameters
{
    public TaskStatus? Status { get; init; }

    public string? Responsible { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
