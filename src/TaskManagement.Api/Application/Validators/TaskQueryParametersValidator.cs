using FluentValidation;
using TaskManagement.Api.Application.DTOs;

namespace TaskManagement.Api.Application.Validators;

public sealed class TaskQueryParametersValidator : AbstractValidator<TaskQueryParameters>
{
    public TaskQueryParametersValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);
        RuleFor(query => query.Responsible)
            .MaximumLength(150);
    }
}
