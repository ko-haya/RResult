namespace RResult.Api.DomainModels;

public record Todo(int? Id, string? Name, bool IsComplete = false) { }

public readonly record struct TodoDto(string? Name, bool IsComplete) { }

public record Tag(int Id, string? Name, bool IsComplete) { }
