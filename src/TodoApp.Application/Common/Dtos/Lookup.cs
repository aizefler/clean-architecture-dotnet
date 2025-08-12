namespace TodoApp.Application.Common.Dtos;

public record Lookup<TKey>(TKey Id, string Title);