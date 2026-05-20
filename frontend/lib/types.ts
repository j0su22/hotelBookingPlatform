export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

export interface HotelDto {
  id: string;
  name: string;
  address: string;
  city: string;
  country: string;
  starRating: number;
  isActive: boolean;
}

export interface RoomTypeDto {
  id: string;
  hotelId: string;
  name: string;
  description: string;
  maxCapacity: number;
  basePrice: number;
}

export interface AvailabilityDto {
  roomTypeId: string;
  roomTypeName: string;
  hotelId: string;
  hotelName: string;
  city: string;
  maxCapacity: number;
  availableRooms: number;
  bestRate: number;
  totalPrice: number;
  nights: number;
}

export type BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled';

export interface BookingDto {
  id: string;
  confirmationNumber: string;
  hotelName: string;
  roomTypeName: string;
  checkIn: string;
  checkOut: string;
  nights: number;
  guestCount: number;
  totalPrice: number;
  status: BookingStatus;
  createdAt: string;
}

export interface BookingDetailDto extends BookingDto {
  guestFirstName: string;
  guestLastName: string;
  guestEmail: string;
  guestPhone: string;
  ratePlanName: string;
}

export interface CreateBookingRequest {
  roomTypeId: string;
  checkIn: string;
  checkOut: string;
  guestCount: number;
  guestFirstName: string;
  guestLastName: string;
  guestEmail: string;
  guestPhone: string;
}

export interface CreateBookingResponse {
  bookingId: string;
  confirmationNumber: string;
}

export interface TokenResponse {
  token: string;
  expiresAt: string;
}

export interface ApiError {
  status: number;
  message: string;
  errors?: string[];
}
