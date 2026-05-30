import { apiFetch, ApiError } from './client';
import type { Picture } from '../types';

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? '';

export async function getPictures(): Promise<Picture[]> {
  return apiFetch<Picture[]>('/api/v1/gallery');
}

export async function uploadPicture(file: File): Promise<Picture> {
  const formData = new FormData();
  formData.append('file', file);
  const response = await fetch(`${API_BASE}/api/v1/gallery`, {
    method: 'POST',
    credentials: 'include',
    body: formData,
  });
  if (!response.ok) {
    const text = await response.text().catch(() => response.statusText);
    throw new ApiError(response.status, text);
  }
  return response.json() as Promise<Picture>;
}

export async function updatePictureOrder(id: string, order: number): Promise<Picture> {
  return apiFetch<Picture>(`/api/v1/gallery/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ order }),
  });
}

export async function deletePicture(id: string): Promise<void> {
  await apiFetch(`/api/v1/gallery/${id}`, { method: 'DELETE' });
}
