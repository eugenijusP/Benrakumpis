export function openModal(id: string): void {
  const modal = document.getElementById(id);
  if (modal) modal.classList.add('bh-modal-open');
}

export function closeModal(id: string): void {
  const modal = document.getElementById(id);
  if (modal) modal.classList.remove('bh-modal-open');
}

export function closeModalOnBackdrop(id: string, event: MouseEvent): void {
  if ((event.target as HTMLElement).id === id) closeModal(id);
}
