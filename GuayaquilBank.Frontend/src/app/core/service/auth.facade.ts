import { inject, Injectable, signal, computed } from '@angular/core';
import { map, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { AuthApiService, LoginRequestDto, LoginResponseDto } from '../api/v1';

@Injectable({
  providedIn: 'root'
})
export class AuthFacade {
  private authApiService = inject(AuthApiService);

  private readonly TOKEN_KEY = 'gb_erp_token';
  private readonly USER_KEY = 'gb_erp_user';

  private _token = signal<string | null>(localStorage.getItem(this.TOKEN_KEY));
  private _currentUser = signal<LoginResponseDto | null>(this.getStoredUser());

  public isAuthenticated = computed<boolean>(() => this._token() !== null);

  public token = computed<string | null>(() => this._token());

  public currentUser = computed<LoginResponseDto | null>(() => this._currentUser());

  public currentTenantId = computed<string | null>(() => this._currentUser()?.companyId ?? null);

  // ==========================================
  // MÉTODOS PÚBLICOS
  // ==========================================

  /**
    * Ejecuta el flujo de autenticación contra la API unificada.
    */
  public login(credentials: LoginRequestDto): Observable<LoginResponseDto> {
    return this.authApiService.apiAuthLoginPost(credentials).pipe(

      map((response: any) => response as LoginResponseDto),

      tap((sessionData: LoginResponseDto) => {
        if (sessionData && sessionData.token) {
          this.setSession(sessionData.token, sessionData);
        }
      })
    );
  }

  /**
   * Cierra la sesión activa limpiando los Signals y destruyendo el LocalStorage.
   */
  public logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);

    this._token.set(null);
    this._currentUser.set(null);
  }

  /**
   * Permite actualizar la información del perfil del usuario en sesión de forma reactiva
   * (Útil cuando modifiquemos el perfil en el ProfileController).
   */
  public updateLocalUser(updatedUser: Partial<LoginResponseDto>): void {
    const current = this._currentUser();
    if (current) {
      const mergedUser = { ...current, ...updatedUser };
      localStorage.setItem(this.USER_KEY, JSON.stringify(mergedUser));
      this._currentUser.set(mergedUser);
    }
  }

  // ==========================================
  // HELPERS PRIVADOS
  // ==========================================

  private setSession(token: string, user: LoginResponseDto): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));

    this._token.set(token);
    this._currentUser.set(user);
  }

  private getStoredUser(): LoginResponseDto | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (!userJson) return null;
    try {
      return JSON.parse(userJson) as LoginResponseDto;
    } catch {
      return null;
    }
  }
}
