'use client';

import { useState, useTransition } from 'react';
import { useRouter } from 'next/navigation';
import type { BookingStatus } from '@/lib/types';
import { api } from '@/lib/api';
import ErrorMessage from '@/components/states/ErrorMessage';

export default function BookingActions({
  bookingId,
  status,
}: {
  bookingId: string;
  status: BookingStatus;
}) {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();
  const [error, setError] = useState<string | null>(null);

  async function handleAction(action: 'confirm' | 'cancel') {
    setError(null);
    try {
      await api.put(`/api/v1/bookings/${bookingId}/${action}`);
      startTransition(() => router.refresh());
    } catch (err: unknown) {
      const e = err as { message?: string };
      setError(e.message ?? `Failed to ${action} booking.`);
    }
  }

  if (status === 'Cancelled') {
    return (
      <p className="text-center text-sm text-gray-400 italic">This booking has been cancelled.</p>
    );
  }

  return (
    <div className="space-y-3">
      {error && <ErrorMessage message={error} />}
      <div className="flex gap-3">
        {status === 'Pending' && (
          <button
            onClick={() => handleAction('confirm')}
            disabled={isPending}
            className="flex-1 rounded-lg bg-green-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-green-700 disabled:opacity-60 transition"
          >
            {isPending ? 'Processing…' : 'Confirm Booking'}
          </button>
        )}
        <button
          onClick={() => handleAction('cancel')}
          disabled={isPending}
          className="flex-1 rounded-lg border border-red-300 px-4 py-2.5 text-sm font-semibold text-red-600 hover:bg-red-50 disabled:opacity-60 transition"
        >
          {isPending ? 'Processing…' : 'Cancel Booking'}
        </button>
      </div>
    </div>
  );
}
