import { Routes } from '@angular/router';
import { MainLayout } from './core/layout/main-layout/main-layout';

export const routes: Routes = [
    {
        path: '',
        component: MainLayout,
        children: [
            { path: 'settings', loadChildren: () => import('./features/settings/settings.routes') },
            { path: '', redirectTo: 'settings', pathMatch: 'full' }
        ]
    },
    {
        path: '**',
        redirectTo: ''
    }
];
