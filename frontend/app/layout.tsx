import type { Metadata } from 'next';
import { Geist } from 'next/font/google';
import Link from 'next/link';
import NavBar from '@/components/NavBar';
import './globals.css';

const geist = Geist({ subsets: ['latin'] });

export const metadata: Metadata = {
  title: 'Hotel Booking Platform',
  description: 'Find and book hotel rooms with ease.',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" className={geist.className}>
      <body className="min-h-screen bg-gray-50">
        <header className="bg-white border-b border-gray-200 shadow-sm">
          <div className="max-w-6xl mx-auto px-4 h-14 flex items-center justify-between">
            <Link href="/search" className="text-lg font-bold text-blue-700 tracking-tight hover:text-blue-800 transition">
              HBP
            </Link>
            <NavBar />
          </div>
        </header>

        <main className="max-w-6xl mx-auto px-4 py-8">
          {children}
        </main>

        <footer className="mt-16 border-t border-gray-200 py-6 text-center text-xs text-gray-400">
          © {new Date().getFullYear()} Hotel Booking Platform
        </footer>
      </body>
    </html>
  );
}
