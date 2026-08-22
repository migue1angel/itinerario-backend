using Domain.Primitives;

namespace Domain.Entities;

public class Booking
{
    private Booking() { }

    public Guid Id { get; private set; }

    public Guid ItineraryId { get; private set; }
    public int PassengerCount { get; private set; }

    public static Result<Booking> Create(
        Guid itineraryId,
        int passengerCount)
    {
        if (passengerCount <= 0)
            return Result.Failure<Booking>(
                BookingErrors.InvalidPassengerCount);

        return Result.Success(new Booking
        {
            Id = Guid.NewGuid(),
            ItineraryId = itineraryId,
            PassengerCount = passengerCount
        });
    }
}