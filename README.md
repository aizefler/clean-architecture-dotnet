Contexto de Arquitetura de Sistemas usando Clean Architecture em .NET
## Objetivo
Definir uma arquitetura de referência baseada nos princípios da Clean Architecture, aplicada ao ecossistema .NET, visando:

Alta manutenibilidade e testabilidade

Baixo acoplamento e alta coesão entre camadas

Clareza na separação de responsabilidades

Facilidade de evolução e extensão da aplicação

Suporte a testes unitários e integração contínua

## Visão Geral da Clean Architecture
A Clean Architecture proposta por Robert C. Martin (Uncle Bob) tem como premissa central a inversão de dependência, onde as regras de negócio não conhecem detalhes de implementação de infraestrutura.

As camadas mais externas podem depender das internas, mas nunca o contrário.

## Camadas da Arquitetura

### Core (Domínio e Casos de Uso)

Define as entidades e interfaces de uso da aplicação.

Independe de frameworks, bancos de dados ou interfaces de usuário.

Exemplo: regras de negócio, validações de domínio.

### Application Services (Casos de Uso Concretos)

Implementa os casos de uso definidos na camada de Core.

Orquestra o domínio e delega chamadas para infraestrutura por meio de interfaces.

### Infrastructure (Infraestrutura)

Implementações concretas de repositórios, serviços externos, acesso a dados.

Pode ser dividida em subprojetos, como: .Infrastructure.Data.SqlServer, .Infrastructure.Data.Services, etc.

### API (Interface de Entrada)

Projeto de API exposto ao consumidor (REST ou gRPC, por exemplo).

Deve ser o mais fino possível, apenas roteando requisições para os Application Services.

Pode usar Minimal APIs, Controllers ou Endpoints dedicados.

## Organização de Projetos na Solution
Organização Final da Solution com Clean Architecture (.NET)

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

## Documentação detalhada
Cada projeto da solução possui um arquivo README dedicado, cuidadosamente elaborado para apresentar de forma clara e completa sua estrutura interna. Nestes arquivos, você encontrará:

A descrição dos padrões adotados

A explicação das responsabilidades de cada componente

Exemplos práticos de classes e suas utilizações

Orientações sobre como reutilizar e expandir os recursos existentes

Essa documentação tem como objetivo garantir entendimento técnico aprofundado e acelerar a produtividade dos desenvolvedores envolvidos.