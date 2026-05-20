using MediatR;

namespace HotelBookingPlatform.Domain.Common;

/// <summary>
/// Marker interface for domain events. Extends INotification so MediatR can
/// publish them without any Infrastructure-layer wrapper.
/// </summary>
public interface IDomainEvent : INotification;
