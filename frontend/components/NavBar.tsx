'use client';

import Link from 'next/link';
import { useRouter, usePathname } from 'next/navigation';
import { useEffect, useState } from 'react';
import { clearToken, isAuthenticated } from '@/lib/api';

export default function NavBar() {
  const router = useRouter();
  const pathname = usePathname();
  const [loggedIn, setLoggedIn] = useState(false);

  useEffect(() => {
    // Initial check
    setLoggedIn(isAuthenticated());

    // Re-check whenever login/logout happens (same tab or cross-tab)
    const handler = () => setLoggedIn(isAuthenticated());
    window.addEventListener('authchange', handler);
    window.addEventListener('storage', handler);
    return () => {
      window.removeEventListener('authchange', handler);
      window.removeEventListener('storage', handler);
    };
  }, []);

  function handleLogout() {
    clearToken();
    router.push('/search');
  }

  const linkCls = (href: string) =>
    `transition ${pathname?.startsWith(href) ? 'text-blue-600 font-semibold' : 'text-gray-600 hover:text-blue-600'}`;

  return (
    <nav className="flex items-center gap-5 text-sm font-medium">
      <Link href="/search" className={linkCls('/search')}>Search</Link>
      <Link href="/bookings" className={linkCls('/bookings')}>Bookings</Link>

      {loggedIn ? (
        <>
          <Link href="/admin" className={`${linkCls('/admin')} flex items-center gap-1`}>
            <span className="inline-block w-1.5 h-1.5 rounded-full bg-emerald-500" />
            Admin
          </Link>
          <button
            onClick={handleLogout}
            className="rounded-lg border border-gray-300 px-4 py-1.5 text-gray-600 hover:border-gray-400 hover:text-gray-800 transition"
          >
            Logout
          </button>
        </>
      ) : (
        <Link
          href="/auth/login"
          className="rounded-lg bg-blue-600 px-4 py-1.5 text-white hover:bg-blue-700 transition"
        >
          Login
        </Link>
      )}
    </nav>
  );
}
