
namespace Api.DTOs;


public record GetItineraryByRangeRequest(
    string? Desde,
    string? Hasta
);
public record CreateItineraryRequest
(
    Guid BoatId,
    List<CreateSegmentRequest> Segments
);

public record CreateSegmentRequest(
    Guid DeparturePortId,
    Guid DestinationPortId,
    string DepartureAt,
    string ArrivalAt
);

public record ItineraryResponse(
    Guid Id,
    string Boat,
    List<SegmentResponse> Segments,
    int AvailableSeats,
    int TotalSeats
);

public record SegmentResponse(
    Guid Id,
    string DeparturePort,
    string DestinationPort,
    DateTimeOffset DepartureAtUtc,
    DateTimeOffset DepartureAtLocal,
    DateTimeOffset ArrivalAtUtc,
    DateTimeOffset ArrivalAtLocal
);

