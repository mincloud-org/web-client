import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ErrorHandler } from '../handlers/error-handler';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorHandler = inject(ErrorHandler);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 400) {
        // Handle bad request error
        errorHandler.handleError(error);
      } else if (error.status === 401) {
        // Handle unauthorized error

      } else if (error.status === 403) {
        // Handle forbidden error

      } else if (error.status === 404) {
        // Handle not found error

      } else {
        // Handle other errors
        errorHandler.handleError(error);
      }
      return throwError(() => error);
    })
  );
};
