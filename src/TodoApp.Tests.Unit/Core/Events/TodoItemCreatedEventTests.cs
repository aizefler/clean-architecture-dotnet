using FluentAssertions;
using TodoApp.Core.Events;
using TodoApp.Core.Entities;

namespace TodoApp.Tests.Unit.Core.Events;

public class TodoItemCreatedEventTests
{
    [Fact]
    public void Constructor_WithValidTodoItem_ShouldCreateEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test Item", null, PriorityLevel.Medium, null, todoList);

        // Act
        var createdEvent = new TodoItemCreatedEvent(todoItem);

        // Assert
        createdEvent.TodoItem.Should().Be(todoItem);
        createdEvent.Topic.Should().Be("TodoItem");
        createdEvent.Id.Should().NotBeEmpty();
        createdEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithNullTodoItem_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new TodoItemCreatedEvent(null!);
        
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("todoItem")
            .WithMessage("Todo item cannot be null*");
    }

    [Fact]
    public void Constructor_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test Item", null, PriorityLevel.Medium, null, todoList);

        // Act
        var createdEvent = new TodoItemCreatedEvent(todoItem);

        // Assert
        createdEvent.Should().BeAssignableTo<TodoApp.Core.Common.Events.DomainEvent>();
    }

    [Theory]
    [InlineData("Test Item 1")]
    [InlineData("Another Item")]
    [InlineData("Special Characters! @#$")]
    public void Constructor_WithDifferentTitles_ShouldPreserveTodoItem(string title)
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem(title, null, PriorityLevel.Medium, null, todoList);

        // Act
        var createdEvent = new TodoItemCreatedEvent(todoItem);

        // Assert
        createdEvent.TodoItem.Should().Be(todoItem);
        createdEvent.TodoItem.Title.Should().Be(title);
    }
}