using Domain.Primitives;

namespace Domain.Entities;

public class Segment
{
    private Segment() { }
    public Guid Id { get; private set; }

    public Guid ItineraryId { get; private set; }
    public Itinerary Itinerary { get; private set; } = null!;
    public Guid DeparturePortId { get; private set; }
    public Port DeparturePort { get; private set; } = null!;
    public Guid DestinationPortId { get; private set; }
    public Port DestinationPort { get; private set; } = null!;
    public DateTimeOffset DepartureAtUtc { get; private set; }
    public DateTimeOffset ArrivalAtUtc { get; private set; }

    public static Result<Segment> Create(Guid itineraryId,
     Guid departurePortId,
     Guid destinationPortId,
     DateTimeOffset departureAtUtc,
     DateTimeOffset arrivalAtUtc)
    {
        var departureUtc = departureAtUtc.ToUniversalTime();
        var arrivalUtc = arrivalAtUtc.ToUniversalTime();

        if (arrivalUtc <= departureUtc)
            return Result.Failure<Segment>(SegmentErrors.InvalidArrivalTime);
            
        var duration = arrivalUtc - departureUtc;

        if (duration < TimeSpan.FromMinutes(30) || duration > TimeSpan.FromHours(18))
            return Result.Failure<Segment>(SegmentErrors.InvalidSegmentDuration);

        if (departurePortId == destinationPortId)
            return Result.Failure<Segment>(
                SegmentErrors.SameDepartureAndDestinationPort);

        return Result.Success(new Segment
        {
            Id = Guid.NewGuid(),
            ItineraryId = itineraryId,
            DeparturePortId = departurePortId,
            DestinationPortId = destinationPortId,
            DepartureAtUtc = departureUtc,
            ArrivalAtUtc = arrivalUtc
        });
    }
}
