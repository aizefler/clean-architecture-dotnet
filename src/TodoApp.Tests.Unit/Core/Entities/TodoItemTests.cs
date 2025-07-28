using FluentAssertions;
using TodoApp.Core.Entities;
using TodoApp.Core.Events;

namespace TodoApp.Tests.Unit.Core.Entities;

public class TodoItemTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateTodoItem()
    {
        // Arrange
        var title = "Test Title";
        var note = "Test Note";
        var priority = PriorityLevel.High;
        var reminder = DateTime.Now.AddDays(1);
        var todoList = new TodoList { Title = "Test List" };

        // Act
        var todoItem = new TodoItem(title, note, priority, reminder, todoList);

        // Assert
        todoItem.Title.Should().Be(title);
        todoItem.Note.Should().Be(note);
        todoItem.Priority.Should().Be(priority);
        todoItem.Reminder.Should().Be(reminder);
        todoItem.List.Should().Be(todoList);
        todoItem.Done.Should().BeFalse();
        todoItem.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void SetDone_WhenChangingFromFalseToTrue_ShouldAddCompletedEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test", null, PriorityLevel.Medium, null, todoList);

        // Act
        todoItem.Done = true;

        // Assert
        todoItem.Done.Should().BeTrue();
        todoItem.DomainEvents.Should().HaveCount(1);
        todoItem.DomainEvents.First().Should().BeOfType<TodoItemCompletedEvent>();
        
        var completedEvent = (TodoItemCompletedEvent)todoItem.DomainEvents.First();
        completedEvent.TodoItem.Should().Be(todoItem);
    }

    [Fact]
    public void SetDone_WhenChangingFromTrueToFalse_ShouldNotAddEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test", null, PriorityLevel.Medium, null, todoList);
        todoItem.Done = true; // Set to true first
        todoItem.ClearDomainEvents(); // Clear the first event

        // Act
        todoItem.Done = false;

        // Assert
        todoItem.Done.Should().BeFalse();
        todoItem.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void SetDone_WhenAlreadyTrue_ShouldNotAddAnotherEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test", null, PriorityLevel.Medium, null, todoList);
        todoItem.Done = true; // Set to true first

        // Act
        todoItem.Done = true; // Set to true again

        // Assert
        todoItem.Done.Should().BeTrue();
        todoItem.DomainEvents.Should().HaveCount(1); // Should still have only one event
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToDomainEvents()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test", null, PriorityLevel.Medium, null, todoList);
        var createdEvent = new TodoItemCreatedEvent(todoItem);

        // Act
        todoItem.AddDomainEvent(createdEvent);

        // Assert
        todoItem.DomainEvents.Should().HaveCount(1);
        todoItem.DomainEvents.Should().Contain(createdEvent);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItem = new TodoItem("Test", null, PriorityLevel.Medium, null, todoList);
        todoItem.Done = true; // This adds an event

        // Act
        todoItem.ClearDomainEvents();

        // Assert
        todoItem.DomainEvents.Should().BeEmpty();
    }
}