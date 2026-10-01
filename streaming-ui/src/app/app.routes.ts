import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login').then(
        (component) => component.Login
      ),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register').then(
        (component) => component.Register
      ),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard/dashboard').then(
        (component) => component.Dashboard
      ),
  },
  {
    path: 'streams',
    loadComponent: () =>
      import('./features/streams/stream-list/stream-list').then(
        (component) => component.StreamList
      ),
  },
  {
    path: 'streams/:id',
    loadComponent: () =>
      import('./features/streams/stream-details/stream-details').then(
        (component) => component.StreamDetails
      ),
  },
  {
    path: 'chat',
    loadComponent: () =>
      import('./features/chat/chat/chat').then(
        (component) => component.Chat
      ),
  },
  {
    path: 'profile',
    loadComponent: () =>
      import('./features/profile/profile/profile').then(
        (component) => component.Profile
      ),
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];
