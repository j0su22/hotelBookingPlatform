'use client';

import { useState, useEffect, useCallback, useMemo } from 'react';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import { api } from '@/lib/api';
import type { InventoryDto, RoomTypeDto, UpsertInventoryRequest } from '@/lib/types';

/* ─── date helpers ─────────────────────────────────────────── */
function toIso(d: Date) {
  return d.toISOString().split('T')[0];
}
function parseDate(iso: string): Date {
  return new Date(iso + 'T00:00:00');
}
function daysInMonth(year: number, month: number) {   // month: 0-based
  return new Date(year, month + 1, 0).getDate();
}
function firstWeekday(year: number, month: number) {  // 0=Sun
  return new Date(year, month, 1).getDay();
}
const MONTHS = ['January','February','March','April','May','June',
                'July','August','September','October','November','December'];
const WEEKDAYS = ['Su','Mo','Tu','We','Th','Fr','Sa'];

/* ─── colour for a cell ────────────────────────────────────── */
function cellStyle(inv: InventoryDto | undefined) {
  if (!inv) return { bg: 'bg-gray-50 text-gray-300', label: '—' };
  if (inv.totalRooms === 0) return { bg: 'bg-gray-100 text-gray-400', label: '—' };
  const ratio = inv.availableRooms / inv.totalRooms;
  if (inv.availableRooms === 0)
    return { bg: 'bg-red-100 text-red-700', label: `0/${inv.totalRooms}` };
  if (ratio <= 0.3)
    return { bg: 'bg-orange-100 text-orange-700', label: `${inv.availableRooms}/${inv.totalRooms}` };
  if (ratio <= 0.6)
    return { bg: 'bg-yellow-100 text-yellow-700', label: `${inv.availableRooms}/${inv.totalRooms}` };
  return { bg: 'bg-emerald-100 text-emerald-800', label: `${inv.availableRooms}/${inv.totalRooms}` };
}

