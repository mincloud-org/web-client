import { Route } from "@angular/router";
import { Settings } from "./settings"; 
import { Users } from "../users/users";
import { Storages } from "../storages/storages";

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
                path: 'users',
                component: Users
            },
            {
                path: 'storages',
                component: Storages
            },
            {
                path: '',
                redirectTo: 'users',
                pathMatch: 'full'
            }
        ]
    }
] satisfies Route[];
