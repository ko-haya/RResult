namespace RResult.Api.DomainModels;

public record Todo(int Id, string Name = "", bool IsComplete = default) { }

public readonly record struct TodoDto(string Name = "", bool IsComplete = default) { }

public record Tag(int Id, string Name = "", bool IsComplete = default) { }
