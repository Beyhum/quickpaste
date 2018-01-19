import { Injectable } from '@angular/core';
import { RegisterModel, RegisterResponse } from './register.model';
import { AuthService } from '../../auth/auth.service';
import { TokenModel } from '../../auth/token.model';
import { Observable } from 'rxjs/Observable';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ApiErrorModel } from '../../shared/api-error.model';

@Injectable()
export class SetupAccountService {

  constructor(private http: HttpClient, private authService: AuthService) { }

  register(registerModel: RegisterModel): Observable<RegisterResponse> {
    let registerObservable = this.http.post<TokenModel>('/api/authentication/register', registerModel).map((token: TokenModel) => {
      this.authService.setSession(token);
      return new RegisterResponse(true, "Successfully registered " + this.authService.getUsername());
    }).catch((errorResponse: HttpErrorResponse) => {
      let errorBody: ApiErrorModel = errorResponse.error;
      return Observable.of(new RegisterResponse(false, errorBody.display_text));
    });

    return registerObservable;
  }

}
