import { Injectable } from '@angular/core';
import { SettingsModel } from './settings.model';
import { AuthService } from '../../auth/auth.service';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class SettingsService {

  settings: SettingsModel = new SettingsModel();
  constructor(private http: HttpClient, private authService: AuthService) { }

  getSettings(): SettingsModel {
    return this.settings;
  }

  /**
   * Retrieves the settings from the server and stores them in memory
   * 
   * @memberof SettingsService
   */
  refreshSettings() {
    let settingsObs: Observable<SettingsModel> = this.http.get<SettingsModel>("/api/settings", {headers: this.authService.authHeader()})
    .catch((error: HttpErrorResponse) => {
        console.log(error.status);
        console.log(error.error);
        return Observable.of(null);
    });

    settingsObs.subscribe((settings: SettingsModel) =>{
      if(settings) {
        this.settings = settings;
      }
    })
  }

}
