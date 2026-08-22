using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/tiempo")]
public class TimeController(TimeService service) : ControllerBase
{

    [HttpGet]
    public ActionResult<ServerTimeResponse> Get()
    {
        return Ok(service.GetCurrentTime());
    }

}
