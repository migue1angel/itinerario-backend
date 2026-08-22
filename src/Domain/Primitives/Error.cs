namespace Domain.Primitives;

public enum ErrorType
{
    Failure,
    Validation,
    Conflict,
    NotFound
}

public sealed record Error(
    string Code,
    string Description,
    ErrorType Type,
    string? Field = null)
{
    public static readonly Error None =
        new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Validation(
        string code,
        string description,
        string? field = null) =>
        new(code, description, ErrorType.Validation, field);

    public static Error Conflict(
        string code,
        string description,
        string? field = null) =>
        new(code, description, ErrorType.Conflict, field);

    public static Error NotFound(
        string code,
        string description,
        string? field = null) =>
        new(code, description, ErrorType.NotFound, field);
}