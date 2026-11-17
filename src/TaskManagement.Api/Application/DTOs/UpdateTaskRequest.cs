using TaskStatus = TaskManagement.Api.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Application.DTOs;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskStatus Status,
    string Responsible,
    DateTime DueDateUtc);
