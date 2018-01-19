import { Injectable } from "@angular/core";
import { LoginModel } from "./login.model";
import { Observable } from "rxjs/Observable";
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/of';
import { HttpClient, HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { TokenModel } from "./token.model";
import { HttpHeaders } from "@angular/common/http";

export const TOKEN_NAME: string = 'jwt_token';
export const EXPIRATION_NAME: string = 'jwt_token_exp';
export const USERNAME_NAME: string = 'jwt_token_uname';

@Injectable()
export class AuthService {


    constructor(private http: HttpClient) { }

    /**
     * Returns HttpHeaders with the current user's jwt token set to the Authorization
     * 
     * @returns {(HttpHeaders)} 
     * @memberof AuthService
     */
    authHeader(): HttpHeaders | null {
        const jwt_token = localStorage.getItem(TOKEN_NAME);
        if (jwt_token) {
            return new HttpHeaders({ Authorization: "Bearer " + jwt_token });
        }
        return new HttpHeaders();
    }

    /**
     * Logs in the user and returns true on success, false on failure
     * 
     * @param {string} userName 
     * @param {string} password 
     * @returns {Observable<boolean>} 
     * @memberof AuthService
     */
    loginUser(userName: string, password: string): Observable<boolean> {
        let loginModel: LoginModel = new LoginModel();
        loginModel.username = userName;
        loginModel.password = password;
        return this.http.post<TokenModel>('/api/authentication/login', loginModel).map(token => {
            if (token) {
                this.setSession(token);
                return true;
            }
            return false;
        }).catch((error: HttpErrorResponse) => {
            console.log(error.status);
            console.log(error.error);
            return Observable.of(false);
        });


    }
    logoutUser() {
        this.clearSession();
        return true;
    }

    /**
     * Saves the JWT, its expiration time and username to persistent storage
     * 
     * @param {TokenModel} token 
     * @memberof AuthService
     */
    setSession(token: TokenModel) {
        localStorage.setItem(TOKEN_NAME, token.access_token);
        localStorage.setItem(EXPIRATION_NAME, new Date(token.expiration).toISOString());
        localStorage.setItem(USERNAME_NAME, token.username);
    }

    /**
     * Removes all user information in persistent storage (JWT, expiration, username) to logout user
     * 
     * @private
     * @memberof AuthService
     */
    private clearSession() {
        localStorage.removeItem(TOKEN_NAME);
        localStorage.removeItem(EXPIRATION_NAME);
        localStorage.removeItem(USERNAME_NAME);
    }

    isAuthenticated(): boolean {
        let firstCond = !!localStorage.getItem(TOKEN_NAME);
        let val1 =  Date.parse(localStorage.getItem(EXPIRATION_NAME));
        let val2 = Date.now();
        return firstCond && val1 > val2;
    }

    /**
     * Gets the expiration date of the JWT
     * 
     * @returns {Date} 
     * @memberof AuthService
     */
    getExpiration(): Date {
        const expDate: Date = new Date(Date.parse(localStorage.getItem(EXPIRATION_NAME)));
        return expDate;
    }

    getUsername(): string {
        const uname: string = (localStorage.getItem(USERNAME_NAME));
        return uname;
    }


}