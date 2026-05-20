'use client';

import ErrorMessage from '@/components/states/ErrorMessage';

export default function BookingDetailError({ reset }: { error: Error; reset: () => void }) {
  return <ErrorMessage message="Failed to load booking details." onRetry={reset} />;
}
