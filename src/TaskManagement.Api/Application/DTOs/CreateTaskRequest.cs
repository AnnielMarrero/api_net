using TaskStatus = TaskManagement.Api.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Application.DTOs;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskStatus Status,
    string Responsible,
    DateTime DueDateUtc);
