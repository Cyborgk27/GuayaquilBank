import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Ui } from '../service/ui';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const ui = inject(Ui);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocurrió un error inesperado en el servidor.';
      let errorTitle = '¡Ups! Algo salió mal';

      if (error.error instanceof ErrorEvent) {
        errorMessage = `Error de red: ${error.error.message}`;
      } else {
        switch (error.status) {
          case 400:
            errorTitle = 'Solicitud Incorrecta';
            errorMessage = error.error?.message || 'Los datos enviados son inválidos.';
            break;

          case 401:
            errorTitle = 'Sesión Expirada';
            errorMessage = 'Tu sesión ha terminado. Por favor, vuelve a iniciar sesión.';
            break;

          case 403:
            errorTitle = 'Acceso Denegado';
            errorMessage = 'No tienes permisos para realizar esta acción.';
            break;

          case 422:
            errorTitle = 'Datos Inválidos';
            if (error.error?.errors) {
              errorMessage = Object.values(error.error.errors).flat().join('<br>');
            } else {
              errorMessage = error.error?.message || 'Por favor verifica los campos.';
            }
            break;

          case 404:
            errorTitle = 'No Encontrado';
            errorMessage = error.error?.message || 'El recurso solicitado no existe.';
            break;

          case 500:
            errorTitle = 'Error Interno';
            errorMessage = 'El servidor experimentó un problema. Inténtalo más tarde.';
            break;

          default:
            errorMessage = error.error?.message || `Error código ${error.status}`;
            break;
        }
      }

      ui.showError(errorMessage, errorTitle);

      return throwError(() => error);
    })
  );
};
