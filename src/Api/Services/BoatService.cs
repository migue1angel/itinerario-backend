
using Api.DTOs;
using Api.Persistence;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class BoatService(AppDbContext context)
{
    public async Task<Result<List<BoatResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var boats = await context.Boats
            .AsNoTracking()
            .Select(boat => new BoatResponse(
                boat.Id,
                boat.Name,
                boat.BasePortId,
                boat.BasePort.Name,
                boat.Capacity))
            .ToListAsync(cancellationToken);

        return Result.Success(boats);
    }


}
