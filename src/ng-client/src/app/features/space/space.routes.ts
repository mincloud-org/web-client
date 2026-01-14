import { Route } from "@angular/router";
import { Space } from "./space";
import { SpaceList } from "./list/list";
import { SpaceDetail } from "./detail/detail";

export default [
    {
        path: '',
        component: Space,
        children: [
            {
                path: 'list',
                component: SpaceList
            },
            {
                path: 'detail/:id',
                component: SpaceDetail
            },
            {
                path: '',
                redirectTo: 'list',
                pathMatch: 'full'
            }
        ]
    }
] satisfies Route[];
