namespace RResult.Api.DomainModels;

public record Todo(int Id, string Name, bool IsComplete)
{
    public static RResult<Todo, ErrT> Ok(Todo todo) =>
    RResult<Todo, ErrT>.Ok(todo);
    public static RResult<Todo, ErrT> Err(ErrT et) =>
        RResult<Todo, ErrT>.Err(et);

}

public readonly record struct TodoDto(string Name, bool IsComplete) { }

public record Tag(int Id, string Name, bool IsComplete) { }