/* ─── component ────────────────────────────────────────────── */
export default function InventoryPage() {
  const params = useParams();
  const hotelId = params.id as string;
  const roomTypeId = params.rtId as string;

  const today = new Date();
  const [viewYear,  setViewYear]  = useState(today.getFullYear());
  const [viewMonth, setViewMonth] = useState(today.getMonth());

  const [roomType,  setRoomType]  = useState<RoomTypeDto | null>(null);
  const [inventory, setInventory] = useState<Map<string, InventoryDto>>(new Map());
  const [loading,   setLoading]   = useState(true);
  const [pageError, setPageError] = useState<string | null>(null);

  // Configure modal
  const [modal, setModal] = useState(false);
  const [form, setForm] = useState({ from: '', to: '', totalRooms: '10' });
  const [submitting, setSubmitting] = useState(false);
  const [formError,  setFormError]  = useState<string | null>(null);
  const [formOk,     setFormOk]     = useState<string | null>(null);

  /* fetch inventory for the visible month (+ one day buffer) */
  const load = useCallback(async (year: number, month: number) => {
    setLoading(true);
    setPageError(null);
    const from = toIso(new Date(year, month, 1));
    const to   = toIso(new Date(year, month + 1, 1)); // exclusive

    try {
      const [rt, inv] = await Promise.all([
        roomType ? Promise.resolve(roomType)
                 : api.get<RoomTypeDto>(`/api/v1/hotels/${hotelId}/room-types`)
                     .then((arr: unknown) => (arr as RoomTypeDto[]).find(r => r.id === roomTypeId) ?? null),
        api.get<InventoryDto[]>(`/api/v1/room-types/${roomTypeId}/inventory?from=${from}&to=${to}`),
      ]);
      if (rt) setRoomType(rt);
      const map = new Map<string, InventoryDto>();
      inv.forEach(i => map.set(i.date, i));
      setInventory(map);
    } catch {
      setPageError('Failed to load inventory.');
    } finally {
      setLoading(false);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hotelId, roomTypeId]);

  useEffect(() => { load(viewYear, viewMonth); }, [load, viewYear, viewMonth]);

  /* month navigation */
  function prevMonth() {
    if (viewMonth === 0) { setViewYear(y => y - 1); setViewMonth(11); }
    else setViewMonth(m => m - 1);
  }
  function nextMonth() {
    if (viewMonth === 11) { setViewYear(y => y + 1); setViewMonth(0); }
    else setViewMonth(m => m + 1);
  }

  /* calendar grid cells */
  const cells = useMemo(() => {
    const days   = daysInMonth(viewYear, viewMonth);
    const offset = firstWeekday(viewYear, viewMonth); // Sun=0
    return Array.from({ length: offset + days }, (_, i) => {
      if (i < offset) return null;
      const day = i - offset + 1;
      const iso = toIso(new Date(viewYear, viewMonth, day));
      return { day, iso };
    });
  }, [viewYear, viewMonth]);

  /* open modal with smart defaults */
  function openModal() {
    const from = toIso(new Date(viewYear, viewMonth, 1));
    const to   = toIso(new Date(viewYear, viewMonth + 1, 0)); // last day of month
    setForm({ from, to, totalRooms: '10' });
    setFormError(null);
    setFormOk(null);
    setModal(true);
  }

  /* submit upsert */
  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitting(true);
    setFormError(null);
    setFormOk(null);

    // "to" in the API is exclusive → add 1 day
    const toExclusive = toIso(new Date(parseDate(form.to).getTime() + 86_400_000));

    const payload: UpsertInventoryRequest = {
      from: form.from,
      to:   toExclusive,
      totalRooms: Number(form.totalRooms),
    };

    try {
      const count = await api.put<number>(
        `/api/v1/room-types/${roomTypeId}/inventory`, payload);
      setFormOk(`✓ ${count} day${count !== 1 ? 's' : ''} configured successfully.`);
      setModal(false);
      await load(viewYear, viewMonth);
    } catch (err: unknown) {
      const e = err as { message?: string };
      setFormError(e.message ?? 'Failed to configure inventory.');
    } finally {
      setSubmitting(false);
    }
  }

  /* legend */
  const legend = [
    { bg: 'bg-emerald-100',  label: 'Available (>60%)' },
    { bg: 'bg-yellow-100',   label: '31–60%' },
    { bg: 'bg-orange-100',   label: '1–30%' },
    { bg: 'bg-red-100',      label: 'Fully booked' },
    { bg: 'bg-gray-50 border border-gray-200', label: 'Not configured' },
  ];

  /* ─── render ──────────────────────────────────────────────── */
  return (
    <div className="space-y-5">
      {/* breadcrumb */}
      <div className="flex items-center gap-2 text-sm text-gray-500 flex-wrap">
        <Link href="/admin/hotels" className="hover:text-blue-600 transition">Hotels</Link>
        <span>/</span>
        <Link href={`/admin/hotels/${hotelId}/room-types`} className="hover:text-blue-600 transition">
          Room Types
        </Link>
        <span>/</span>
        <span className="text-gray-900 font-medium">{roomType?.name ?? '…'}</span>
        <span>/</span>
        <span className="text-gray-900 font-medium">Inventory</span>
      </div>

      {/* header */}
      <div className="flex items-start justify-between gap-4 flex-wrap">
        <div>
          <h1 className="text-xl font-bold text-gray-900">
            {roomType ? `${roomType.name} — Inventory` : 'Inventory'}
          </h1>
          <p className="text-sm text-gray-500 mt-0.5">
            Manage available rooms per day · Base price ${roomType?.basePrice}/night
          </p>
        </div>
        <button
          onClick={openModal}
          className="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 transition shadow-sm whitespace-nowrap"
        >
          + Configure Period
        </button>
      </div>

      {pageError && (
        <div className="rounded-lg bg-red-50 border border-red-200 px-4 py-3 text-sm text-red-700">{pageError}</div>
      )}
      {formOk && (
        <div className="rounded-lg bg-emerald-50 border border-emerald-200 px-4 py-3 text-sm text-emerald-700">{formOk}</div>
      )}

      {/* ── Calendar ── */}
      <div className="rounded-xl border border-gray-200 bg-white shadow-sm overflow-hidden">
        {/* month nav */}
        <div className="flex items-center justify-between px-5 py-3 border-b border-gray-100 bg-gray-50">
          <button onClick={prevMonth}
            className="rounded-lg p-1.5 text-gray-500 hover:bg-gray-200 transition">
            ‹
          </button>
          <h2 className="font-semibold text-gray-800 text-sm tracking-wide">
            {MONTHS[viewMonth]} {viewYear}
          </h2>
          <button onClick={nextMonth}
            className="rounded-lg p-1.5 text-gray-500 hover:bg-gray-200 transition">
            ›
          </button>
        </div>

        {/* weekday headers */}
        <div className="grid grid-cols-7 border-b border-gray-100">
          {WEEKDAYS.map(d => (
            <div key={d} className="py-2 text-center text-xs font-semibold text-gray-400">{d}</div>
          ))}
        </div>

        {/* day cells */}
        {loading ? (
          <div className="py-16 text-center text-sm text-gray-400">Loading…</div>
        ) : (
          <div className="grid grid-cols-7 gap-px bg-gray-100">
            {cells.map((cell, i) =>
              cell === null ? (
                <div key={`empty-${i}`} className="bg-white h-16" />
              ) : (
                <DayCell key={cell.iso} day={cell.day} inv={inventory.get(cell.iso)} />
              )
            )}
          </div>
        )}

        {/* legend */}
        <div className="flex flex-wrap items-center gap-3 px-5 py-3 border-t border-gray-100 bg-gray-50">
          {legend.map(l => (
            <div key={l.label} className="flex items-center gap-1.5 text-xs text-gray-500">
              <span className={`inline-block w-3 h-3 rounded ${l.bg}`} />
              {l.label}
            </div>
          ))}
        </div>
      </div>

      {/* ─── Configure Period Modal ─── */}
      {modal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md p-6 space-y-5">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-bold text-gray-900">Configure Available Rooms</h2>
              <button onClick={() => setModal(false)} className="text-gray-400 hover:text-gray-600 text-xl">&times;</button>
            </div>

            <p className="text-sm text-gray-500">
              Set the number of available rooms for each day in the selected period.
              Days with existing bookings will not lose reserved capacity.
            </p>

            {formError && (
              <div className="rounded-lg bg-red-50 border border-red-200 px-3 py-2 text-sm text-red-700">{formError}</div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-xs font-semibold text-gray-600 mb-1">From date *</label>
                  <input
                    type="date"
                    value={form.from}
                    onChange={e => setForm(f => ({ ...f, from: e.target.value }))}
                    required
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 [color-scheme:light]"
                  />
                </div>
                <div>
                  <label className="block text-xs font-semibold text-gray-600 mb-1">To date (inclusive) *</label>
                  <input
                    type="date"
                    value={form.to}
                    min={form.from}
                    onChange={e => setForm(f => ({ ...f, to: e.target.value }))}
                    required
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 [color-scheme:light]"
                  />
                </div>
              </div>

              <div>
                <label className="block text-xs font-semibold text-gray-600 mb-1">
                  Rooms per day *
                </label>
                <input
                  type="number"
                  value={form.totalRooms}
                  min={1}
                  max={500}
                  onChange={e => setForm(f => ({ ...f, totalRooms: e.target.value }))}
                  required
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500"
                />
                <p className="text-xs text-gray-400 mt-1">
                  Total rooms to offer per night. Existing bookings are preserved.
                </p>
              </div>

              <div className="flex gap-3 pt-1">
                <button type="button" onClick={() => setModal(false)}
                  className="flex-1 rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition">
                  Cancel
                </button>
                <button type="submit" disabled={submitting}
                  className="flex-1 rounded-lg bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-60 transition">
                  {submitting ? 'Saving…' : 'Apply'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

/* ─── Day cell sub-component ──────────────────────────────── */
function DayCell({ day, inv }: { day: number; inv: InventoryDto | undefined }) {
  const { bg, label } = cellStyle(inv);
  return (
    <div className={`bg-white h-16 flex flex-col items-center justify-center gap-0.5 ${bg}`}>
      <span className="text-xs font-semibold">{day}</span>
      <span className="text-[10px] leading-none font-medium">{label}</span>
    </div>
  );
}
