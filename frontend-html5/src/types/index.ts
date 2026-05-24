export interface User {
  id: string;
  username: string;
  role: 'Admin' | 'User' | 'Guest';
}
