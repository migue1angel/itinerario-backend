using Api.DTOs;

namespace Api.Services;

public sealed class TimeService(TimeProvider timeProvider)
{
    public ServerTimeResponse GetCurrentTime()
    {
        return new ServerTimeResponse(
            timeProvider.GetUtcNow());
    }
}