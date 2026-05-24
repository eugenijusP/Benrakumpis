const escapeMap: Record<string, string> = {
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;',
  '"': '&quot;',
  "'": '&#39;',
};

export function escHtml(value: unknown): string {
  return String(value).replace(/[&<>"']/g, (c) => escapeMap[c] ?? c);
}

export function escAttr(value: unknown): string {
  return String(value).replace(/[&<>"']/g, (c) => escapeMap[c] ?? c);
}
