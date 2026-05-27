import { getMe, logout as apiLogout } from './api/auth.api';
import type { User } from './types';

let _currentUser: User | null = null;

export async function initAuth(): Promise<User | null> {
  try {
    _currentUser = await getMe();
  } catch {
    _currentUser = null;
  }
  return _currentUser;
}

export function currentUser(): User | null {
  return _currentUser;
}

export function isAdmin(): boolean {
  return _currentUser?.role === 'Admin';
}

export async function logout(): Promise<void> {
  await apiLogout();
  _currentUser = null;
}
