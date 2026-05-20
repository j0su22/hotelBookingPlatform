using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.RatePlans.Commands;

public sealed record CreateRatePlanCommand(
    Guid RoomTypeId,
    string Name,
    decimal PricePerNight,
    DateOnly ValidFrom,
    DateOnly ValidTo) : IRequest<Result<Guid>>;

public sealed class CreateRatePlanCommandValidator : AbstractValidator<CreateRatePlanCommand>
{
    public CreateRatePlanCommandValidator()
    {
        RuleFor(x => x.RoomTypeId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PricePerNight).GreaterThan(0);
        RuleFor(x => x.ValidFrom).NotEmpty();
        RuleFor(x => x.ValidTo).GreaterThan(x => x.ValidFrom)
            .WithMessage("ValidTo debe ser posterior a ValidFrom.");
    }
}

public sealed class CreateRatePlanCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateRatePlanCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRatePlanCommand request, CancellationToken cancellationToken)
    {
        var roomType = await uow.RoomTypes.GetByIdAsync(request.RoomTypeId, cancellationToken);
        if (roomType is null)
            return Result.Failure<Guid>("Tipo de habitación no encontrado.", "NOT_FOUND");

        var ratePlan = RatePlan.Create(request.RoomTypeId, request.Name, request.PricePerNight, request.ValidFrom, request.ValidTo);
        uow.RatePlans.Add(ratePlan);
        await uow.CommitAsync(cancellationToken);
        return Result.Success(ratePlan.Id);
    }
}
