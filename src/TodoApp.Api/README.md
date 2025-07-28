# TodoApp.Api

Projeto responsável pela exposição da API HTTP do TodoApp, utilizando ASP.NET Core (.NET 9). Centraliza a configuração dos módulos, middlewares, endpoints, OpenAPI/Swagger, Application Insights, autenticação, CORS e integração com as camadas de aplicação e infraestrutura.

## Funcionalidades
- **Exposição de endpoints REST**: Utiliza Minimal APIs para rotas e módulos.
- **Documentação automática**: OpenAPI/Swagger integrado.
- **Monitoramento**: Application Insights para telemetria.
- **Health Checks**: Endpoint de verificação de saúde.
- **Middlewares customizados**: Autenticação, tratamento de exceções, CORS.
- **Integração total**: Consome serviços de Application, Infrastructure, Broker e Core via DI.

## Estrutura de Pastas e Componentes

### Program.cs
Configuração principal da aplicação, DI, middlewares, endpoints, OpenAPI, health checks e registro dos módulos.
```csharp
var builder = WebApplication.CreateBuilder(args);
// ... configurações ...
builder.Services.AddApplicationAggregates();
builder.Services.AddDataServices(...);
builder.Services.AddSqlServerInfrastructure(...);
builder.Services.AddAzureServiceBusBroker(...);
// ...
var app = builder.Build();
app.UseMiddleware<AuthHeaderValidationsMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapHealthChecks("/health");
app.RegisterModules();
app.Run();
```

### Modules
Organiza os endpoints em módulos agrupados por contexto de negócio.
#### Exemplo de Módulo
```csharp
public class TodoAppModule : IRegisterModule
{
    public void RegisterModule(WebApplication app)
    {
        var group = app.MapGroup("/todolistitem").WithTags("ListItem").WithOpenApi();
        group.MapGet("", ListarTodoListItems);
        group.MapPost("", CriarTodoItem);
    }
    public static async Task<IResult> ListarTodoListItems([FromServices] ITodoQuery todoQuery, CancellationToken cancellationToken = default)
    {
        var result = await todoQuery.GetAllAsync();
        return Results.Ok(result);
    }
    public static async Task<IResult> CriarTodoItem([FromBody] TodoItemCreate todoItemCreate, [FromServices] ITodoItemCreateService todoItemCreateService, CancellationToken cancellationToken = default)
    {
        await todoItemCreateService.CreateAsync(todoItemCreate, cancellationToken);
        return Results.Created($"/todolistitem", null);
    }
}
```

### Common
- **Middlewares**: Autenticação, tratamento de exceções, etc.
- **Configs**: Configurações customizadas de AppSettings, Telemetria, HealthCheck.

#### Detalhamento dos Middlewares Customizados

- **AuthHeaderValidationsMiddleware**: Responsável por validar os cabeçalhos de autenticação das requisições HTTP. Garante que os tokens de autenticação (ex: Bearer, x-id-token) estejam presentes e válidos antes de permitir o processamento da requisição. Caso estejam ausentes ou inválidos, retorna erro 401 Unauthorized, protegendo os endpoints contra acessos não autorizados.

- **ExceptionHandlingMiddleware**: Responsável por capturar e tratar exceções não previstas durante o processamento das requisições. Centraliza o tratamento de erros, garantindo respostas padronizadas ao cliente em caso de falhas, registrando logs e retornando respostas HTTP apropriadas (ex: 500 Internal Server Error). Evita vazamento de detalhes internos e facilita o diagnóstico de problemas.

### Integração
- **Application**: Serviços de caso de uso, validação, comandos e queries.
- **Infrastructure**: Serviços de dados, brokers, repositórios, etc.
- **Core**: Contratos e entidades de domínio.

---

## Exemplo de Estrutura de Pastas
```
Api/
├── Program.cs
├── Modules/
│   └── TodoAppModule.cs
├── Common/
│   ├── Middlewares/
│   └── Configs/
```
