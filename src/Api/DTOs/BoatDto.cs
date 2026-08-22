namespace Api.DTOs;

public record BoatResponse(
    Guid Id,
    string Name,
    Guid BasePortId,
    string BasePortName,
    int Capacity);