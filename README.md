Contexto de Arquitetura de Sistemas usando Clean Architecture em .NET
## Objetivo
Definir uma arquitetura de referência baseada nos princípios da Clean Architecture, aplicada ao ecossistema .NET, visando:

- Alta manutenibilidade e testabilidade

- Baixo acoplamento e alta coesão entre camadas

- Clareza na separação de responsabilidades

- Facilidade de evolução e extensão da aplicação

- Suporte a testes unitários e integração contínua

## Visão Geral da Clean Architecture
A Clean Architecture proposta por Robert C. Martin (Uncle Bob) tem como premissa central a inversão de dependência, onde as regras de negócio não conhecem detalhes de implementação de infraestrutura.

As camadas mais externas podem depender das internas, mas nunca o contrário.

## Camadas da Arquitetura

Para cada projeto da solução possui um arquivo README dedicado, cuidadosamente elaborado para apresentar de forma clara e completa sua estrutura interna. Nestes arquivos, você encontrará:

- A descrição dos padrões adotados

- A explicação das responsabilidades de cada componente

### Core (Domínio e Casos de Uso)

- Define as entidades e interfaces de uso da aplicação.

- Independe de frameworks, bancos de dados ou interfaces de usuário.

- Exemplo: regras de negócio, validações de domínio.

[Acesso a documentação desta camada](https://github.com/aizefler/clean-architecture-dotnet/blob/main/src/TodoApp.Core/README.md)

### Application Services (Casos de Uso Concretos)

- Implementa os casos de uso definidos na camada de Core.

- Orquestra o domínio e delega chamadas para infraestrutura por meio de interfaces.

[Acesso a documentação desta camada](https://github.com/aizefler/clean-architecture-dotnet/blob/main/src/TodoApp.Application/README.md)

### Infrastructure (Infraestrutura)

- Implementações concretas de repositórios, serviços externos, acesso a dados.

- Pode ser dividida em subprojetos, como: .Infrastructure.Data.SqlServer, .Infrastructure.Data.Services, etc.

[Acesso a documentação desta camada - Bus](https://github.com/aizefler/clean-architecture-dotnet/tree/main/src/TodoApp.Infrastructure.Broker.AzureServiceBus/README.md)

[Acesso a documentação desta camada - Data Services](https://github.com/aizefler/clean-architecture-dotnet/tree/main/src/TodoApp.Infrastructure.Data.Services/README.md)

[Acesso a documentação desta camada - SQL Server](https://github.com/aizefler/clean-architecture-dotnet/tree/main/src/TodoApp.Infrastructure.Data.SqlServer/README.md)

### API (Interface de Entrada)

- Projeto de API exposto ao consumidor (REST ou gRPC, por exemplo).

- Deve ser o mais fino possível, apenas roteando requisições para os Application Services.

- Pode usar Minimal APIs, Controllers ou Endpoints dedicados.

[Acesso a documentação desta camada](https://github.com/aizefler/clean-architecture-dotnet/blob/main/src/TodoApp.Api/README.md)

## Organização de Projetos na Solution
Organização Final da Solution com Clean Architecture (.NET)

```
NomeDoProduto.sln
│
├── NomeDoProduto.Api  → Interface pública (API REST/gRPC)
│
├── NomeDoProduto.Core  → Camada de domínio (entidades, regras de negócio, interfaces)
│
├── NomeDoProduto.Application  → Camada de aplicação (casos de uso)
│
├── NomeDoProduto.Infrastructure.Data.Services  → Consumo de APIs externas (ex: Refit, REST, gRPC, SOAP)
│
├── NomeDoProduto.Infrastructure.Broker.AzureServiceBus  → Integração com Azure Service Bus (publisher, listener, eventos)
│
├── NomeDoProduto.Infrastructure.Data.SqlServer  → Acesso a dados com SQL Server (EF Core, Dapper, Unit of Work, Migrations)
│
├── NomeDoProduto.Infrastructure.Data.DbSqlServer  → Projeto DACPAC para versionamento de schema (Database Project)
│
├── NomeDoProduto.Tests.Unit  → Testes unitários (Core + Application)
│
├── NomeDoProduto.Tests.Integration  → Testes de integração (infraestrutura, API, banco, fila etc.)
```

## Referências

Os seguintes projetos e documentações foram usadas como referência para a produção deste template.

- [ardalis - CleanArchitecture](https://github.com/ardalis/CleanArchitecture?tab=readme-ov-file#clean-architecture)

- [Clean Architecture - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2011/11/22/Clean-Architecture.html)

- [Clean Architecture - Microsoft](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
