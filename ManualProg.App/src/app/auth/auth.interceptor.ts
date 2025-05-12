import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "./data-access/auth.service";

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);

  const token = authService.getToken();

  request = request.clone({
    setHeaders: {
      Authorization: token !== null ? `Bearer ${token}` : '',
    },
  });

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const instance: string = error.error?.instance ?? '';

      if (error.status === 401 && !instance.includes('refresh')) {
        return authService.refresh().pipe(
          switchMap(response => {
            const retriedRequest = request.clone({
              setHeaders: {
                Authorization: `Bearer ${response.accessToken}`,
              },
            });

            return next(retriedRequest);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
