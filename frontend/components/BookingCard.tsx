import Link from 'next/link';
import type { BookingDto } from '@/lib/types';
import StatusBadge from './StatusBadge';

export default function BookingCard({ booking }: { booking: BookingDto }) {
  const fmt = (d: string) => new Date(d).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });

  return (
    <Link href={`/bookings/${booking.id}`} className="block group">
      <div className="rounded-xl border border-gray-200 bg-white p-5 shadow-sm hover:shadow-md transition">
        <div className="flex items-start justify-between gap-2">
          <div>
            <p className="font-semibold text-gray-900 group-hover:text-blue-600 transition">
              {booking.hotelName}
            </p>
            <p className="text-sm text-gray-500">{booking.roomTypeName}</p>
          </div>
          <StatusBadge status={booking.status} />
        </div>

        <div className="mt-3 flex flex-wrap gap-x-6 gap-y-1 text-sm text-gray-600">
          <span>
            <span className="font-medium">Check-in:</span> {fmt(booking.checkIn)}
          </span>
          <span>
            <span className="font-medium">Check-out:</span> {fmt(booking.checkOut)}
          </span>
          <span>
            <span className="font-medium">Nights:</span> {booking.nights}
          </span>
          <span>
            <span className="font-medium">Guests:</span> {booking.guestCount}
          </span>
        </div>

        <div className="mt-3 flex items-center justify-between">
          <span className="text-xs text-gray-400 font-mono">{booking.confirmationNumber}</span>
          <span className="text-base font-bold text-blue-700">
            ${booking.totalPrice.toFixed(2)}
          </span>
        </div>
      </div>
    </Link>
  );
}
