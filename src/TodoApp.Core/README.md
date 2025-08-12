# TodoApp.Core

Projeto responsável pelo núcleo do domínio e regras de negócio do TodoApp, seguindo os princípios da Clean Architecture.
Centraliza toda a lógica de negócio, abstrações e contratos do domínio, sem dependências de infraestrutura, facilitando testes, manutenção e evolução.
Por não depender de infraestrutura, este projeto é ideal para testes unitários e evolução independente das demais camadas.

## Estrutura de Pastas e Componentes

### Entities
Entidades de negócio, concentrando propriedades e comportamentos referenciados ao ORM na infraestrutura.
Definem regras de negócio e disparam eventos de domínio para notificar mudanças relevantes.

#### Exemplo de Entidade
```csharp   
public class TodoItem : BaseAuditableEntity<int>
{
    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value && !_done)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public TodoList List { get; set; } = null!;

    protected TodoItem() { }

    public TodoItem(string? title, string? note, PriorityLevel priority, DateTime? reminder, TodoList? list)
    {
        Title = title;
        Note = note;
        Priority = priority;
        Reminder = reminder;
        List = list;
    }
}
```

### Common

#### Entities
- **BaseEntity<TKey>**: Entidade base com Id, deleção lógica (soft delete) e gerenciamento de eventos de domínio.
- **BaseAuditableEntity<TKey>**: Herda de BaseEntity, adicionando propriedades de auditoria (datas e usuários de criação/atualização).

#### Data
- **IRepository<T>**: Contrato para repositórios genéricos, com métodos de CRUD e consultas.
- **IUnitOfWork**: Contrato para o padrão Unit of Work, gerenciando repositórios e persistência.

#### Events
- **IDomainEvent**: Interface base para eventos de domínio.
- **DomainEvent**: Implementação base de evento de domínio, com Id, data e tópico.
- **IEventHandler<TEvent>**: Contrato para manipuladores de eventos de domínio.
- **BaseEventHandler<TDomainEvent>**: Implementação base que publica eventos em um barramento.
- **IBusPublisher / IBusBatchPublisher**: Contratos para publicação de eventos em barramento (individual ou em lote).
- **MessageDomainEvent**: Representa eventos de mensagem para integração.
- **IDomainEventDispatcher / DomainEventDispatcher**: Contrato e implementação para despachar eventos de domínio para seus handlers.

#### IUserContext
- **IUserContext**: Interface para abstração do usuário atual, fornecendo informações de identidade e tokens.

### Events
Eventos criados para cada entidade implementada, herdando de DomainEvent, permitindo notificação e processamento desacoplado de mudanças no domínio.

#### Exemplo de Evento
```csharp
public class TodoItemCompletedEvent : DomainEvent
{
    public TodoItem TodoItem { get; }

    public TodoItemCompletedEvent(TodoItem todoItem) : base("TodoItem")
    {
        TodoItem = todoItem ?? throw new ArgumentNullException(nameof(todoItem), "Todo item cannot be null");
    }
} 
```

### Repositories
Interfaces para repositórios específicos, criadas quando o Repository<T> genérico não atende requisitos particulares do domínio.

---

## Exemplo de Estrutura de Pastas
```
Core/
├── Entities/
├── Common/
│   ├── Entities/
│   ├── Data/
│   ├── Events/
│   └── IUserContext.cs
├── Events/
├── Repositories/
```

