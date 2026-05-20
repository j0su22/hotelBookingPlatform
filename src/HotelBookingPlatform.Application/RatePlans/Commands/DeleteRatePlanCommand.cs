using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.RatePlans.Commands;

public sealed record DeleteRatePlanCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteRatePlanCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteRatePlanCommand, Result>
{
    public async Task<Result> Handle(DeleteRatePlanCommand request, CancellationToken cancellationToken)
    {
        var ratePlan = await uow.RatePlans.GetByIdAsync(request.Id, cancellationToken);
        if (ratePlan is null)
            return Result.Failure("Plan de tarifas no encontrado.", "NOT_FOUND");

        ratePlan.Deactivate();
        uow.RatePlans.Update(ratePlan);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
