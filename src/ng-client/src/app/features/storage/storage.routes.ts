import { Route } from "@angular/router";
import { StorageList } from "./list/list";
import { StorageDetail } from "./detail/detail";
import { StorageEditor } from "./editor/editor";
import { Storage } from "./storage";

export default [
    {
        path: '',
        component: Storage,
        children: [
            {
                path: 'list',
                component: StorageList
            },
            {
                path: 'detail/:id',
                component: StorageDetail
            },
            {
                path: 'edit/:id',
                component: StorageEditor
            },
            {
                path: 'create',
                component: StorageEditor
            },
            {
                path: '',
                redirectTo: 'list',
                pathMatch: 'full'
            }
        ]
    }
] satisfies Route[];
