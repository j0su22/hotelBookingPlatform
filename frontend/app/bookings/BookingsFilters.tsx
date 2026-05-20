'use client';

import { useRouter, useSearchParams } from 'next/navigation';

const STATUS_OPTIONS = ['', 'Pending', 'Confirmed', 'Cancelled'] as const;

export default function BookingsFilters({ currentStatus }: { currentStatus?: string }) {
  const router = useRouter();
  const searchParams = useSearchParams();

  function handleStatus(status: string) {
    const params = new URLSearchParams(searchParams.toString());
    if (status) params.set('status', status);
    else params.delete('status');
    params.delete('page');
    router.push(`/bookings?${params}`);
  }

  return (
    <div className="flex items-center gap-2 flex-wrap">
      {STATUS_OPTIONS.map((s) => (
        <button
          key={s || 'all'}
          onClick={() => handleStatus(s)}
          className={`rounded-full px-4 py-1.5 text-sm font-medium transition ${
            (currentStatus ?? '') === s
              ? 'bg-blue-600 text-white'
              : 'bg-white border border-gray-300 text-gray-600 hover:bg-gray-50'
          }`}
        >
          {s || 'All'}
        </button>
      ))}
    </div>
  );
}
