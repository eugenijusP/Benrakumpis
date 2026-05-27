import { apiFetch } from './client';
import type { User } from '../types';

export async function login(username: string, password: string): Promise<void> {
  await apiFetch('/api/v1/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password }),
  });
}

export async function logout(): Promise<void> {
  await apiFetch('/api/v1/auth/logout', { method: 'POST' });
}

export async function getMe(): Promise<User> {
  return apiFetch<User>('/api/v1/auth/me');
}
