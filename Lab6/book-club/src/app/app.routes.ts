import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' }, // Default route
  {
    path: 'home',
    loadComponent: () =>
      import('./components/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'my-reviews',
    loadComponent: () =>
      import('./components/my-reviews/my-reviews.component').then(
        (m) => m.MyReviewsComponent
      ),
  },
  {
    path: 'favorites',
    loadComponent: () =>
      import('./components/favorites/favorites.component').then(
        (m) => m.FavoritesComponent
      ),
  },
];
