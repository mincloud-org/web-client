import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

export interface FormValidationError {
    propertyName: string;
    errors: string[];
}

export interface ApiErrorResult {
    message: string;
    validationErrors?: FormValidationError[];
    hasValidationErrors: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class ErrorHandler {
    private _apiError: BehaviorSubject<ApiErrorResult | undefined> = new BehaviorSubject<ApiErrorResult | undefined>(undefined);
    public apiError$ = this._apiError.asObservable();
    constructor() { }

    handleError(error: HttpErrorResponse): void {
        console.error('An error occurred:', error);
    }

    handleBadRequest(response: HttpErrorResponse): void {
        if (response.error && response.error.errors) {
            if (response.error.errors.message) {
                this._apiError.next({
                    message: response.error.errors.message,
                    hasValidationErrors: false
                });
            } else {
                const validationErrors: ApiErrorResult = {
                    message: 'Validation errors occurred',
                    validationErrors: Object.keys(response.error.errors).map(key => ({
                        propertyName: key,
                        errors: response.error.errors[key] || []
                    })),
                    hasValidationErrors: true
                };
                this._apiError.next(validationErrors);
            }
        }
    }

    clear(): void {
        this._apiError.next(undefined);
    }
}
