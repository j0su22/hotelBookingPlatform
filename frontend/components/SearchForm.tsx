'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';

export default function SearchForm({
  defaultValues,
}: {
  defaultValues?: { hotelId?: string; checkIn?: string; checkOut?: string; guests?: string };
}) {
  const router = useRouter();
  const today = new Date().toISOString().split('T')[0];
  const tomorrow = new Date(Date.now() + 86_400_000).toISOString().split('T')[0];

  const [form, setForm] = useState({
    checkIn: defaultValues?.checkIn ?? tomorrow,
    checkOut: defaultValues?.checkOut ?? new Date(Date.now() + 3 * 86_400_000).toISOString().split('T')[0],
    guests: defaultValues?.guests ?? '2',
  });

  function set(field: string, value: string) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const params = new URLSearchParams({
      checkIn: form.checkIn,
      checkOut: form.checkOut,
      guests: form.guests,
    });
    router.push(`/search?${params}`);
  }

  return (
    <form onSubmit={handleSubmit} className="rounded-2xl bg-white shadow-lg p-6 flex flex-col sm:flex-row gap-3 items-end">
      <div className="flex-1 min-w-0">
        <label className="block text-xs font-semibold text-gray-500 mb-1">Check-in</label>
        <input
          type="date"
          value={form.checkIn}
          min={today}
          onChange={(e) => set('checkIn', e.target.value)}
          required
          className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div className="flex-1 min-w-0">
        <label className="block text-xs font-semibold text-gray-500 mb-1">Check-out</label>
        <input
          type="date"
          value={form.checkOut}
          min={form.checkIn || today}
          onChange={(e) => set('checkOut', e.target.value)}
          required
          className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div className="w-28">
        <label className="block text-xs font-semibold text-gray-500 mb-1">Guests</label>
        <input
          type="number"
          value={form.guests}
          min="1"
          max="10"
          onChange={(e) => set('guests', e.target.value)}
          required
          className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <button
        type="submit"
        className="shrink-0 rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 active:bg-blue-800 transition"
      >
        Search
      </button>
    </form>
  );
}
