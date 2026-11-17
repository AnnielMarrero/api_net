using TaskStatus = TaskManagement.Api.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Domain.Entities;

public sealed class TaskItem
{
    private TaskItem()
    {
    }

    public TaskItem(
        string title,
        string? description,
        TaskStatus status,
        string responsible,
        DateTime dueDateUtc)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Status = status;
        Responsible = responsible;
        DueDateUtc = dueDateUtc;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public TaskStatus Status { get; private set; }

    public string Responsible { get; private set; } = string.Empty;

    public DateTime DueDateUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(
        string title,
        string? description,
        TaskStatus status,
        string responsible,
        DateTime dueDateUtc)
    {
        Title = title;
        Description = description;
        Status = status;
        Responsible = responsible;
        DueDateUtc = dueDateUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
