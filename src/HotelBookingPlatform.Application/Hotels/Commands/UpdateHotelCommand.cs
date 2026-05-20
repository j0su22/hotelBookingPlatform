using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.Hotels.Commands;

public sealed record UpdateHotelCommand(
    Guid Id,
    string Name,
    string Address,
    string City,
    string Country,
    int StarRating) : IRequest<Result>;

public sealed class UpdateHotelCommandValidator : AbstractValidator<UpdateHotelCommand>
{
    public UpdateHotelCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StarRating).InclusiveBetween(1, 5);
    }
}

public sealed class UpdateHotelCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateHotelCommand, Result>
{
    public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await uow.Hotels.GetByIdAsync(request.Id, cancellationToken);
        if (hotel is null)
            return Result.Failure("Hotel no encontrado.", "NOT_FOUND");

        hotel.Update(request.Name, request.Address, request.City, request.Country, request.StarRating);
        uow.Hotels.Update(hotel);
        await uow.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
