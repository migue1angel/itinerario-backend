
using Api.DTOs;
using Api.Services;
using Domain.Entities;
using Microsoft.Extensions.Time.Testing;

namespace Api.Tests;

public class BookingTests
{
    [Fact]
    public async Task CreateBooking_WhenDepartureIsLessThan24HoursAway_ReturnsFailure()
    {
        await using var context = TestHelpers.CreateContext();

        var nowUtc = new DateTimeOffset(
            2026, 8, 20, 12, 0, 0, TimeSpan.Zero);

        var timeProvider = new FakeTimeProvider(nowUtc);

        var departurePort = new Port
        {
            Id = Guid.NewGuid(),
            Name = "Puerto A",
            Island = "Isla A",
            Timezone = "Pacific/Galapagos"
        };

        var destinationPort = new Port
        {
            Id = Guid.NewGuid(),
            Name = "Puerto B",
            Island = "Isla B",
            Timezone = "Pacific/Galapagos"
        };

        var boat = new Boat
        {
            Id = Guid.NewGuid(),
            Name = "Test Boat",
            BasePortId = departurePort.Id,
            Capacity = 16
        };

        var itinerary = Itinerary.Create(boat.Id).Value;

        var departureUtc = nowUtc.AddHours(23);

        var segment = Segment.Create(
            itinerary.Id,
            departurePort.Id,
            destinationPort.Id,
            departureUtc,
            departureUtc.AddHours(2)).Value;

        itinerary.AddSegment(segment);

        context.AddRange(
            departurePort,
            destinationPort,
            boat,
            itinerary);

        await context.SaveChangesAsync();

        var service = new BookingService(context, timeProvider);

        var result = await service.CreateAsync(
            itinerary.Id,
            new CreateBookingRequest(2),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Booking.DepartureTooClose",
            result.Error.Code);
    }
}
