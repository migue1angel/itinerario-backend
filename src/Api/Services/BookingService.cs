
using Api.DTOs;
using Api.Persistence;
using Domain.Entities;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class BookingService(AppDbContext context, TimeProvider timeProvider)
{

    public async Task<Result<BookingResponse>> CreateAsync(Guid itineraryId, CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var itinerary = await context.Itineraries
        .Include(itinerary => itinerary.Boat)
        .Include(itinerary => itinerary.Segments)
        .FirstOrDefaultAsync(itinerary => itinerary.Id == itineraryId, cancellationToken);

        if (itinerary is null)
            return Result.Failure<BookingResponse>(BookingErrors.ItineraryNotFound);

        var firstDepartureUtc = itinerary.Segments.Min(
            segment => segment.DepartureAtUtc);

        if (firstDepartureUtc < timeProvider.GetUtcNow().AddHours(24))
            return Result.Failure<BookingResponse>(BookingErrors.DepartureTooClose);

        var reservedPassengers = await context.Bookings
            .Where(booking => booking.ItineraryId == itineraryId)
            .SumAsync(booking => booking.PassengerCount, cancellationToken);

        var totalReservedPassengers = reservedPassengers + request.PassengerCount;

        if (totalReservedPassengers > itinerary.Boat.Capacity)
            return Result.Failure<BookingResponse>(BookingErrors.CapacityExceeded);

        var bookingResult = Booking.Create(itineraryId, request.PassengerCount);
        if (!bookingResult.IsSuccess)
            return Result.Failure<BookingResponse>(bookingResult.Error);

        context.Bookings.Add(bookingResult.Value);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new BookingResponse(
            bookingResult.Value.Id,
            bookingResult.Value.ItineraryId,
            bookingResult.Value.PassengerCount,
            itinerary.Boat.Capacity - totalReservedPassengers,
            itinerary.Boat.Capacity
        ));
    }

}
