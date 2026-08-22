namespace Api.DTOs;

public record ApiError(
    string Code,
    string Message,
    string? Field = null);

public record ApiErrorResponse(
    List<ApiError> Errors);