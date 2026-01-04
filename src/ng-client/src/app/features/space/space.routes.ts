import { Route } from "@angular/router";
import { Space } from "./space";
import { SpaceList } from "./list/list";
import { SpaceDetail } from "./detail/detail";
import { SpaceEditor } from "./editor/editor";

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
                path: 'edit/:id',
                component: SpaceEditor
            },
            {
                path: 'create',
                component: SpaceEditor
            },
            {
                path: '',
                redirectTo: 'list',
                pathMatch: 'full'
            }
        ]
    }
] satisfies Route[];
