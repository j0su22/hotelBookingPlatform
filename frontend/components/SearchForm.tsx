'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';

const inputCls =
  'w-full rounded-lg border border-gray-300 bg-white px-3 py-2.5 text-sm text-gray-900 ' +
  'placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ' +
  '[color-scheme:light]';

const labelCls = 'block text-xs font-semibold text-gray-600 mb-1.5';

export default function SearchForm({
  defaultValues,
}: {
  defaultValues?: { hotelId?: string; checkIn?: string; checkOut?: string; guests?: string };
}) {
  const router = useRouter();
  const today = new Date().toISOString().split('T')[0];

  const [form, setForm] = useState({
    checkIn:  defaultValues?.checkIn  ?? new Date(Date.now() + 86_400_000).toISOString().split('T')[0],
    checkOut: defaultValues?.checkOut ?? new Date(Date.now() + 4 * 86_400_000).toISOString().split('T')[0],
    guests:   defaultValues?.guests   ?? '2',
  });

  function set(field: string, value: string) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const params = new URLSearchParams({
      checkIn:  form.checkIn,
      checkOut: form.checkOut,
      guests:   form.guests,
    });
    router.push(`/search?${params}`);
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="rounded-2xl bg-white shadow-xl p-5 flex flex-col sm:flex-row gap-4 items-end"
    >
      <div className="flex-1 min-w-0">
        <label className={labelCls}>Check-in</label>
        <input
          type="date"
          value={form.checkIn}
          min={today}
          onChange={(e) => set('checkIn', e.target.value)}
          required
          className={inputCls}
        />
      </div>

      <div className="flex-1 min-w-0">
        <label className={labelCls}>Check-out</label>
        <input
          type="date"
          value={form.checkOut}
          min={form.checkIn || today}
          onChange={(e) => set('checkOut', e.target.value)}
          required
          className={inputCls}
        />
      </div>

      <div className="w-32">
        <label className={labelCls}>Guests</label>
        <input
          type="number"
          value={form.guests}
          min="1"
          max="10"
          onChange={(e) => set('guests', e.target.value)}
          required
          className={inputCls}
        />
      </div>

      <button
        type="submit"
        className="shrink-0 rounded-lg bg-blue-600 px-7 py-2.5 text-sm font-semibold text-white
                   hover:bg-blue-700 active:bg-blue-800 transition-colors shadow-sm"
      >
        Search
      </button>
    </form>
  );
}
