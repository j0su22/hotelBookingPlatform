import Link from 'next/link';

export default function AdminDashboard() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Admin Dashboard</h1>
        <p className="text-sm text-gray-500 mt-1">Manage hotels, room types, and platform settings.</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <AdminCard
          href="/admin/hotels"
          title="Hotels"
          description="Create, edit and manage hotel listings and their room types."
          icon="🏨"
          color="blue"
        />
        <AdminCard
          href="/bookings"
          title="Bookings"
          description="View and manage guest reservations across all properties."
          icon="📋"
          color="purple"
        />
        <AdminCard
          href="/search"
          title="Availability"
          description="Preview the public search page and room availability."
          icon="🔍"
          color="gray"
        />
      </div>
    </div>
  );
}

function AdminCard({
  href,
  title,
  description,
  icon,
  color,
}: {
  href: string;
  title: string;
  description: string;
  icon: string;
  color: 'blue' | 'purple' | 'gray';
}) {
  const border = { blue: 'border-blue-200 hover:border-blue-400', purple: 'border-purple-200 hover:border-purple-400', gray: 'border-gray-200 hover:border-gray-400' }[color];
  const iconBg = { blue: 'bg-blue-50', purple: 'bg-purple-50', gray: 'bg-gray-50' }[color];
  const iconText = { blue: 'text-blue-700', purple: 'text-purple-700', gray: 'text-gray-700' }[color];

  return (
    <Link
      href={href}
      className={`group rounded-xl border ${border} bg-white p-6 shadow-sm hover:shadow-md transition flex flex-col gap-3`}
    >
      <span className={`inline-flex items-center justify-center w-11 h-11 rounded-xl ${iconBg} text-2xl`}>
        {icon}
      </span>
      <div>
        <p className={`font-semibold text-gray-900 group-hover:${iconText} transition`}>{title}</p>
        <p className="text-sm text-gray-500 mt-0.5 leading-relaxed">{description}</p>
      </div>
    </Link>
  );
}
