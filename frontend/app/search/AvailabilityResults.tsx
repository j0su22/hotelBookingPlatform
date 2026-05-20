'use client';

import { useState, useTransition } from 'react';
import { useRouter } from 'next/navigation';
import type { AvailabilityDto, CreateBookingRequest, CreateBookingResponse, HotelDto } from '@/lib/types';
import { api } from '@/lib/api';
import EmptyState from '@/components/states/EmptyState';
import ErrorMessage from '@/components/states/ErrorMessage';

interface BookingFormState {
  roomTypeId: string;
  roomTypeName: string;
  totalPrice: number;
}

export default function AvailabilityResults({
  results,
  checkIn,
  checkOut,
  guests,
  hotels,
}: {
  results: AvailabilityDto[];
  checkIn: string;
  checkOut: string;
  guests: string;
  hotels: HotelDto[];
}) {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();
  const [booking, setBooking] = useState<BookingFormState | null>(null);
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', phone: '' });
  const [error, setError] = useState<string | null>(null);
  const [selectedHotel, setSelectedHotel] = useState('');

  const filtered = selectedHotel ? results.filter((r) => r.hotelId === selectedHotel) : results;

  function set(field: string, value: string) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  async function handleBook(e: React.FormEvent) {
    e.preventDefault();
    if (!booking) return;
    setError(null);

    const req: CreateBookingRequest = {
      roomTypeId: booking.roomTypeId,
      checkIn,
      checkOut,
      guestCount: Number(guests),
      guestFirstName: form.firstName,
      guestLastName: form.lastName,
      guestEmail: form.email,
      guestPhone: form.phone,
    };

    try {
      const res = await api.post<CreateBookingResponse>('/api/v1/bookings', req);
      startTransition(() => router.push(`/bookings/${res.bookingId}`));
    } catch (err: unknown) {
      const e = err as { message?: string; status?: number };
      if (e.status === 409) setError('No rooms available for those dates. Please try different dates.');
      else setError(e.message ?? 'Booking failed. Please try again.');
    }
  }

  const fmt = (d: string) => new Date(d + 'T00:00:00').toLocaleDateString('en-US', { month: 'short', day: 'numeric' });

  if (results.length === 0) {
    return <EmptyState title="No rooms available" description="Try different dates or adjust your guest count." />;
  }

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between flex-wrap gap-3">
        <h2 className="text-xl font-bold text-gray-800">
          {filtered.length} room{filtered.length !== 1 ? 's' : ''} available
          <span className="ml-2 text-sm font-normal text-gray-500">
            {fmt(checkIn)} – {fmt(checkOut)} · {guests} guest{Number(guests) !== 1 ? 's' : ''}
          </span>
        </h2>

        {hotels.length > 0 && (
          <select
            value={selectedHotel}
            onChange={(e) => setSelectedHotel(e.target.value)}
            className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All hotels</option>
            {hotels.map((h) => (
              <option key={h.id} value={h.id}>{h.name}</option>
            ))}
          </select>
        )}
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {filtered.map((room) => (
          <div key={room.roomTypeId} className="rounded-xl border border-gray-200 bg-white shadow-sm p-5 flex flex-col gap-3">
            <div>
              <p className="font-semibold text-gray-900">{room.roomTypeName}</p>
              <p className="text-sm text-blue-600">{room.hotelName}</p>
              <p className="text-xs text-gray-400">{room.city}</p>
            </div>

            <div className="flex flex-wrap gap-x-4 gap-y-1 text-xs text-gray-600">
              <span>Max {room.maxCapacity} guests</span>
              <span>{room.availableRooms} room{room.availableRooms !== 1 ? 's' : ''} left</span>
              <span>{room.nights} night{room.nights !== 1 ? 's' : ''}</span>
            </div>

            <div className="flex items-end justify-between">
              <div>
                <span className="text-2xl font-bold text-blue-700">${room.totalPrice.toFixed(0)}</span>
                <span className="text-xs text-gray-400 ml-1">total</span>
                <p className="text-xs text-gray-400">${room.bestRate}/night</p>
              </div>
              <button
                onClick={() => { setBooking({ roomTypeId: room.roomTypeId, roomTypeName: room.roomTypeName, totalPrice: room.totalPrice }); setError(null); }}
                className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 transition"
              >
                Book
              </button>
            </div>
          </div>
        ))}
      </div>

      {booking && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-md p-6 space-y-5">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-bold">Complete Your Booking</h3>
              <button onClick={() => setBooking(null)} className="text-gray-400 hover:text-gray-600 text-xl leading-none">&times;</button>
            </div>

            <div className="rounded-lg bg-blue-50 p-3 text-sm">
              <p className="font-medium text-blue-900">{booking.roomTypeName}</p>
              <p className="text-blue-700">{fmt(checkIn)} – {fmt(checkOut)} · ${booking.totalPrice.toFixed(2)}</p>
            </div>

            {error && <ErrorMessage message={error} />}

            <form onSubmit={handleBook} className="space-y-3">
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">First name</label>
                  <input value={form.firstName} onChange={(e) => set('firstName', e.target.value)} required
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">Last name</label>
                  <input value={form.lastName} onChange={(e) => set('lastName', e.target.value)} required
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
                </div>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-600 mb-1">Email</label>
                <input type="email" value={form.email} onChange={(e) => set('email', e.target.value)} required
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-600 mb-1">Phone</label>
                <input type="tel" value={form.phone} onChange={(e) => set('phone', e.target.value)} required placeholder="+503..."
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              </div>

              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setBooking(null)}
                  className="flex-1 rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition">
                  Cancel
                </button>
                <button type="submit" disabled={isPending}
                  className="flex-1 rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-60 transition">
                  {isPending ? 'Booking…' : 'Confirm Booking'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
