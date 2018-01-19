import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import { AuthService } from "../../auth/auth.service";
import "rxjs/add/operator/publishReplay";
import { UploadLinkModel } from "./upload-link.model";
import { ApiErrorModel } from "../../shared/api-error.model";




@Injectable()
export class UploadLinkService {

    constructor(private http: HttpClient, private authService: AuthService) { }

    /**
     * Get an upload link with the given quick link. Returns null on failure
     * 
     * @returns {Observable<UploadLinkModel>} 
     * @memberof UploadLinkService
     */
    getUploadLink(quickLink: string): Observable<UploadLinkModel> {

        return this.http.get<UploadLinkModel>("/api/uploadlinks/" + quickLink, { headers: this.authService.authHeader() })
            .catch((error: HttpErrorResponse) => {
                return Observable.of(null);
            });

    }

    /**
     * Get the list of all upload links, returning an array of UploadLinkModels on success, or null on failure
     * 
     * @returns {Observable<UploadLinkModel[]>} 
     * @memberof UploadLinkService
     */
    getUploadLinks(): Observable<UploadLinkModel[]> {

        return this.http.get<UploadLinkModel[]>("/api/uploadlinks", { headers: this.authService.authHeader() });

    }
    /**
     * Create a new upload link and return the created UploadLinkModel on success, or null on failure. Also returns a string for the reason on failure
     * 
     * @param {UploadLinkModel} uploadLinkToCreate 
     * @returns {Observable<[UploadLinkModel?, string?]>}
     * @memberof UploadLinkService
     */
    createUploadLink(uploadLinkToCreate: UploadLinkModel): Observable<[UploadLinkModel, string]> {

        return this.http.post<UploadLinkModel>("api/uploadlinks", uploadLinkToCreate, { headers: this.authService.authHeader() })
            .map((paste: UploadLinkModel) => {
                let formattedResponse: [UploadLinkModel, string] = [paste, null];
                return formattedResponse;
            })
            .publishReplay(1)
            .refCount()
            .catch((errorResp: HttpErrorResponse) => {
                let errorBody: ApiErrorModel = errorResp.error;
                let formattedResponse: [UploadLinkModel, string] = [null, errorBody.display_text];
                return Observable.of(formattedResponse);
            });
    }




    /**
     * Deletes an upload link with the given quickLink
     * 
     * @param {string} quickLink 
     * @returns {Observable<[UploadLinkModel?, string?]>} 
     * @memberof UploadLinkModel
     */
    deleteUploadLink(quickLink: string): Observable<boolean> {

        return this.http.delete(`/api/uploadlinks/${quickLink}`, { headers: this.authService.authHeader() })
            .map((deletedUploadLink: UploadLinkModel) => {
                return true;
            })
            .catch((error: HttpErrorResponse) => {
                return Observable.of(false);
            });

    }

}
