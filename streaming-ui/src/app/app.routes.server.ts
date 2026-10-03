import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: '',
    renderMode: RenderMode.Client,
  },
  {
    path: 'dashboard',
    renderMode: RenderMode.Client,
  },
  {
    path: 'streams',
    renderMode: RenderMode.Client,
  },
  {
    path: 'streams/:id',
    renderMode: RenderMode.Client,
  },
  {
    path: 'chat',
    renderMode: RenderMode.Client,
  },
  {
    path: 'profile',
    renderMode: RenderMode.Client,
  },
  {
    path: 'login',
    renderMode: RenderMode.Prerender,
  },
  {
    path: 'register',
    renderMode: RenderMode.Prerender,
  },
  {
    path: '**',
    renderMode: RenderMode.Client,
  },
];
