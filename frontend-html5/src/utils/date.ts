export function formatDate(isoString: string): string {
  return new Date(isoString).toLocaleDateString('lt-LT', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  });
}

export function formatDateTime(isoString: string): string {
  return new Date(isoString).toLocaleString('lt-LT');
}
