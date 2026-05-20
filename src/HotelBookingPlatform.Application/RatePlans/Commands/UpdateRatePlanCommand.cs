using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.RatePlans.Commands;

public sealed record UpdateRatePlanCommand(
    Guid Id,
    string Name,
    decimal PricePerNight,
    DateOnly ValidFrom,
    DateOnly ValidTo) : IRequest<Result>;

public sealed class UpdateRatePlanCommandValidator : AbstractValidator<UpdateRatePlanCommand>
{
    public UpdateRatePlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PricePerNight).GreaterThan(0);
        RuleFor(x => x.ValidTo).GreaterThan(x => x.ValidFrom);
    }
}

public sealed class UpdateRatePlanCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateRatePlanCommand, Result>
{
    public async Task<Result> Handle(UpdateRatePlanCommand request, CancellationToken cancellationToken)
    {
        var ratePlan = await uow.RatePlans.GetByIdAsync(request.Id, cancellationToken);
        if (ratePlan is null)
            return Result.Failure("Plan de tarifas no encontrado.", "NOT_FOUND");

        ratePlan.Update(request.Name, request.PricePerNight, request.ValidFrom, request.ValidTo);
        uow.RatePlans.Update(ratePlan);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
