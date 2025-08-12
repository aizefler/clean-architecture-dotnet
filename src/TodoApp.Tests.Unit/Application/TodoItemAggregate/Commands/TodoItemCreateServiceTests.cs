using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Application.Common;
using TodoApp.Application.TodoItemAggregate.Commands;
using TodoApp.Application.TodoItemAggregate.Commands.Dtos;
using TodoApp.Application.TodoItemAggregate.Commands.Interfaces;
using TodoApp.Core.Common;
using TodoApp.Core.Common.Data;
using TodoApp.Core.Entities;
using TodoApp.Core.Events;

namespace TodoApp.Tests.Unit.Application.TodoItemAggregate.Commands;

public class TodoItemCreateServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IValidator<TodoItem>> _mockValidator;
    private readonly Mock<IRepository<TodoList>> _mockTodoListRepository;
    private readonly Mock<IRepository<TodoItem>> _mockTodoItemRepository;
    private readonly Mock<IUserRolesContext> _mockUserRolesContext;
    private readonly Mock<ILogger<TodoItemCreateService>> _mockLogger;
    private readonly TodoItemCreateService _service;

    public TodoItemCreateServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockValidator = new Mock<IValidator<TodoItem>>();
        _mockTodoListRepository = new Mock<IRepository<TodoList>>();
        _mockTodoItemRepository = new Mock<IRepository<TodoItem>>();
        _mockUserRolesContext = new Mock<IUserRolesContext>();
        _mockLogger = new Mock<ILogger<TodoItemCreateService>>();
        
        _mockUnitOfWork.Setup(u => u.Repository<TodoItem>())
                      .Returns(_mockTodoItemRepository.Object);

        // Setup user roles context to allow creation by default
        _mockUserRolesContext.Setup(u => u.IsInRole(RolesConstants.User))
                            .Returns(true);

        _service = new TodoItemCreateService(
            _mockUnitOfWork.Object,
            _mockTodoListRepository.Object,
            _mockValidator.Object,
            _mockUserRolesContext.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateTodoItemSuccessfully()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItemCreate = new TodoItemCreate(1, "Test Item");
        var validationResult = new ValidationResult();

        _mockTodoListRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(todoList);
        
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);

        _mockTodoItemRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                              .ReturnsAsync((TodoItem item) => item);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);

        // Act
        await _service.CreateAsync(todoItemCreate);

        // Assert
        _mockTodoListRepository.Verify(r => r.GetByIdAsync(todoItemCreate.ListId), Times.Once);
        _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockTodoItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(t => 
            t.Title == todoItemCreate.Title && 
            t.List == todoList &&
            t.DomainEvents.Any(e => e is TodoItemCreatedEvent))), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidationErrors_ShouldThrowValidationException()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItemCreate = new TodoItemCreate(1, "");
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Title", "Title is required")
        };
        var validationResult = new ValidationResult(validationErrors);

        _mockTodoListRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(todoList);
        
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);

        // Act & Assert
        var action = async () => await _service.CreateAsync(todoItemCreate);
        
        await action.Should().ThrowAsync<ValidationException>()
                   .WithMessage("*Title is required*");

        _mockTodoItemRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNullTodoList_ShouldStillProcessRequest()
    {
        // Arrange
        var todoItemCreate = new TodoItemCreate(999, "Test Item");
        var validationResult = new ValidationResult();

        _mockTodoListRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync((TodoList?)null);
        
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);

        _mockTodoItemRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                              .ReturnsAsync((TodoItem item) => item);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);

        // Act
        await _service.CreateAsync(todoItemCreate);

        // Assert
        _mockTodoListRepository.Verify(r => r.GetByIdAsync(todoItemCreate.ListId), Times.Once);
        _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockTodoItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(t => 
            t.Title == todoItemCreate.Title && 
            t.List == null)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddTodoItemCreatedEvent()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItemCreate = new TodoItemCreate(1, "Test Item");
        var validationResult = new ValidationResult();
        TodoItem? capturedTodoItem = null;

        _mockTodoListRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(todoList);
        
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);

        _mockTodoItemRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                              .Callback<TodoItem>(item => capturedTodoItem = item)
                              .ReturnsAsync((TodoItem item) => item);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);

        // Act
        await _service.CreateAsync(todoItemCreate);

        // Assert
        capturedTodoItem.Should().NotBeNull();
        capturedTodoItem!.DomainEvents.Should().HaveCount(1);
        capturedTodoItem.DomainEvents.First().Should().BeOfType<TodoItemCreatedEvent>();
        
        var createdEvent = (TodoItemCreatedEvent)capturedTodoItem.DomainEvents.First();
        createdEvent.TodoItem.Should().Be(capturedTodoItem);
    }

    [Fact]
    public async Task CreateAsync_WithCancellationToken_ShouldPassTokenToMethods()
    {
        // Arrange
        var todoList = new TodoList { Title = "Test List" };
        var todoItemCreate = new TodoItemCreate(1, "Test Item");
        var validationResult = new ValidationResult();
        var cancellationToken = new CancellationToken();

        _mockTodoListRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(todoList);
        
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validationResult);

        _mockTodoItemRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                              .ReturnsAsync((TodoItem item) => item);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);

        // Act
        await _service.CreateAsync(todoItemCreate, cancellationToken);

        // Assert
        _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<TodoItem>(), cancellationToken), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(cancellationToken), Times.Once);
    }
}