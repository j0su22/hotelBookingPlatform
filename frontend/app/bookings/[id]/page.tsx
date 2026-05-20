import { notFound } from 'next/navigation';
import type { BookingDetailDto } from '@/lib/types';
import StatusBadge from '@/components/StatusBadge';
import BookingActions from './BookingActions';

interface BookingDetailPageProps {
  params: Promise<{ id: string }>;
}

async function fetchBooking(id: string): Promise<BookingDetailDto | null> {
  const base = process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000';
  const res = await fetch(`${base}/api/v1/bookings/${id}`, { cache: 'no-store' });
  if (res.status === 404) return null;
  if (!res.ok) throw new Error('Failed to load booking');
  return res.json();
}

export default async function BookingDetailPage({ params }: BookingDetailPageProps) {
  const { id } = await params;
  const booking = await fetchBooking(id);

  if (!booking) notFound();

  const fmt = (d: string) =>
    new Date(d + 'T00:00:00').toLocaleDateString('en-US', {
      weekday: 'short', month: 'long', day: 'numeric', year: 'numeric',
    });

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-xs font-mono text-gray-400 mb-1">{booking.confirmationNumber}</p>
          <h1 className="text-2xl font-bold text-gray-900">{booking.hotelName}</h1>
          <p className="text-gray-500">{booking.roomTypeName}</p>
        </div>
        <StatusBadge status={booking.status} />
      </div>

      <div className="rounded-xl border border-gray-200 bg-white shadow-sm divide-y divide-gray-100">
        <Section title="Stay Details">
          <Row label="Check-in" value={fmt(booking.checkIn)} />
          <Row label="Check-out" value={fmt(booking.checkOut)} />
          <Row label="Nights" value={String(booking.nights)} />
          <Row label="Guests" value={String(booking.guestCount)} />
          <Row label="Rate plan" value={booking.ratePlanName} />
          <Row label="Total price" value={`$${booking.totalPrice.toFixed(2)}`} bold />
        </Section>

        <Section title="Guest Information">
          <Row label="Name" value={`${booking.guestFirstName} ${booking.guestLastName}`} />
          <Row label="Email" value={booking.guestEmail} />
          <Row label="Phone" value={booking.guestPhone} />
        </Section>

        <Section title="Booking Info">
          <Row label="Booking ID" value={booking.id} mono />
          <Row label="Created" value={new Date(booking.createdAt).toLocaleString()} />
        </Section>
      </div>

      <BookingActions bookingId={booking.id} status={booking.status} />
    </div>
  );
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="p-5 space-y-3">
      <h2 className="text-xs font-semibold uppercase tracking-wider text-gray-400">{title}</h2>
      {children}
    </div>
  );
}

function Row({ label, value, bold, mono }: { label: string; value: string; bold?: boolean; mono?: boolean }) {
  return (
    <div className="flex justify-between gap-4 text-sm">
      <span className="text-gray-500 shrink-0">{label}</span>
      <span className={`text-right ${bold ? 'font-bold text-blue-700 text-base' : 'text-gray-800'} ${mono ? 'font-mono text-xs break-all' : ''}`}>
        {value}
      </span>
    </div>
  );
}
