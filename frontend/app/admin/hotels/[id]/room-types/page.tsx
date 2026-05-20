'use client';

import { useState, useEffect, useCallback } from 'react';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import { api } from '@/lib/api';
import type { RoomTypeDto, HotelDto, CreateRoomTypeAdminRequest, UpdateRoomTypeAdminRequest } from '@/lib/types';

/* ─── helpers ─────────────────────────────────────────────────── */
const inputCls =
  'w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500';
const labelCls = 'block text-xs font-semibold text-gray-600 mb-1';

type FormState = { name: string; description: string; maxCapacity: string; basePrice: string };
const emptyForm: FormState = { name: '', description: '', maxCapacity: '2', basePrice: '' };

/* ─── component ───────────────────────────────────────────────── */
export default function RoomTypesAdminPage() {
  const params = useParams();
  const hotelId = params.id as string;

  const [hotel, setHotel] = useState<HotelDto | null>(null);
  const [roomTypes, setRoomTypes] = useState<RoomTypeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [pageError, setPageError] = useState<string | null>(null);

  // Modal state
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editing, setEditing] = useState<RoomTypeDto | null>(null);
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
      const [hotelRes, rtRes] = await Promise.all([
        api.get<HotelDto>(`/api/v1/hotels/${hotelId}`),
        api.get<RoomTypeDto[]>(`/api/v1/hotels/${hotelId}/room-types`),
      ]);
      setHotel(hotelRes);
      setRoomTypes(rtRes);
    } catch {
      setPageError('Failed to load room types.');
    } finally {
      setLoading(false);
    }
  }, [hotelId]);

  useEffect(() => { load(); }, [load]);

  /* open modals */
  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setFormError(null);
    setModal('create');
  }

  function openEdit(rt: RoomTypeDto) {
    setEditing(rt);
    setForm({
      name: rt.name,
      description: rt.description,
      maxCapacity: String(rt.maxCapacity),
      basePrice: String(rt.basePrice),
    });
    setFormError(null);
    setModal('edit');
  }

  function closeModal() { setModal(null); setEditing(null); }

  /* submit */
  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitting(true);
    setFormError(null);

    const payload = {
      name: form.name.trim(),
      description: form.description.trim(),
      maxCapacity: Number(form.maxCapacity),
      basePrice: Number(form.basePrice),
    };

    try {
      if (modal === 'create') {
        await api.post(`/api/v1/hotels/${hotelId}/room-types`, payload as CreateRoomTypeAdminRequest);
      } else if (editing) {
        await api.put(`/api/v1/hotels/${hotelId}/room-types/${editing.id}`, payload as UpdateRoomTypeAdminRequest);
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
      await api.delete(`/api/v1/hotels/${hotelId}/room-types/${id}`);
      setDeletingId(null);
      await load();
    } catch (err: unknown) {
      const e = err as { message?: string };
      setDeleteError(e.message ?? 'Failed to delete room type.');
    }
  }

  function setField(field: keyof FormState, value: string) {
    setForm(prev => ({ ...prev, [field]: value }));
  }

  /* ─── render ──────────────────────────────────────────────── */
  return (
    <div className="space-y-5">
      {/* breadcrumb */}
      <div className="flex items-center gap-2 text-sm text-gray-500">
        <Link href="/admin/hotels" className="hover:text-blue-600 transition">Hotels</Link>
        <span>/</span>
        <span className="text-gray-900 font-medium">{hotel?.name ?? '…'}</span>
        <span>/</span>
        <span className="text-gray-900 font-medium">Room Types</span>
      </div>

      {/* header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-bold text-gray-900">
            {hotel ? `${hotel.name} — Room Types` : 'Room Types'}
          </h1>
          <p className="text-sm text-gray-500 mt-0.5">
            {hotel ? `${hotel.city}, ${hotel.country}` : ''}
          </p>
        </div>
        <button
          onClick={openCreate}
          className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 transition shadow-sm"
        >
          + New Room Type
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
        <div className="py-16 text-center text-sm text-gray-400">Loading room types…</div>
      ) : roomTypes.length === 0 ? (
        <div className="rounded-xl border border-dashed border-gray-300 py-16 text-center">
          <p className="text-gray-500 font-medium">No room types yet</p>
          <p className="text-sm text-gray-400 mt-1">Add a room type to configure inventory and rates.</p>
        </div>
      ) : (
        <div className="rounded-xl border border-gray-200 overflow-hidden shadow-sm">
          <table className="w-full text-sm">
            <thead>
              <tr className="bg-gray-50 border-b border-gray-200">
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Name</th>
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Description</th>
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Max Cap.</th>
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Base Price</th>
                <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Status</th>
                <th className="text-right px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {roomTypes.map((rt) => (
                <tr key={rt.id} className="hover:bg-gray-50 transition">
                  <td className="px-4 py-3 font-medium text-gray-900">{rt.name}</td>
                  <td className="px-4 py-3 text-gray-500 max-w-xs">
                    <span className="line-clamp-2">{rt.description || '—'}</span>
                  </td>
                  <td className="px-4 py-3 text-gray-700">
                    <span className="font-medium">{rt.maxCapacity}</span>
                    <span className="text-gray-400 ml-0.5 text-xs">pax</span>
                  </td>
                  <td className="px-4 py-3">
                    <span className="font-semibold text-gray-900">${rt.basePrice.toFixed(2)}</span>
                    <span className="text-gray-400 ml-0.5 text-xs">/night</span>
                  </td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex items-center gap-1 rounded-full px-2 py-0.5 text-xs font-medium ${
                      rt.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-gray-100 text-gray-500'
                    }`}>
                      <span className={`w-1.5 h-1.5 rounded-full ${rt.isActive ? 'bg-emerald-500' : 'bg-gray-400'}`} />
                      {rt.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <Link
                        href={`/admin/hotels/${hotelId}/room-types/${rt.id}/inventory`}
                        className="rounded-md bg-emerald-50 px-2.5 py-1 text-xs font-medium text-emerald-700 hover:bg-emerald-100 transition"
                      >
                        Inventory
                      </Link>
                      <button
                        onClick={() => openEdit(rt)}
                        className="rounded-md bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-700 hover:bg-gray-200 transition"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => { setDeletingId(rt.id); setDeleteError(null); }}
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
                {modal === 'create' ? 'New Room Type' : `Edit — ${editing?.name}`}
              </h2>
              <button onClick={closeModal} className="text-gray-400 hover:text-gray-600 text-xl leading-none">&times;</button>
            </div>

            {formError && (
              <div className="rounded-lg bg-red-50 border border-red-200 px-3 py-2 text-sm text-red-700">{formError}</div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className={labelCls}>Room type name *</label>
                <input value={form.name} onChange={e => setField('name', e.target.value)} required maxLength={100}
                  className={inputCls} placeholder="e.g. Deluxe Double Room" />
              </div>

              <div>
                <label className={labelCls}>Description</label>
                <textarea value={form.description} onChange={e => setField('description', e.target.value)} rows={3}
                  className={inputCls} placeholder="Describe the room type, amenities, views…" />
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className={labelCls}>Max capacity *</label>
                  <input type="number" value={form.maxCapacity} onChange={e => setField('maxCapacity', e.target.value)}
                    required min={1} max={20} className={inputCls} />
                </div>
                <div>
                  <label className={labelCls}>Base price / night *</label>
                  <div className="relative">
                    <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm">$</span>
                    <input type="number" value={form.basePrice} onChange={e => setField('basePrice', e.target.value)}
                      required min={0} step={0.01} className={inputCls + ' pl-7'} placeholder="0.00" />
                  </div>
                </div>
              </div>

              <div className="flex gap-3 pt-1">
                <button type="button" onClick={closeModal}
                  className="flex-1 rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition">
                  Cancel
                </button>
                <button type="submit" disabled={submitting}
                  className="flex-1 rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-60 transition">
                  {submitting ? 'Saving…' : modal === 'create' ? 'Create Room Type' : 'Save Changes'}
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
            <h2 className="text-lg font-bold text-gray-900">Delete room type?</h2>
            <p className="text-sm text-gray-600">
              <span className="font-medium">"{roomTypes.find(r => r.id === deletingId)?.name}"</span> will be deactivated.
              Existing bookings will not be affected.
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
