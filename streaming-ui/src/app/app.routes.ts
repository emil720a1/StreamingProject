import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
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
        path: 'creator-studio',
        loadComponent: () =>
          import(
            './features/creator-studio/creator-studio/creator-studio'
            ).then((component) => component.CreatorStudio),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/profile/profile').then(
            (component) => component.Profile
          ),
      },
    ],
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
    path: '**',
    redirectTo: 'dashboard',
  },
];
