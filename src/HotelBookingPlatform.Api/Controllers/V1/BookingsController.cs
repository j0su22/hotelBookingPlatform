using Asp.Versioning;
using HotelBookingPlatform.Api.Extensions;
using HotelBookingPlatform.Application.Bookings.Commands;
using HotelBookingPlatform.Application.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingPlatform.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/bookings")]
public sealed class BookingsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] string? guestEmail = null,
        [FromQuery] Guid? hotelId = null,
        [FromQuery] DateOnly? checkInFrom = null,
        [FromQuery] DateOnly? checkInTo = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "desc",
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetBookingsQuery(status, guestEmail, hotelId, checkInFrom, checkInTo, pageNumber, pageSize, sortBy, sortDirection), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:guid}", Name = "GetBookingById")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetBookingByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken ct = default)
    {
        var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        var command = new CreateBookingCommand(
            idempotencyKey,
            request.RoomTypeId,
            request.CheckIn,
            request.CheckOut,
            request.GuestCount,
            request.GuestFirstName,
            request.GuestLastName,
            request.GuestEmail,
            request.GuestPhone);

        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, created: true, routeName: "GetBookingById", routeValues: new { id = result.Value?.BookingId });
    }

    [HttpPut("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ConfirmBookingCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new CancelBookingCommand(id), ct);
        return result.ToActionResult(this);
    }
}

public sealed record CreateBookingRequest(
    Guid RoomTypeId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int GuestCount,
    string GuestFirstName,
    string GuestLastName,
    string GuestEmail,
    string GuestPhone);
