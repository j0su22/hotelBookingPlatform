'use client';

import ErrorMessage from '@/components/states/ErrorMessage';

export default function SearchError({ reset }: { error: Error; reset: () => void }) {
  return <ErrorMessage message="Failed to load search results." onRetry={reset} />;
}
