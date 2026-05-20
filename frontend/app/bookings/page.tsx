import { Suspense } from 'react';
import type { BookingDto, PagedResponse } from '@/lib/types';
import BookingCard from '@/components/BookingCard';
import EmptyState from '@/components/states/EmptyState';
import LoadingSpinner from '@/components/states/LoadingSpinner';
import BookingsFilters from './BookingsFilters';

interface BookingsPageProps {
  searchParams: Promise<{
    status?: string;
    page?: string;
    pageSize?: string;
  }>;
}

async function fetchBookings(params: URLSearchParams): Promise<PagedResponse<BookingDto>> {
  const base = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000';
  const res = await fetch(`${base}/api/v1/bookings?${params}`, { cache: 'no-store' });
  if (!res.ok) throw new Error('Failed to load bookings');
  return res.json();
}

export default async function BookingsPage({ searchParams }: BookingsPageProps) {
  const sp = await searchParams;
  const params = new URLSearchParams();
  if (sp.status) params.set('status', sp.status);
  params.set('pageNumber', sp.page ?? '1');
  params.set('pageSize', sp.pageSize ?? '12');

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-3">
        <h1 className="text-2xl font-bold text-gray-900">Bookings</h1>
        <BookingsFilters currentStatus={sp.status} />
      </div>

      <Suspense fallback={<LoadingSpinner label="Loading bookings…" />}>
        <BookingsList params={params} currentPage={Number(sp.page ?? 1)} />
      </Suspense>
    </div>
  );
}

async function BookingsList({ params, currentPage }: { params: URLSearchParams; currentPage: number }) {
  const data = await fetchBookings(params);

  if (data.data.length === 0) {
    return <EmptyState title="No bookings found" description="Try a different status filter or create a new booking." />;
  }

  return (
    <div className="space-y-4">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {data.data.map((b) => <BookingCard key={b.id} booking={b} />)}
      </div>

      {data.totalPages > 1 && (
        <ServerPagination currentPage={currentPage} totalPages={data.totalPages} params={params} />
      )}

      <p className="text-xs text-gray-400 text-right">
        {data.totalRecords} total · page {data.pageNumber} of {data.totalPages}
      </p>
    </div>
  );
}

function ServerPagination({
  currentPage,
  totalPages,
  params,
}: {
  currentPage: number;
  totalPages: number;
  params: URLSearchParams;
}) {
  function pageUrl(p: number) {
    const next = new URLSearchParams(params);
    next.set('page', String(p));
    return `/bookings?${next}`;
  }

  const pages = Array.from({ length: totalPages }, (_, i) => i + 1).filter(
    (p) => p === 1 || p === totalPages || Math.abs(p - currentPage) <= 1
  );

  return (
    <nav className="flex items-center justify-center gap-1 mt-4">
      {currentPage > 1 && (
        <a href={pageUrl(currentPage - 1)} className="rounded-md px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-100 transition">
          ‹ Prev
        </a>
      )}
      {pages.map((p, idx) => {
        const prev = pages[idx - 1];
        return (
          <span key={p} className="flex items-center gap-1">
            {prev && p - prev > 1 && <span className="px-1 text-gray-400">…</span>}
            <a href={pageUrl(p)} className={`rounded-md px-3 py-1.5 text-sm transition ${p === currentPage ? 'bg-blue-600 text-white font-medium' : 'text-gray-600 hover:bg-gray-100'}`}>
              {p}
            </a>
          </span>
        );
      })}
      {currentPage < totalPages && (
        <a href={pageUrl(currentPage + 1)} className="rounded-md px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-100 transition">
          Next ›
        </a>
      )}
    </nav>
  );
}
