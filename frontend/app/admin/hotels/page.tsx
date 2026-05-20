'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import { api } from '@/lib/api';
import type { HotelDto, CreateHotelRequest, UpdateHotelRequest, PagedResponse } from '@/lib/types';

/* ─── helpers ────────────────────────────────────────────────── */
const stars = (n: number) => '★'.repeat(n) + '☆'.repeat(5 - n);

const inputCls =
  'w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500';
const labelCls = 'block text-xs font-semibold text-gray-600 mb-1';

type FormState = { name: string; address: string; city: string; country: string; starRating: string };
const emptyForm: FormState = { name: '', address: '', city: '', country: '', starRating: '3' };

/* ─── component ──────────────────────────────────────────────── */
export default function HotelsAdminPage() {
  const [hotels, setHotels] = useState<HotelDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [pageError, setPageError] = useState<string | null>(null);

  // Modal state
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editing, setEditing] = useState<HotelDto | null>(null);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  // Delete confirm
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  /* load */
  const load = useCallback(async () => {
    setLoading(true);
    setPageError(null);
    try {
      const res = await api.get<PagedResponse<HotelDto>>('/api/v1/hotels?pageSize=100');
      setHotels(res.data);
    } catch {
      setPageError('Failed to load hotels.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  /* open modals */
  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setFormError(null);
    setModal('create');
  }

  function openEdit(hotel: HotelDto) {
    setEditing(hotel);
    setForm({
      name: hotel.name,
      address: hotel.address,
      city: hotel.city,
      country: hotel.country,
      starRating: String(hotel.starRating),
    });
    setFormError(null);
    setModal('edit');
  }

  function closeModal() { setModal(null); setEditing(null); }

  /* submit create / edit */
  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitting(true);
    setFormError(null);

    const payload = {
      name: form.name.trim(),
      address: form.address.trim(),
      city: form.city.trim(),
      country: form.country.trim(),
      starRating: Number(form.starRating),
    };

    try {
      if (modal === 'create') {
        await api.post<string>('/api/v1/hotels', payload as CreateHotelRequest);
      } else if (editing) {
        await api.put(`/api/v1/hotels/${editing.id}`, payload as UpdateHotelRequest);
      }
      closeModal();
      await load();
    } catch (err: unknown) {
      const e = err as { message?: string };
      setFormError(e.message ?? 'Something went wrong.');
    } finally {
      setSubmitting(false);
    }
  }

  /* delete */
  async function handleDelete(id: string) {
    setDeleteError(null);
    try {
      await api.delete(`/api/v1/hotels/${id}`);
      setDeletingId(null);
      await load();
    } catch (err: unknown) {
      const e = err as { message?: string };
      setDeleteError(e.message ?? 'Failed to delete hotel.');
    }
  }

  function set(field: keyof FormState, value: string) {
    setForm(prev => ({ ...prev, [field]: value }));
  }

  /* ─── render ─────────────────────────────────────────────── */
  return (
    <div className="space-y-5">
      {/* header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-bold text-gray-900">Hotels</h1>
          <p className="text-sm text-gray-500">Manage all hotel listings</p>
        </div>
        <button
          onClick={openCreate}
          className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 transition shadow-sm"
        >
          + New Hotel
        </button>
      </div>

      {pageError && (
        <div className="rounded-lg bg-red-50 border border-red-200 px-4 py-3 text-sm text-red-700">{pageError}</div>
      )}

      {deleteError && (
        <div className="rounded-lg bg-red-50 border border-red-200 px-4 py-3 text-sm text-red-700">{deleteError}</div>
      )}

      {/* table */}
      {loading ? (
        <div className="py-16 text-center text-sm text-gray-400">Loading hotels…</div>
      ) : hotels.length === 0 ? (
        <div className="rounded-xl border border-dashed border-gray-300 py-16 text-center">
          <p className="text-gray-500 font-medium">No hotels yet</p>
          <p className="text-sm text-gray-400 mt-1">Create your first hotel to get started.</p>
        </div>
      ) : (
        <div className="rounded-xl border border-gray-200 overflow-hidden shadow-sm">
          <table className="w-full text-sm">
            <thead>
              <tr className="bg-gray-50 border-b border-gray-200">
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Hotel</th>
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Location</th>
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Stars</th>
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Status</th>
                <th className="text-right px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {hotels.map((h) => (
                <tr key={h.id} className="hover:bg-gray-50 transition">
                  <td className="px-4 py-3">
                    <p className="font-medium text-gray-900">{h.name}</p>
                    <p className="text-xs text-gray-400 truncate max-w-xs">{h.address}</p>
                  </td>
                  <td className="px-4 py-3 text-gray-600">
                    {h.city}, {h.country}
                  </td>
                  <td className="px-4 py-3 text-amber-500 text-base leading-none tracking-tighter">
                    {stars(h.starRating)}
                  </td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex items-center gap-1 rounded-full px-2 py-0.5 text-xs font-medium ${
                      h.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-gray-100 text-gray-500'
                    }`}>
                      <span className={`w-1.5 h-1.5 rounded-full ${h.isActive ? 'bg-emerald-500' : 'bg-gray-400'}`} />
                      {h.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <Link
                        href={`/admin/hotels/${h.id}/room-types`}
                        className="rounded-md bg-blue-50 px-2.5 py-1 text-xs font-medium text-blue-700 hover:bg-blue-100 transition"
                      >
                        Room Types
                      </Link>
                      <button
                        onClick={() => openEdit(h)}
                        className="rounded-md bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-700 hover:bg-gray-200 transition"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => { setDeletingId(h.id); setDeleteError(null); }}
                        className="rounded-md bg-red-50 px-2.5 py-1 text-xs font-medium text-red-700 hover:bg-red-100 transition"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* ─── Create / Edit Modal ─── */}
      {modal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg p-6 space-y-5">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-bold text-gray-900">
                {modal === 'create' ? 'New Hotel' : `Edit — ${editing?.name}`}
              </h2>
              <button onClick={closeModal} className="text-gray-400 hover:text-gray-600 text-xl leading-none">&times;</button>
            </div>

            {formError && (
              <div className="rounded-lg bg-red-50 border border-red-200 px-3 py-2 text-sm text-red-700">{formError}</div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className={labelCls}>Hotel name *</label>
                <input value={form.name} onChange={e => set('name', e.target.value)} required maxLength={200} className={inputCls} placeholder="e.g. Grand Hotel Plaza" />
              </div>

              <div>
                <label className={labelCls}>Address *</label>
                <input value={form.address} onChange={e => set('address', e.target.value)} required maxLength={500} className={inputCls} placeholder="e.g. 123 Main Street" />
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className={labelCls}>City *</label>
                  <input value={form.city} onChange={e => set('city', e.target.value)} required maxLength={100} className={inputCls} placeholder="e.g. San Salvador" />
                </div>
                <div>
                  <label className={labelCls}>Country *</label>
                  <input value={form.country} onChange={e => set('country', e.target.value)} required maxLength={100} className={inputCls} placeholder="e.g. El Salvador" />
                </div>
              </div>

              <div>
                <label className={labelCls}>Star rating *</label>
                <select value={form.starRating} onChange={e => set('starRating', e.target.value)} className={inputCls}>
                  {[1, 2, 3, 4, 5].map(n => (
                    <option key={n} value={n}>{stars(n)} ({n} star{n > 1 ? 's' : ''})</option>
                  ))}
                </select>
              </div>

              <div className="flex gap-3 pt-1">
                <button type="button" onClick={closeModal}
                  className="flex-1 rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition">
                  Cancel
                </button>
                <button type="submit" disabled={submitting}
                  className="flex-1 rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-60 transition">
                  {submitting ? 'Saving…' : modal === 'create' ? 'Create Hotel' : 'Save Changes'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* ─── Delete Confirm Modal ─── */}
      {deletingId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-sm p-6 space-y-4">
            <h2 className="text-lg font-bold text-gray-900">Delete hotel?</h2>
            <p className="text-sm text-gray-600">
              This will mark <span className="font-medium">"{hotels.find(h => h.id === deletingId)?.name}"</span> as
              inactive. Existing bookings are not affected.
            </p>
            <div className="flex gap-3">
              <button onClick={() => setDeletingId(null)}
                className="flex-1 rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition">
                Cancel
              </button>
              <button onClick={() => handleDelete(deletingId)}
                className="flex-1 rounded-lg bg-red-600 px-4 py-2 text-sm font-semibold text-white hover:bg-red-700 transition">
                Delete
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
