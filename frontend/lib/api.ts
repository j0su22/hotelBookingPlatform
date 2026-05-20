import type { ApiError } from './types';

const BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000';

function getToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem('hbp_token');
}

export function clearToken(): void {
  if (typeof window !== 'undefined') localStorage.removeItem('hbp_token');
}

export function saveToken(token: string): void {
  if (typeof window !== 'undefined') localStorage.setItem('hbp_token', token);
}

async function request<T>(
  path: string,
  options: RequestInit & { idempotent?: boolean } = {}
): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  };

  const token = getToken();
  if (token) headers['Authorization'] = `Bearer ${token}`;

  if (options.idempotent) {
    headers['Idempotency-Key'] = crypto.randomUUID();
  }

  const res = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers,
    cache: 'no-store',
  });

  if (!res.ok) {
    let message = `HTTP ${res.status}`;
    try {
      const body = await res.json();
      message = body?.title ?? body?.message ?? message;
    } catch {
      // ignore parse error
    }
    const err: ApiError = { status: res.status, message };
    throw err;
  }

  if (res.status === 204) return undefined as T;
  return res.json();
}

export const api = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'POST', body: JSON.stringify(body), idempotent: true }),
  put: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: 'PUT', body: body ? JSON.stringify(body) : undefined }),
  delete: <T>(path: string) => request<T>(path, { method: 'DELETE' }),
};
