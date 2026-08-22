using Domain.Primitives;

namespace Domain.Entities;

public class Itinerary
{
    private readonly List<Segment> _segments = [];
    private Itinerary() { }

    public Guid Id { get; private set; }

    public Guid BoatId { get; private set; }
    public Boat Boat { get; private set; } = null!;
    public IReadOnlyCollection<Segment> Segments => _segments.AsReadOnly();

    public static Result<Itinerary> Create(Guid boatId)
    {
        return Result.Success(new Itinerary
        {
            Id = Guid.NewGuid(),
            BoatId = boatId,
        });
    }

    public Result AddSegment(Segment segment)
    {
        if (segment.ItineraryId != Id)
            return Result.Failure(ItineraryErrors.SegmentDoesNotBelongToItinerary);

        if(_segments.Count > 0)
        {
            var previousSegment = _segments[^1];

            if(previousSegment.DestinationPortId != segment.DeparturePortId)
                return Result.Failure(ItineraryErrors.DisconnectedRoute);

            var operationalMargin = segment.DepartureAtUtc - previousSegment.ArrivalAtUtc;
            if(operationalMargin < TimeSpan.FromMinutes(45))
                return Result.Failure(ItineraryErrors.InsufficientOperationalMargin);
        }

        _segments.Add(segment);
        return Result.Success();
    }

}
