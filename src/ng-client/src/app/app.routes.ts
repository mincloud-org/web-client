import { Routes } from '@angular/router';
import { MainLayout } from './core/layout/main-layout/main-layout';

export const routes: Routes = [
    {
        path: '',
        component: MainLayout,
        children: [
            { path: 'settings', loadComponent: () => import('./features/settings/settings').then(m => m.Settings) },
            { path: '', redirectTo: 'settings', pathMatch: 'full' }
        ]
    },
    {
        path: '**',
        redirectTo: ''
    }
];
