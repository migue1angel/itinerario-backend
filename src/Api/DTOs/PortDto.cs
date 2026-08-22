namespace Api.DTOs;

public record PortResponse(
    Guid Id,
    string Name,
    string Island,
    string Timezone);
