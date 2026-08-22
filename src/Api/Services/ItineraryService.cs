using System.Globalization;
using Api.DTOs;
using Api.Persistence;
using Domain.Entities;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class ItineraryService(AppDbContext context, PortTimeConverter portTimeConverter)
{
    public async Task<List<ItineraryResponse>> GetByRangeAsync(GetItineraryByRangeRequest request, CancellationToken cancellationToken)
    {
        DateTimeOffset? fromUtc = request.Desde is null
            ? null
            : DateTimeOffset.Parse(request.Desde, CultureInfo.InvariantCulture).ToUniversalTime();

        DateTimeOffset? toUtc = request.Hasta is null
            ? null
            : DateTimeOffset.Parse(request.Hasta, CultureInfo.InvariantCulture).ToUniversalTime();

        var query = context.Itineraries
            .AsNoTracking()
            .Include(i => i.Boat)
            .Include(i => i.Segments).ThenInclude(s => s.DeparturePort)
            .Include(i => i.Segments).ThenInclude(s => s.DestinationPort)
            .AsQueryable();

        if (fromUtc is not null)
            query = query.Where(itinerary =>
                itinerary.Segments.Min(segment => segment.DepartureAtUtc) >= fromUtc.Value);

        if (toUtc is not null)
            query = query.Where(itinerary =>
                itinerary.Segments.Max(segment => segment.ArrivalAtUtc) <= toUtc.Value);

        var itineraries = await query
            .OrderBy(itinerary => itinerary.Segments.Min(segment => segment.DepartureAtUtc))
            .ToListAsync(cancellationToken);

        var reservedPassengersByItinerary =
            await GetReservedPassengersByItineraryAsync(
                itineraries.Select(itinerary => itinerary.Id),
                cancellationToken);

        return itineraries
            .Select(itinerary => ItineraryMapper.ToResponse(
                itinerary,
                reservedPassengersByItinerary.GetValueOrDefault(itinerary.Id),
                portTimeConverter))
            .ToList();
    }

    public async Task<Result<ItineraryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var itinerary = await context.Itineraries
            .AsNoTracking()
            .Include(i => i.Boat)
            .Include(i => i.Segments).ThenInclude(s => s.DeparturePort)
            .Include(i => i.Segments).ThenInclude(s => s.DestinationPort)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (itinerary is null)
            return Result.Failure<ItineraryResponse>(ItineraryErrors.NotFound);

        var reservedPassengersByItinerary =
            await GetReservedPassengersByItineraryAsync([itinerary.Id], cancellationToken);

        return Result.Success(ItineraryMapper.ToResponse(
            itinerary,
            reservedPassengersByItinerary.GetValueOrDefault(itinerary.Id),
            portTimeConverter));
    }

    public async Task<Result<ItineraryResponse>> CreateAsync(CreateItineraryRequest request, CancellationToken cancellationToken)
    {
        var boat = await context.Boats.FindAsync([request.BoatId], cancellationToken);
        if (boat is null)
            return Result.Failure<ItineraryResponse>(BoatErrors.NotFound);

        var orderedSegments = request.Segments
            .Select(segment => new
            {
                segment.DeparturePortId,
                segment.DestinationPortId,
                DepartureAt = DateTimeOffset.Parse(segment.DepartureAt, CultureInfo.InvariantCulture),
                ArrivalAt = DateTimeOffset.Parse(segment.ArrivalAt, CultureInfo.InvariantCulture)
            })
            .OrderBy(segment => segment.DepartureAt)
            .ToList();

        var portIds = orderedSegments
            .SelectMany(segment => new[] { segment.DeparturePortId, segment.DestinationPortId })
            .ToHashSet();

        var portsById = await context.Ports
            .AsNoTracking()
            .Where(port => portIds.Contains(port.Id))
            .ToDictionaryAsync(port => port.Id, cancellationToken);

        if (portsById.Count != portIds.Count)
            return Result.Failure<ItineraryResponse>(PortErrors.NotFound);

        foreach (var segment in orderedSegments)
        {
            var departurePort = portsById[segment.DeparturePortId];
            var destinationPort = portsById[segment.DestinationPortId];

            if (!portTimeConverter.HasMatchingOffset(segment.DepartureAt, departurePort.Timezone))
                return Result.Failure<ItineraryResponse>(SegmentErrors.InvalidDepartureOffset);

            if (!portTimeConverter.HasMatchingOffset(segment.ArrivalAt, destinationPort.Timezone))
                return Result.Failure<ItineraryResponse>(SegmentErrors.InvalidArrivalOffset);

            if (!portTimeConverter.IsWithinOperatingWindow(segment.DepartureAt, departurePort.Timezone))
                return Result.Failure<ItineraryResponse>(SegmentErrors.DepartureOutsideOperatingWindow(departurePort.Name));
        }

        var itineraryResult = Itinerary.Create(boat.Id);
        if (!itineraryResult.IsSuccess)
            return Result.Failure<ItineraryResponse>(itineraryResult.Error);

        var itinerary = itineraryResult.Value;
        foreach (var segment in orderedSegments)
        {
            var segmentResult = Segment.Create(
                itinerary.Id,
                segment.DeparturePortId,
                segment.DestinationPortId,
                segment.DepartureAt,
                segment.ArrivalAt);

            if (!segmentResult.IsSuccess)
                return Result.Failure<ItineraryResponse>(segmentResult.Error);

            var addSegmentResult = itinerary.AddSegment(segmentResult.Value);
            if (!addSegmentResult.IsSuccess)
                return Result.Failure<ItineraryResponse>(addSegmentResult.Error);
        }

        var firstDepartureUtc = itinerary.Segments.Min(segment => segment.DepartureAtUtc);
        var lastArrivalUtc = itinerary.Segments.Max(segment => segment.ArrivalAtUtc);

        var existingItineraryRanges = await context.Segments
            .AsNoTracking()
            .Where(segment => segment.Itinerary.BoatId == itinerary.BoatId)
            .GroupBy(segment => segment.ItineraryId)
            .Select(group => new
            {
                FirstDepartureUtc = group.Min(segment => segment.DepartureAtUtc),
                LastArrivalUtc = group.Max(segment => segment.ArrivalAtUtc)
            })
            .ToListAsync(cancellationToken);

        var operationalMargin = TimeSpan.FromMinutes(45);

        foreach (var existing in existingItineraryRanges)
        {
            var isOverlapping = existing.FirstDepartureUtc < lastArrivalUtc &&
                                 existing.LastArrivalUtc > firstDepartureUtc;

            if (isOverlapping)
                return Result.Failure<ItineraryResponse>(BoatErrors.OverlappingSegments);

            var gap = existing.FirstDepartureUtc >= lastArrivalUtc
                ? existing.FirstDepartureUtc - lastArrivalUtc
                : firstDepartureUtc - existing.LastArrivalUtc;

            if (gap < operationalMargin)
                return Result.Failure<ItineraryResponse>(ItineraryErrors.InsufficientOperationalMargin);
        }

        context.Itineraries.Add(itinerary);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(
            ItineraryMapper.ToResponseFromCreate(
                itinerary,
                boat,
                portsById,
                reservedPassengers: 0,
                portTimeConverter: portTimeConverter));
    }

    private async Task<IReadOnlyDictionary<Guid, int>>
        GetReservedPassengersByItineraryAsync(
            IEnumerable<Guid> itineraryIds,
            CancellationToken cancellationToken)
    {
        var ids = itineraryIds.Distinct().ToList();

        return await context.Bookings
            .AsNoTracking()
            .Where(booking => ids.Contains(booking.ItineraryId))
            .GroupBy(booking => booking.ItineraryId)
            .ToDictionaryAsync(
                group => group.Key,
                group => group.Sum(booking => booking.PassengerCount),
                cancellationToken);
    }
}
