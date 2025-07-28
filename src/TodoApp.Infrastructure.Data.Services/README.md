# TodoApp.Infrastructure.Data.Services

Biblioteca responsável pela integração com serviços externos via HTTP, utilizando Refit para facilitar o consumo de APIs REST.
Centraliza a configuração de clientes HTTP, handlers de autenticação, serviços de acesso externo e abstrações para comunicação entre camadas.

## Funcionalidades
- **Consumo de APIs REST**: Utiliza Refit para facilitar chamadas HTTP tipadas.
- **Autenticação automática**: Adiciona tokens e cabeçalhos personalizados em cada requisição.
- **Extensibilidade**: Permite adicionar novos serviços externos facilmente.
- **Testabilidade**: Serviços desacoplados e facilmente testáveis via mock de HttpClient.

## Estrutura de Pastas e Componentes

### Common
- **BaseApiService**: Classe base para serviços de API, encapsulando o HttpClient.
- **AuthHeaderHandler**: Handler para adicionar cabeçalhos de autenticação (Bearer e x-id-token) nas requisições HTTP, utilizando IUserContext.

### TodoAppExternal
- **ITodoExternalApi**: Interface Refit para consumo da API externa de Todo.
- **TodoExternalDataServices**: Implementação de ITodoExternalDataServices, utilizando ITodoExternalApi para buscar dados externos.

#### Exemplo de Serviço Externo
```csharp
public class TodoExternalDataServices : BaseApiService, ITodoExternalDataServices
{
    private readonly ITodoExternalApi _api;
    public TodoExternalDataServices(HttpClient httpClient) : base(httpClient)
    {
        _api = RestService.For<ITodoExternalApi>(httpClient);
    }
    public async Task<IEnumerable<TodoItemResponse>> GetAllTodoItemsAsync()
    {
        return await _api.GetAllTodoItemsAsync();
    }
}
```

### ServiceCollectionExtensions
Extensão para registrar os serviços de integração, handlers e clientes HTTP no container de DI.
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string todoApiBaseUrl)
    {
        services.AddScoped<ITodoExternalDataServices, TodoExternalDataServices>();
        services.AddScoped<AuthHeaderHandler>();
        services.AddRefitClient<ITodoExternalApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(todoApiBaseUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();
        return services;
    }
}
```

---

## Exemplo de Estrutura de Pastas
```
Infrastructure.Data.Services/
├── Common/
│   ├── BaseApiService.cs
│   └── AuthHeaderHandler.cs
├── TodoAppExternal/
│   └── TodoExternalDataServices.cs
├── ServiceCollectionExtensions.cs
```



