import { Suspense } from 'react';
import type { AvailabilityDto, HotelDto, PagedResponse } from '@/lib/types';
import SearchForm from '@/components/SearchForm';
import AvailabilityResults from './AvailabilityResults';
import LoadingSpinner from '@/components/states/LoadingSpinner';

interface SearchPageProps {
  searchParams: Promise<{
    checkIn?: string;
    checkOut?: string;
    guests?: string;
    hotelId?: string;
  }>;
}

// API_URL = internal Docker URL (container→container); falls back to public URL for local dev
const serverBase = () => process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000';

async function fetchHotels(): Promise<HotelDto[]> {
  const res = await fetch(`${serverBase()}/api/v1/hotels?pageSize=50`, { cache: 'no-store' });
  if (!res.ok) return [];
  const data: PagedResponse<HotelDto> = await res.json();
  return data.data;
}

async function fetchAvailability(
  checkIn: string,
  checkOut: string,
  guests: string,
  hotelId?: string
): Promise<AvailabilityDto[]> {
  const base = serverBase();
  const params = new URLSearchParams({ checkIn, checkOut, guests: guests ?? '1' });
  if (hotelId) params.set('hotelId', hotelId);
  const res = await fetch(`${base}/api/v1/availability?${params}`, { cache: 'no-store' });
  if (!res.ok) return [];
  return res.json();
}

export default async function SearchPage({ searchParams }: SearchPageProps) {
  const sp = await searchParams;
  const { checkIn, checkOut, guests, hotelId } = sp;
  const hasSearch = !!(checkIn && checkOut && guests);

  const hotels = await fetchHotels();

  return (
    <div className="space-y-8">
      <div className="rounded-2xl bg-gradient-to-br from-blue-700 to-blue-500 p-8 text-white">
        <h1 className="text-3xl font-bold mb-1">Find your perfect room</h1>
        <p className="text-blue-100 mb-6 text-sm">Search availability across all our hotels</p>
        <SearchForm defaultValues={{ checkIn, checkOut, guests, hotelId }} />
      </div>

      {hasSearch ? (
        <Suspense fallback={<LoadingSpinner label="Searching available rooms…" />}>
          <AvailabilitySection
            checkIn={checkIn!}
            checkOut={checkOut!}
            guests={guests!}
            hotelId={hotelId}
            hotels={hotels}
          />
        </Suspense>
      ) : (
        <div className="text-center py-12 text-gray-500">
          <p className="text-lg">Enter your dates and guest count to search for availability.</p>
        </div>
      )}
    </div>
  );
}

async function AvailabilitySection({
  checkIn,
  checkOut,
  guests,
  hotelId,
  hotels,
}: {
  checkIn: string;
  checkOut: string;
  guests: string;
  hotelId?: string;
  hotels: HotelDto[];
}) {
  const availability = await fetchAvailability(checkIn, checkOut, guests, hotelId);
  return (
    <AvailabilityResults
      results={availability}
      checkIn={checkIn}
      checkOut={checkOut}
      guests={guests}
      hotels={hotels}
    />
  );
}
