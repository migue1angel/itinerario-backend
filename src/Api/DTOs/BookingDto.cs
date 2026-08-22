namespace Api.DTOs;

public record CreateBookingRequest(
    int PassengerCount
);

public record BookingResponse(
    Guid Id,
    Guid ItineraryId,
    int PassengerCount,
    int AvailableSeats,
    int TotalSeats
);