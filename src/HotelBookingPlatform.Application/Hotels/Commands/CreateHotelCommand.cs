using FluentValidation;
using HotelBookingPlatform.Domain.Common;
using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Interfaces;
using MediatR;

namespace HotelBookingPlatform.Application.Hotels.Commands;

public sealed record CreateHotelCommand(
    string Name,
    string Address,
    string City,
    string Country,
    int StarRating) : IRequest<Result<Guid>>;

public sealed class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
{
    public CreateHotelCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StarRating).InclusiveBetween(1, 5);
    }
}

public sealed class CreateHotelCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateHotelCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = Hotel.Create(request.Name, request.Address, request.City, request.Country, request.StarRating);
        uow.Hotels.Add(hotel);
        await uow.CommitAsync(cancellationToken);
        return Result.Success(hotel.Id);
    }
}
