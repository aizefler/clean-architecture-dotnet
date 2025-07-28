using FluentValidation;
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

    public TodoItemCreateService(IUnitOfWork unitOfWork, 
        IRepository<TodoList> repositoryTodoList,
        IValidator<TodoItem> validator, IUserRolesContext userRolesContext)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _repositoryTodoList = repositoryTodoList;
        _userRolesContext = userRolesContext;
    }

    public async Task CreateAsync(TodoItemCreate todoItemCreate, CancellationToken cancellationToken = default)
    {
        if (_userRolesContext.IsInRole(RolesConstants.FeatureA))
            throw new UnauthorizedAccessException(ResultError.UsuarioNaoAutorizado);

        var todoList = await _repositoryTodoList.GetByIdAsync(todoItemCreate.ListId);

        var todoItem = todoItemCreate.ToMap(todoList);

        var validationResult = await _validator.ValidateAsync(todoItem, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        todoItem.AddDomainEvent(new TodoItemCreatedEvent(todoItem));

        await _unitOfWork.Repository<TodoItem>().AddAsync(todoItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}