
using Api.DTOs;
using Api.Validators;

namespace Api.Tests;

public class SegmentTests
{
    [Fact]
    public void CreateSegment_WhenDepartureHasNoOffset_ReturnsValidationError()
    {
        var validator = new CreateSegmentRequestValidator();
        var request = new CreateSegmentRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2026-09-20T08:00:00",
            "2026-09-20T10:00:00");

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error
            => error.PropertyName == nameof(CreateSegmentRequest.DepartureAt) &&
             error.ErrorCode == "FECHA_INVALIDA");
    }

    [Fact]
    public void GetByRange_WhenDesdeHasNoOffset_ReturnsValidationError()
    {
        var validator = new GetItineraryByRangeValidator();

        var request = new GetItineraryByRangeRequest(
            "2026-09-20T08:00:00",
            "2026-09-21T08:00:00-06:00");

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(GetItineraryByRangeRequest.Desde) &&
            error.ErrorCode == "INVALID_DATE_FORMAT");
    }

}
