namespace TaskManagement.Api.Application.Exceptions;

public sealed class TaskNotFoundException(Guid id)
    : Exception($"Task '{id}' was not found.");
