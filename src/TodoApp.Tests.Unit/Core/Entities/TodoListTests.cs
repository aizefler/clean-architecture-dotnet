using FluentAssertions;
using TodoApp.Core.Entities;
using System.Drawing;

namespace TodoApp.Tests.Unit.Core.Entities;

public class TodoListTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var todoList = new TodoList();

        // Assert
        todoList.Title.Should().BeNull();
        todoList.Colour.Should().Be(Color.White);
        todoList.Items.Should().NotBeNull();
        todoList.Items.Should().BeEmpty();
        todoList.DomainEvents.Should().BeEmpty();
        todoList.Deleted.Should().BeFalse();
    }

    [Fact]
    public void SetTitle_ShouldUpdateTitle()
    {
        // Arrange
        var todoList = new TodoList();
        var title = "My Todo List";

        // Act
        todoList.Title = title;

        // Assert
        todoList.Title.Should().Be(title);
    }

    [Fact]
    public void SetColour_ShouldUpdateColour()
    {
        // Arrange
        var todoList = new TodoList();
        var color = Color.Blue;

        // Act
        todoList.Colour = color;

        // Assert
        todoList.Colour.Should().Be(color);
    }

    [Fact]
    public void Items_ShouldBeReadOnlyButModifiable()
    {
        // Arrange
        var todoList = new TodoList();
        var todoItem = new TodoItem("Test Item", null, PriorityLevel.Low, null, todoList);

        // Act
        todoList.Items.Add(todoItem);

        // Assert
        todoList.Items.Should().HaveCount(1);
        todoList.Items.Should().Contain(todoItem);
    }

    [Fact]
    public void Items_ShouldAllowMultipleItems()
    {
        // Arrange
        var todoList = new TodoList();
        var item1 = new TodoItem("Item 1", null, PriorityLevel.Low, null, todoList);
        var item2 = new TodoItem("Item 2", null, PriorityLevel.High, null, todoList);

        // Act
        todoList.Items.Add(item1);
        todoList.Items.Add(item2);

        // Assert
        todoList.Items.Should().HaveCount(2);
        todoList.Items.Should().Contain(item1);
        todoList.Items.Should().Contain(item2);
    }

    [Fact]
    public void Items_ShouldAllowRemoval()
    {
        // Arrange
        var todoList = new TodoList();
        var todoItem = new TodoItem("Test Item", null, PriorityLevel.Low, null, todoList);
        todoList.Items.Add(todoItem);

        // Act
        todoList.Items.Remove(todoItem);

        // Assert
        todoList.Items.Should().BeEmpty();
    }
}