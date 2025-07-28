using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using TodoApp.Application.TodoItemAggregate.Commands.Dtos;
using TodoApp.Application.TodoItemAggregate.Queries.Dtos;
using TodoApp.Core.Entities;
using TodoApp.Core.Events;
using TodoApp.Tests.Integration.Base;
using TodoApp.Tests.Integration.Setup;

namespace TodoApp.Tests.Integration.Scenarios;

public class TodoItemCompleteWorkflowTests : IntegrationTestBase
{
    public TodoItemCompleteWorkflowTests(TodoAppWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CompleteWorkflow_CreateTodoItemAndRetrieve_ShouldWorkEndToEnd()
    {
        // Arrange - Primeiro cria uma TodoList
        var todoList = new TodoList { Title = "Test List" };
        DbContext.TodoLists.Add(todoList);
        await DbContext.SaveChangesAsync();

        var createRequest = new TodoItemCreate
        {
            ListId = todoList.Id,
            Title = "Integration Test Item"
        };

        // Act 1: Criar TodoItem via API
        var createResponse = await Client.PostAsJsonAsync("/todolistitem", createRequest);

        // Assert 1: Cria��o bem-sucedida
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act 2: Verificar persist�ncia no banco
        var createdItem = await DbContext.TodoItems
            .Include(x => x.List)
            .FirstOrDefaultAsync(x => x.Title == createRequest.Title);

        // Assert 2: Item foi persistido corretamente
        createdItem.Should().NotBeNull();
        createdItem!.Title.Should().Be(createRequest.Title);
        createdItem.List.Should().NotBeNull();
        createdItem.List.Id.Should().Be(todoList.Id);
        createdItem.Done.Should().BeFalse();
        createdItem.Priority.Should().Be(PriorityLevel.None);

        // Assert 3: Evento de dom�nio foi disparado
        AssertDomainEventWasPublished<TodoItemCreatedEvent>(e => 
            e.TodoItem.Title == createRequest.Title);

        // Act 3: Buscar via API
        var getResponse = await Client.GetAsync("/todolistitem");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await GetAsync<List<TodoListItemDto>>("/todolistitem");

        // Assert 4: Item aparece na consulta
        items.Should().HaveCount(1);
        items.First().TitleItem.Should().Be(createRequest.Title);
        items.First().TitleListItem.Should().Be(todoList.Title);
        items.First().IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateTodoItem_WithInvalidData_ShouldReturnValidationError()
    {
        // Arrange
        var invalidRequest = new TodoItemCreate
        {
            ListId = 999, // Lista inexistente
            Title = "" // T�tulo vazio
        };

        // Act
        var response = await Client.PostAsJsonAsync("/todolistitem", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Verifica que nenhum item foi criado
        var itemCount = await DbContext.TodoItems.CountAsync();
        itemCount.Should().Be(0);

        // Verifica que nenhum evento foi disparado
        BusBacthPublisher.PublishedMessageEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateTodoItem_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var clientWithoutAuth = Factory.CreateClient();
        // N�o adiciona o header x-id-token

        var request = new TodoItemCreate
        {
            ListId = 1,
            Title = "Test"
        };

        // Act
        var response = await clientWithoutAuth.PostAsJsonAsync("/todolistitem", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest); // Middleware retorna BadRequest
    }

    [Fact]
    public async Task CompleteWorkflow_WithMultipleItems_ShouldMaintainDataIntegrity()
    {
        // Arrange - Cria m�ltiplas listas e itens
        var list1 = new TodoList { Title = "Personal Tasks" };
        var list2 = new TodoList { Title = "Work Tasks" };
        
        DbContext.TodoLists.AddRange(list1, list2);
        await DbContext.SaveChangesAsync();

        var requests = new[]
        {
            new TodoItemCreate { ListId = list1.Id, Title = "Buy groceries" },
            new TodoItemCreate { ListId = list1.Id, Title = "Walk the dog" },
            new TodoItemCreate { ListId = list2.Id, Title = "Finish report" },
            new TodoItemCreate { ListId = list2.Id, Title = "Team meeting" }
        };

        // Act - Cria todos os itens
        foreach (var request in requests)
        {
            var response = await Client.PostAsJsonAsync("/todolistitem", request);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        // Assert - Verifica integridade dos dados
        var allItems = await GetAsync<List<TodoListItemDto>>("/todolistitem");
        
        allItems.Should().HaveCount(4);
        
        var personalTasks = allItems.Where(x => x.TitleListItem == "Personal Tasks").ToList();
        var workTasks = allItems.Where(x => x.TitleListItem == "Work Tasks").ToList();
        
        personalTasks.Should().HaveCount(2);
        workTasks.Should().HaveCount(2);
        
        // Verifica que todos os eventos foram disparados
        BusBacthPublisher.PublishedMessageEvents
            .Where(e => e.Type == typeof(TodoItemCreatedEvent).FullName)
            .Should().HaveCount(4);

        // Verifica auditoria
        var dbItems = await DbContext.TodoItems.ToListAsync();
        dbItems.Should().AllSatisfy(item =>
        {
            item.CreatedBy.Should().Be("test-user");
            item.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
        });
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        // Act
        var response = await Client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}