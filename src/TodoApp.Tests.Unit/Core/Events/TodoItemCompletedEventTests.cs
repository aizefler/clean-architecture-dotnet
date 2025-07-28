using FluentAssertions;
using TodoApp.Core.Events;
using TodoApp.Core.Entities;

namespace TodoApp.Tests.Unit.Core.Events;

public class TodoItemCompletedEventTests
{
    [Fact]
    public void Constructor_WithValidTodoItem_ShouldCreateEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test Item", null, PriorityLevel.High, null, todoList);

        // Act
        var completedEvent = new TodoItemCompletedEvent(todoItem);

        // Assert
        completedEvent.TodoItem.Should().Be(todoItem);
        completedEvent.Topic.Should().Be("TodoItem");
        completedEvent.Id.Should().NotBeEmpty();
        completedEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithNullTodoItem_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new TodoItemCompletedEvent(null!);
        
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("todoItem")
            .WithMessage("Todo item cannot be null*");
    }

    [Fact]
    public void Constructor_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test Item", null, PriorityLevel.High, null, todoList);

        // Act
        var completedEvent = new TodoItemCompletedEvent(todoItem);

        // Assert
        completedEvent.Should().BeAssignableTo<TodoApp.Core.Common.Events.DomainEvent>();
    }

    [Fact]
    public void Constructor_WithCompletedTodoItem_ShouldPreserveState()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Completed Item", "Some notes", PriorityLevel.Low, DateTime.Now.AddDays(1), todoList);
        todoItem.Done = true;

        // Act
        var completedEvent = new TodoItemCompletedEvent(todoItem);

        // Assert
        completedEvent.TodoItem.Should().Be(todoItem);
        completedEvent.TodoItem.Done.Should().BeTrue();
        completedEvent.TodoItem.Title.Should().Be("Completed Item");
        completedEvent.TodoItem.Note.Should().Be("Some notes");
        completedEvent.TodoItem.Priority.Should().Be(PriorityLevel.Low);
    }

    [Theory]
    [InlineData(PriorityLevel.None)]
    [InlineData(PriorityLevel.Low)]
    [InlineData(PriorityLevel.Medium)]
    [InlineData(PriorityLevel.High)]
    public void Constructor_WithDifferentPriorities_ShouldPreservePriority(PriorityLevel priority)
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test Item", null, priority, null, todoList);

        // Act
        var completedEvent = new TodoItemCompletedEvent(todoItem);

        // Assert
        completedEvent.TodoItem.Priority.Should().Be(priority);
    }
}