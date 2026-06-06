import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Ui } from '../service/ui';
import { ObjectApiResponse } from '../api/v1';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const ui = inject(Ui);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocurrió un error inesperado en el servidor.';
      let errorTitle = '¡Ups! Algo salió mal';

      const apiResponse = error.error && typeof error.error === 'object'
        ? (error.error as ObjectApiResponse)
        : null;

      if (error.error instanceof ErrorEvent) {
        errorMessage = `Error de red: ${error.error.message}`;
      } else {
        switch (error.status) {
          case 400:
            errorTitle = 'Solicitud Incorrecta';
            errorMessage = apiResponse?.message || 'Los datos enviados fallaron las validaciones.';

            if (apiResponse?.errors && apiResponse.errors.length > 0) {
              errorMessage = apiResponse.errors.join('<br>');
            }
            break;

          case 401:
            errorTitle = 'Sesión Expirada';
            errorMessage = apiResponse?.message || 'Tu sesión ha terminado. Por favor, vuelve a iniciar sesión.';
            router.navigate(['/login']);
            break;

          case 403:
            errorTitle = 'Acceso Denegado';
            errorMessage = apiResponse?.message || 'No tienes permisos suficientes para realizar esta acción.';
            break;

          case 404:
            errorTitle = 'No Encontrado';
            errorMessage = apiResponse?.message || 'El recurso solicitado no existe o pertenece a otro Tenant.';
            break;

          case 422:
            errorTitle = 'Datos Inválidos';
            if (apiResponse?.errors && apiResponse.errors.length > 0) {
              errorMessage = apiResponse.errors.join('<br>');
            } else {
              errorMessage = apiResponse?.message || 'Por favor verifica los campos ingresados.';
            }
            break;

          case 500:
            errorTitle = 'Error Interno';
            errorMessage = apiResponse?.message || 'El servidor experimentó un problema en el core. Inténtalo más tarde.';
            break;

          default:
            errorMessage = apiResponse?.message || `Error controlado por el sistema [Código ${error.status}].`;
            break;
        }
      }

      ui.showError(errorMessage, errorTitle);

      return throwError(() => error);
    })
  );
};
