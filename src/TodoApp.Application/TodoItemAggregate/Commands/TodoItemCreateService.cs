using FluentValidation;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.TodoItemAggregate.Commands.Dtos;
using TodoApp.Application.TodoItemAggregate.Commands.Interfaces;
using TodoApp.Application.TodoItemAggregate.Mappers;
using TodoApp.Core.Common;
using TodoApp.Core.Common.Data;
using TodoApp.Core.Entities;
using TodoApp.Core.Events;

namespace TodoApp.Application.TodoItemAggregate.Commands;

public class TodoItemCreateService : ITodoItemCreateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<TodoItem> _validator;
    private readonly IRepository<TodoList> _repositoryTodoList;
    private readonly IUserRolesContext _userRolesContext;
    private readonly ILogger<TodoItemCreateService> _logger;

    public TodoItemCreateService(IUnitOfWork unitOfWork, 
        IRepository<TodoList> repositoryTodoList,
        IValidator<TodoItem> validator, IUserRolesContext userRolesContext,
        ILogger<TodoItemCreateService> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _repositoryTodoList = repositoryTodoList;
        _userRolesContext = userRolesContext;
        _logger = logger;
    }

    public async Task CreateAsync(TodoItemCreate todoItemCreate, CancellationToken cancellationToken = default)
    {
        if (_userRolesContext.IsInRole(RolesConstants.FeatureA))
        {
            _logger.LogTrace("Usuário autorizado a criar TodoItem.");
            throw new UnauthorizedAccessException(ResultError.UsuarioNaoAutorizado);
        }

        var todoList = await _repositoryTodoList.GetByIdAsync(todoItemCreate.ListId);

        var todoItem = todoItemCreate.ToMap(todoList);

        var validationResult = await _validator.ValidateAsync(todoItem, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogTrace("Erros de validações: {Errors}", errorMessages);
            throw new ValidationException(validationResult.Errors);
        }

        todoItem.AddDomainEvent(new TodoItemCreatedEvent(todoItem));
        _logger.LogInformation("Criado o evento ItemCreated");

        await _unitOfWork.Repository<TodoItem>().AddAsync(todoItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}