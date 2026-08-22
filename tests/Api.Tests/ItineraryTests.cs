

using Api.DTOs;
using Api.Services;
using Domain.Entities;

namespace Api.Tests;

public class ItineraryTests
{
    [Fact]
    public void AddSegment_WhenOperationalMarginIsLessThan45Mins_ReturnsFailure()
    {
        var itinerary = Itinerary.Create(Guid.NewGuid()).Value;

        var departurePortId = Guid.NewGuid();
        var firstDestinationPortId = Guid.NewGuid();
        var secondDestinationPortId = Guid.NewGuid();
        var firstDeparture = new DateTimeOffset(2026, 9, 20, 8, 0, 0, TimeSpan.Zero);

        var firstSegment = Segment.Create(
            itinerary.Id,
            departurePortId,
            firstDestinationPortId,
            firstDeparture,
            firstDeparture.AddHours(2)).Value;

        var secondDeparture = firstSegment.ArrivalAtUtc.AddMinutes(40);

        var secondSegment = Segment.Create(
            itinerary.Id,
            firstDestinationPortId,
            secondDestinationPortId,
            secondDeparture,
            secondDeparture.AddHours(2)).Value;

        var firstAddResult = itinerary.AddSegment(firstSegment);
        var secondAddResult = itinerary.AddSegment(secondSegment);

        Assert.True(firstAddResult.IsSuccess);
        Assert.False(secondAddResult.IsSuccess);
        Assert.Equal("Itinerary.InsufficientOperationalMargin", secondAddResult.Error.Code);
    }

    [Theory]
    [InlineData(
    "2026-09-20T05:59:00-06:00",
    "2026-09-20T08:00:00-06:00")]
    [InlineData(
    "2026-09-20T18:01:00-06:00",
    "2026-09-20T20:00:00-06:00")]
    public async Task CreateItinerary_WhenDepartureOrArrivalAreOutsideOperatingWindow_ReturnsFailure(
    string departureAt,
    string arrivalAt)
    {
        await using var context = TestHelpers.CreateContext();

        var departurePort = new Port
        {
            Id = Guid.NewGuid(),
            Name = "Puerto A",
            Island = "Isla A",
            Timezone = "Pacific/Galapagos"
        };

        var firstDestinationPort = new Port
        {
            Id = Guid.NewGuid(),
            Name = "Puerto B",
            Island = "Isla B",
            Timezone = "Pacific/Galapagos"
        };

        var secondDestinationPort = new Port
        {
            Id = Guid.NewGuid(),
            Name = "Puerto C",
            Island = "Isla C",
            Timezone = "Pacific/Galapagos"
        };

        var boat = new Boat
        {
            Id = Guid.NewGuid(),
            Name = "Test Boat",
            BasePortId = departurePort.Id,
            Capacity = 16
        };

        context.AddRange(
            departurePort,
            firstDestinationPort,
            secondDestinationPort,
            boat);

        await context.SaveChangesAsync();

        var request = new CreateItineraryRequest(
            boat.Id,
            new List<CreateSegmentRequest>
            {
            new(
                departurePort.Id,
                firstDestinationPort.Id,
                departureAt,
                arrivalAt),
            new(
                firstDestinationPort.Id,
                secondDestinationPort.Id,
                "2026-09-21T08:45:00-06:00",
                "2026-09-21T11:00:00-06:00")
            });

        var service = new ItineraryService(
            context,
            new PortTimeConverter());

        var result = await service.CreateAsync(
            request,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Segment.DepartureOutsideOperatingWindow",
            result.Error.Code);
    }
}
