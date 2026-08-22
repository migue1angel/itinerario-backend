
using Api.DTOs;
using Api.Persistence;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class PortService(AppDbContext context)
{
    public async Task<Result<List<PortResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var ports = await context.Ports
        .AsNoTracking()
        .Select(port => new PortResponse(
            port.Id,
            port.Name,
            port.Island,
            port.Timezone))
        .ToListAsync(cancellationToken);

        return Result.Success(ports);
    }


}
