using Api.DTOs;
using Domain.Entities;

namespace Api.Services;

public static class ItineraryMapper
{
    public static ItineraryResponse ToResponse(
        Itinerary itinerary,
        int reservedPassengers,
        PortTimeConverter portTimeConverter)
    {
        var segments = itinerary.Segments
            .OrderBy(segment => segment.DepartureAtUtc)
            .Select(segment => new SegmentResponse(
                segment.Id,
                segment.DeparturePort.Name,
                segment.DestinationPort.Name,
                segment.DepartureAtUtc,
                portTimeConverter.ToPortLocalTime(
                    segment.DepartureAtUtc,
                    segment.DeparturePort.Timezone),
                segment.ArrivalAtUtc,
                portTimeConverter.ToPortLocalTime(
                    segment.ArrivalAtUtc,
                    segment.DestinationPort.Timezone)))
            .ToList();

        return new ItineraryResponse(
            itinerary.Id,
            itinerary.Boat.Name,
            segments,
            Math.Max(0, itinerary.Boat.Capacity - reservedPassengers),
            itinerary.Boat.Capacity);
    }

    public static ItineraryResponse ToResponseFromCreate(
        Itinerary itinerary,
        Boat boat,
        IReadOnlyDictionary<Guid, Port> portsById,
        int reservedPassengers,
        PortTimeConverter portTimeConverter)
    {
        var segments = itinerary.Segments
            .OrderBy(segment => segment.DepartureAtUtc)
            .Select(segment =>
            {
                var departurePort = portsById[segment.DeparturePortId];
                var destinationPort = portsById[segment.DestinationPortId];

                return new SegmentResponse(
                    segment.Id,
                    departurePort.Name,
                    destinationPort.Name,
                    segment.DepartureAtUtc,
                    portTimeConverter.ToPortLocalTime(
                        segment.DepartureAtUtc,
                        departurePort.Timezone),
                    segment.ArrivalAtUtc,
                    portTimeConverter.ToPortLocalTime(
                        segment.ArrivalAtUtc,
                        destinationPort.Timezone));
            })
            .ToList();

        return new ItineraryResponse(
            itinerary.Id,
            boat.Name,
            segments,
            Math.Max(0, boat.Capacity - reservedPassengers),
            boat.Capacity);
    }
}
