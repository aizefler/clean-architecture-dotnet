# Query Repositories com Dapper

Esta pasta contém os repositórios de consulta (readonly) usando Dapper para operações de leitura otimizadas.

## Estrutura

### BaseQueryRepository
Classe base abstrata que fornece métodos utilitários para consultas Dapper:

- **QueryAsync<T>**: Consulta que retorna uma coleção de objetos
- **QueryFirstOrDefaultAsync<T>**: Consulta que retorna o primeiro resultado ou null
- **QueryFirstAsync<T>**: Consulta que retorna o primeiro resultado (lança exceção se não encontrar)
- **QuerySingleAsync<T>**: Consulta que retorna um único resultado (lança exceção se não encontrar ou encontrar mais de um)
- **QuerySingleOrDefaultAsync<T>**: Consulta que retorna um único resultado ou null
- **ExecuteAsync**: Executa comandos que não retornam dados
- **QueryMultipleAsync**: Executa múltiplas consultas em uma única execução
- **QueryAsync com mapeamento**: Consultas com mapeamento de múltiplas entidades (até 7 entidades)

### TodoQueryRepository
Implementação específica da interface `ITodoQuery`:

- **GetAllAsync()**: Retorna todos os itens de todo com suas respectivas listas

## Como Usar

### 1. Criar um novo Query Repository

```csharp
public class CustomQueryRepository : BaseQueryRepository, ICustomQuery
{
    public CustomQueryRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<CustomDto>> GetCustomDataAsync()
    {
        const string sql = @"
            SELECT 
                Id,
                Name,
                CreatedAt
            FROM CustomTable
            WHERE Deleted = 0
            ORDER BY CreatedAt DESC";

        return await QueryAsync<CustomDto>(sql);
    }
}
```

### 2. Consultas com Parâmetros

```csharp
public async Task<CustomDto?> GetByIdAsync(int id)
{
    const string sql = @"
        SELECT 
            Id,
            Name,
            CreatedAt
        FROM CustomTable
        WHERE Id = @Id AND Deleted = 0";

    var parameters = new { Id = id };
    return await QueryFirstOrDefaultAsync<CustomDto>(sql, parameters);
}
```

### 3. Consultas com Mapeamento de Múltiplas Entidades

```csharp
public async Task<IEnumerable<ComplexDto>> GetComplexDataAsync()
{
    const string sql = @"
        SELECT 
            u.Id, u.Name, u.Email,
            p.Id, p.Title, p.Content
        FROM Users u
        LEFT JOIN Posts p ON u.Id = p.UserId
        WHERE u.Deleted = 0";

    return await QueryAsync<User, Post, ComplexDto>(sql, (user, post) => 
        new ComplexDto 
        { 
            UserId = user.Id, 
            UserName = user.Name, 
            PostTitle = post?.Title 
        });
}
```

### 4. Consultas com Múltiplos Resultados

```csharp
public async Task<MultiResultDto> GetMultipleResultsAsync(int userId)
{
    const string sql = @"
        SELECT Id, Name FROM Users WHERE Id = @UserId;
        SELECT Id, Title FROM Posts WHERE UserId = @UserId;
        SELECT COUNT(*) FROM Comments WHERE UserId = @UserId";

    using var multi = await QueryMultipleAsync(sql, new { UserId = userId });
    
    return new MultiResultDto
    {
        User = await multi.ReadFirstOrDefaultAsync<User>(),
        Posts = await multi.ReadAsync<Post>(),
        CommentCount = await multi.ReadFirstAsync<int>()
    };
}
```

### 5. Registrar no DI Container

```csharp
// No ServiceCollectionExtensions
services.AddScoped<ICustomQuery, CustomQueryRepository>();
```

## Vantagens do Dapper

1. **Performance**: Micro-ORM leve e rápido
2. **Controle**: Controle total sobre as consultas SQL
3. **Flexibilidade**: Suporte a consultas complexas e stored procedures
4. **Mapeamento**: Mapeamento automático de objetos
5. **Transações**: Suporte completo a transações
6. **Bulk Operations**: Operações em lote otimizadas

## Boas Práticas

### 1. SQL Constantes
```csharp
private const string GetAllSql = @"
    SELECT Id, Name, CreatedAt 
    FROM Table 
    WHERE Deleted = 0 
    ORDER BY CreatedAt DESC";
```

### 2. Parâmetros Tipados
```csharp
var parameters = new { Id = id, Name = name };
return await QueryAsync<Dto>(sql, parameters);
```

### 3. Tratamento de Erros
```csharp
public async Task<Result<CustomDto>> GetByIdAsync(int id)
{
    try
    {
        var result = await QueryFirstOrDefaultAsync<CustomDto>(sql, new { Id = id });
        return Result<CustomDto>.Success(result);
    }
    catch (Exception ex)
    {
        return Result<CustomDto>.Failure(ex.Message);
    }
}
```

### 4. Paginação
```csharp
public async Task<IEnumerable<CustomDto>> GetPagedAsync(int page, int pageSize)
{
    const string sql = @"
        SELECT Id, Name, CreatedAt
        FROM CustomTable
        WHERE Deleted = 0
        ORDER BY CreatedAt DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

    var parameters = new { Offset = (page - 1) * pageSize, PageSize = pageSize };
    return await QueryAsync<CustomDto>(sql, parameters);
}
```

## Exemplo de Implementação Completa

```csharp
[ExcludeFromCodeCoverage]
public class UserQueryRepository : BaseQueryRepository, IUserQuery
{
    private const string GetAllSql = @"
        SELECT Id, Name, Email, CreatedAt
        FROM Users
        WHERE Deleted = 0
        ORDER BY Name";

    private const string GetByIdSql = @"
        SELECT Id, Name, Email, CreatedAt
        FROM Users
        WHERE Id = @Id AND Deleted = 0";

    private const string GetWithPostsSql = @"
        SELECT 
            u.Id, u.Name, u.Email,
            p.Id, p.Title, p.Content, p.CreatedAt
        FROM Users u
        LEFT JOIN Posts p ON u.Id = p.UserId AND p.Deleted = 0
        WHERE u.Id = @Id AND u.Deleted = 0
        ORDER BY p.CreatedAt DESC";

    public UserQueryRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await QueryAsync<UserDto>(GetAllSql);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        return await QueryFirstOrDefaultAsync<UserDto>(GetByIdSql, new { Id = id });
    }

    public async Task<UserWithPostsDto?> GetWithPostsAsync(int id)
    {
        return await QueryAsync<User, Post, UserWithPostsDto>(
            GetWithPostsSql, 
            (user, post) => new UserWithPostsDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Posts = post != null ? new List<PostDto> { new PostDto { Id = post.Id, Title = post.Title } } : new List<PostDto>()
            }, 
            new { Id = id });
    }
}
``` 