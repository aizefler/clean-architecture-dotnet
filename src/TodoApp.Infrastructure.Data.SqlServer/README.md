# TodoApp.Infrastructure.Data.SqlServer

Biblioteca responsável pela implementação do acesso a dados via Entity Framework Core e Dapper, utilizando SQL Server como banco de dados.
Centraliza a configuração do DbContext, mapeamentos, interceptors, repositórios, Unit of Work, serviços de auditoria e publicação de eventos de domínio.

## Funcionalidades      
- **Auditoria automática**: Preenchimento de campos CreatedAt, UpdatedAt, CreatedBy, UpdatedBy.
- **Soft Delete**: Exclusão lógica das entidades.
- **Domain Events**: Publicação automática de eventos de domínio e integração.
- **Consultas otimizadas**: Dapper para queries complexas e de alta performance.
- **Retry e Logging**: Configuração de retry e logging detalhado em ambiente de desenvolvimento.

## Estrutura de Pastas e Componentes

### AppDbContext
Contexto principal do Entity Framework Core, gerenciando entidades, interceptors e mapeamentos.
```csharp  
public class AppDbContext : DbContext
{
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<MessageDomainEvent> MessageDomainEvents => Set<MessageDomainEvent>();
    // ... interceptors e configurações ...
}
```

### ModelMapping
Configurações de mapeamento das entidades para o banco de dados.
- **BaseEntityTypeConfiguration / BaseAuditableEntityTypeConfiguration**: Configuração padrão para entidades base e auditáveis.
- **TodoListTypeConfiguration / TodoItemTypeConfiguration**: Mapeamento específico para TodoList e TodoItem.
- **Exemplo de Mapeamento**:
```csharp   
public class TodoItemTypeConfiguration : BaseAuditableEntityTypeConfiguration<TodoItem>
{
    public override void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems");
        builder.Property(e => e.Title).HasMaxLength(255);
        builder.Property(e => e.Priority).IsRequired().HasConversion<int>();
        // ... outros mapeamentos ...
    }
}
```
### Common
- **Repositories**: Implementação dos padrões Repository e Unit of Work.
- **DomainEvents**: Interceptors para publicação de eventos de domínio e mensagens.
- **Auditable**: Interceptor para preenchimento automático de campos de auditoria.
- **Queries**: Repositórios de consulta otimizados com Dapper.

### Queries
Repositórios de consulta (read-only) usando Dapper para operações otimizadas.

```csharp 
public class TodoQueryRepository : BaseQueryRepository, ITodoQuery
{
    public async Task<IEnumerable<TodoListItemDto>> GetAllAsync()
    {
        const string sql = "...";
        return await QueryAsync<TodoListItemDto>(sql);
    }
}
```

### ServiceCollectionExtensions
Extensão para registrar todos os serviços, interceptors, repositórios e DbContext no container de DI.
```csharp 
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(...);
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        // ... outros serviços ...
        return services;
    }
}
```
---

## Exemplo de Estrutura de PastasInfrastructure.Data.SqlServer/
├── AppDbContext.cs
├── ServiceCollectionExtensions.cs
├── ModelMapping/
│   ├── BaseEntityTypeConfiguration.cs
│   ├── TodoListTypeConfiguration.cs
│   └── TodoItemTypeConfiguration.cs
├── Common/
│   ├── Repositories/
│   ├── DomainEvents/
│   ├── Auditable/
│   └── Queries/
├── Queries/
│   └── TodoQueryRepository.cs
---

