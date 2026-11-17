using FluentValidation;
using TaskManagement.Api.Application.DTOs;

namespace TaskManagement.Api.Application.Validators;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(request => request.Description)
            .MaximumLength(2000);
        RuleFor(request => request.Responsible)
            .NotEmpty()
            .MaximumLength(150);
        RuleFor(request => request.DueDateUtc)
            .Must(date => date.Kind == DateTimeKind.Utc)
            .WithMessage("DueDateUtc must be a UTC date.");
        RuleFor(request => request.Status)
            .IsInEnum();
    }
}
