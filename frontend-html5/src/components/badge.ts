export function spinner(): string {
  return '<div class="bh-spinner" aria-label="Loading"></div>';
}

export function errorMessage(message: string): string {
  return `<div class="bh-error-message" role="alert">${message}</div>`;
}

export function emptyState(message: string): string {
  return `<div class="bh-empty-state">${message}</div>`;
}

export function roleBadge(role: string): string {
  const cls = role === 'Admin' ? 'bh-badge-admin' : 'bh-badge-user';
  return `<span class="bh-badge ${cls}">${role}</span>`;
}
