using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Domain.Entities;

public sealed class Guest : Entity
{
    private Guest() { }

    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;

    public static Guest Create(string firstName, string lastName, string email, string phoneNumber)
    {
        return new Guest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email.ToLowerInvariant(),
            PhoneNumber = phoneNumber
        };
    }

    public void Update(string firstName, string lastName, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        SetUpdatedAt();
    }
}
