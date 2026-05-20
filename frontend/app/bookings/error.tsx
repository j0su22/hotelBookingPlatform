'use client';

import ErrorMessage from '@/components/states/ErrorMessage';

export default function BookingsError({ reset }: { error: Error; reset: () => void }) {
  return <ErrorMessage message="Failed to load bookings." onRetry={reset} />;
}
