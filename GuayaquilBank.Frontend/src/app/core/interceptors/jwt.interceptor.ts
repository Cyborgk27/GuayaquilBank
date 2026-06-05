import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthFacade } from '../service/auth.facade';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthFacade);
  const token = authService.token();

  if (token && req.url.includes('/api/')) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};
