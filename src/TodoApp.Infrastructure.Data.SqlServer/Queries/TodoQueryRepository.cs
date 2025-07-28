using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using TodoApp.Application.TodoItemAggregate.Queries;
using TodoApp.Application.TodoItemAggregate.Queries.Dtos;
using TodoApp.Infrastructure.Data.SqlServer.Common.Queries;

namespace TodoApp.Infrastructure.Data.SqlServer.Queries;

[ExcludeFromCodeCoverage]
public class TodoQueryRepository : BaseQueryRepository, ITodoQuery
{
    public TodoQueryRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<TodoListItemDto>> GetAllAsync()
    {
        const string sql = @"
            SELECT 
                ti.Id AS IdItem,
                ti.Title AS TitleItem,
                tl.Id AS IdListItem,
                tl.Title AS TitleListItem,
                ti.Done AS IsCompleted,
                CASE 
                    WHEN ti.Done = 1 THEN ti.UpdatedAt 
                    ELSE NULL 
                END AS CompletedAt
            FROM TodoItems ti
            INNER JOIN TodoLists tl ON ti.ListId = tl.Id
            WHERE ti.Deleted = 0 AND tl.Deleted = 0
            ORDER BY tl.Title, ti.Title";

        return await QueryAsync<TodoListItemDto>(sql);
    }
} 