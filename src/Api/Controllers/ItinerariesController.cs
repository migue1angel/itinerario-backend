using Api.DTOs;
using Api.Services;
using Domain.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/itinerarios")]
public class ItinerariesController(
    ItineraryService itineraryService,
    BookingService bookingService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetByRangeAsync([FromQuery] GetItineraryByRangeRequest request, CancellationToken cancellationToken)
    {
        var result = await itineraryService.GetByRangeAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await itineraryService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : ToErrorResult(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateItineraryRequest request, CancellationToken cancellationToken)
    {
        var result = await itineraryService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
           ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
           : ToErrorResult(result.Error);
    }

    [HttpPost("{itineraryId:guid}/reservas")]
    public async Task<IActionResult> CreateBookingAsync(Guid itineraryId, CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var result = await bookingService.CreateAsync(itineraryId, request, cancellationToken);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : ToErrorResult(result.Error);
    }


    private ObjectResult ToErrorResult(Error error)
    {
        var response = new ApiErrorResponse(
            [
                new ApiError(
                error.Code,
                error.Description,
                error.Field)
            ]);

        return error.Type switch
        {
            ErrorType.NotFound => NotFound(response),
            ErrorType.Conflict => Conflict(response),
            ErrorType.Validation => UnprocessableEntity(response),
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
