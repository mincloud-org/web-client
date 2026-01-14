import { Route } from "@angular/router";
import { Settings } from "./settings"; 
import { User } from "../user/user";
import { Storage } from "../storage/storage";

export default [
    {
        path: '',
        component: Settings,
        children: [
            {
                path: 'space',
                loadChildren: () => import('../space/space.routes')
            },
            {
                path: 'user',
                component: User
            },
            {
                path: 'storage',
                loadChildren: () => import('../storage/storage.routes')
            },
            {
                path: '',
                redirectTo: 'user',
                pathMatch: 'full'
            }
        ]
    }
] satisfies Route[];
