using FluentValidation;
using TodoApp.Application.Common;
using TodoApp.Core.Entities;

namespace TodoApp.Application.TodoItemAggregate.Validations;

public class TodoItemValidator : AbstractValidator<TodoItem>
{
    public TodoItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(string.Format(ResultError.FieldRequired, "Title"))
            .MaximumLength(100).WithMessage(string.Format(ResultError.FieldMaxLength, "Title", 100));

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage(string.Format(ResultError.FieldInvalid, "Priority"));

        RuleFor(x => x.List)
        .NotNull().WithMessage(string.Format(ResultError.FieldRequired, "List"));
    }
}