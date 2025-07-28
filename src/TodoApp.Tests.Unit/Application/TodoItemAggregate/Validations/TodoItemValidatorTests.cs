using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using TodoApp.Application.Common;
using TodoApp.Application.TodoItemAggregate.Validations;
using TodoApp.Core.Entities;

namespace TodoApp.Tests.Unit.Application.TodoItemAggregate.Validations;

public class TodoItemValidatorTests
{
    private readonly TodoItemValidator _validator;

    public TodoItemValidatorTests()
    {
        _validator = new TodoItemValidator();
    }

    [Fact]
    public void Validate_WithValidTodoItem_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Valid Title", "Valid Note", PriorityLevel.Medium, DateTime.Now.AddDays(1), todoList);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyTitle_ShouldHaveValidationError(string? title)
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem(title, null, PriorityLevel.Medium, null, todoList);
        var expectedMessage = string.Format(ResultError.CampoObrigatorio, "Title");

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_WithTitleExceedingMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var longTitle = new string('a', 101); // 101 characters
        var todoItem = new TodoItem(longTitle, null, PriorityLevel.Medium, null, todoList);
        var expectedMessage = string.Format(ResultError.CampoMaximoCaracteres, "Title", 100);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_WithTitleAtMaxLength_ShouldNotHaveValidationError()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var maxLengthTitle = new string('a', 100); // Exactly 100 characters
        var todoItem = new TodoItem(maxLengthTitle, null, PriorityLevel.Medium, null, todoList);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(PriorityLevel.None)]
    [InlineData(PriorityLevel.Low)]
    [InlineData(PriorityLevel.Medium)]
    [InlineData(PriorityLevel.High)]
    public void Validate_WithValidPriorityLevel_ShouldNotHaveValidationError(PriorityLevel priority)
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Valid Title", null, priority, null, todoList);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Validate_WithInvalidPriorityLevel_ShouldHaveValidationError()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Valid Title", null, (PriorityLevel)999, null, todoList); // Invalid enum value
        var expectedMessage = string.Format(ResultError.CampoInvalido, "Prioridade");

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Priority)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_WithNullList_ShouldHaveValidationError()
    {
        // Arrange
        var todoItem = new TodoItem("Valid Title", null, PriorityLevel.Medium, null, null!);
        var expectedMessage = string.Format(ResultError.CampoObrigatorio, "Lista");

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.List)
              .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_WithValidList_ShouldNotHaveValidationError()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Valid Title", null, PriorityLevel.Medium, null, todoList);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.List);
    }

    [Fact]
    public void Validate_WithValidNoteAndReminder_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Valid Title", "This is a valid note", PriorityLevel.High, DateTime.Now.AddDays(7), todoList);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNullNoteAndReminder_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Valid Title", null, PriorityLevel.Low, null, todoList);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithMultipleValidationErrors_ShouldHaveAllErrors()
    {
        // Arrange
        var todoItem = new TodoItem("", null, (PriorityLevel)999, null, null!);

        // Act
        var result = _validator.TestValidate(todoItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Priority);
        result.ShouldHaveValidationErrorFor(x => x.List);
    }
}