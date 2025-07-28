# TodoApp.Application

Projeto responsável pela lógica de aplicação, orquestrando casos de uso, validações, mapeamentos e integração entre o domínio e as demais camadas da arquitetura.

Centraliza serviços, comandos, consultas, autorizações, validações e mapeamentos, promovendo separação de responsabilidades e facilitando testes.

Os serviços de aplicação são facilmente testáveis, pois dependem apenas de contratos do domínio e abstrações de infraestrutura.

## Estrutura de Pastas e Componentes

### TodoItemAggregate
Agrupamento de funcionalidades relacionadas à entidade TodoItem.

- **Commands**: Serviços e DTOs para operações de escrita (ex: criação de TodoItem).
- **Queries**: Interfaces e DTOs para operações de leitura.
- **Validations**: Validadores para regras de negócio e integridade dos dados.
- **Mappers**: Extensões para conversão entre DTOs e entidades.
- **EventHandlers**: Manipuladores de eventos de domínio disparados pelo Core.
- **DataServices**: Interfaces e DTOs para integração com serviços externos.

#### Exemplo de Serviço de Criação
```csharp
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
```

#### Exemplo de DTO de Criação
```csharp
public class TodoItemCreate
{
    public int ListId { get; set; }
    public string Title { get; set; }
}
```

#### Exemplo de Validação
```csharp
public class TodoItemValidator : AbstractValidator<TodoItem>
{
    public TodoItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(string.Format(ResultError.CampoObrigatorio, "Title"))
            .MaximumLength(100).WithMessage(string.Format(ResultError.CampoMaximoCaracteres, "Title", 100));

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage(string.Format(ResultError.CampoInvalido, "Prioridade"));

        RuleFor(x => x.List)
        .NotNull().WithMessage(string.Format(ResultError.CampoObrigatorio, "Lista"));
    }
}
```

#### Exemplo de Manipulador de Evento
```csharp
public class TodoItemCreatedEventHandler : BaseEventHandler<TodoItemCreatedEvent>
{
    public TodoItemCreatedEventHandler(IBusPublisher busPublisher) : base(busPublisher) { }
}
```

### Common
- **Dtos**: Objetos de transferência de dados comuns (ex: Result, PaginatedList, Lookup).
- **ErrorMessages**: Mensagens de erro padronizadas para validações e respostas.

### ServiceCollectionExtensions
Extensão para registrar os serviços de aplicação no container de DI.
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationAggregates(this IServiceCollection services)
    {
        services.AddScoped<ITodoItemCreateService, TodoItemCreateService>();
        services.AddScoped<IValidator<TodoItem>, TodoItemValidator>();
        return services;
    }
}
```

---

## Exemplo de Estrutura de Pastas
```
Application/
├── TodoItemAggregate/
│   ├── Commands/
│   ├── Queries/
│   ├── Validations/
│   ├── Mappers/
│   ├── EventHandlers/
│   └── DataServices/
├── Common/
│   ├── Dtos/
│   └── ErrorMessages.cs
├── ServiceCollectionExtensions.cs
```

