import { Route } from "@angular/router";
import { Settings } from "./settings";
import { Space } from "./space/space";

export default [
    {
        path: '',
        component: Settings,
        children: [
            {
                path: 'space',
                component: Space
            }

        ]
    }
] satisfies Route[];
