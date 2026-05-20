export default function EmptyState({
  title = 'No results found',
  description = 'Try adjusting your search criteria.',
}: {
  title?: string;
  description?: string;
}) {
  return (
    <div className="flex flex-col items-center justify-center py-16 gap-3 text-gray-400">
      <svg className="h-16 w-16" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1}
          d="M21 21l-4.35-4.35M17 11A6 6 0 115 11a6 6 0 0112 0z" />
      </svg>
      <p className="text-lg font-semibold text-gray-600">{title}</p>
      <p className="text-sm">{description}</p>
    </div>
  );
}
