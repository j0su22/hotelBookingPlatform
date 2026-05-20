using Asp.Versioning;
using HotelBookingPlatform.Api.Extensions;
using HotelBookingPlatform.Application.Hotels.Commands;
using HotelBookingPlatform.Application.Hotels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingPlatform.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hotels")]
public sealed class HotelsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "desc",
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetHotelsQuery(pageNumber, pageSize, sortBy, sortDirection), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:guid}", Name = "GetHotelById")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetHotelByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateHotelCommand command, CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, created: true, routeName: "GetHotelById", routeValues: new { id = result.Value });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHotelRequest request, CancellationToken ct = default)
    {
        var command = new UpdateHotelCommand(id, request.Name, request.Address, request.City, request.Country, request.StarRating);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new DeleteHotelCommand(id), ct);
        return result.ToActionResult(this);
    }
}

public sealed record UpdateHotelRequest(string Name, string Address, string City, string Country, int StarRating);
