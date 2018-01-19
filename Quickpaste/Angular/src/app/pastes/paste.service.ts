import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import { PasteModel, PasteCreateModel, AnonPasteCreateModel } from "./models/paste.model";
import { AuthService } from "../auth/auth.service";
import { SharedLinkCreateModel, SharedLinkModel } from "./models/shared-link.model";
import "rxjs/add/operator/publishReplay";




@Injectable()
export class PasteService {

    /**
     * A map used to cache Pastes and retrieve them from memory
     */
    pasteMap: Map<string, Observable<PasteModel>> = new Map<string, Observable<PasteModel>>();

    constructor(private http: HttpClient, private authService: AuthService) { }

    /**
     * Gets the paste with the specified quickLink. The response is formatted into an array with a PasteModel and string for error message.
     * If the paste could not be retrieved, the 1st element of the array is set to null and the 2nd element contains an error message.
     * If the paste was found, the 1st element contains the PasteModel and the 2nd element is set to null
     * 
     * @param {string} quickLink 
     * @returns {Observable<[PasteModel?, string?]>} 
     * @memberof PasteService
     */
    getPaste(quickLink: string): Observable<[PasteModel, string]> {
        let cachedPaste = this.pasteMap.get(quickLink)
        if (cachedPaste) {
            return cachedPaste.map((resp: PasteModel) => {
                let formattedResponse: [PasteModel, string] = [resp, null];
                return formattedResponse;
            });
        }
        return this.http.get<PasteModel>(`/api/pastes/${quickLink}?isLink=true`, { headers: this.authService.authHeader() })
            .map((paste: PasteModel) => {
                this.pasteMap.set(paste.quick_link, Observable.of(paste));
                let formattedResponse: [PasteModel, string] = [paste, null];
                return formattedResponse;
            })
            .catch((error: HttpErrorResponse) => {
                let formattedResponse: [PasteModel, string] = [null, 'An unexpected error occured'];

                if (error.status === 404) {
                    formattedResponse[1] = 'Paste not found';
                } else if (error.status === 401) {
                    formattedResponse[1] = 'This paste is private';
                }
                return Observable.of(formattedResponse);
            });

    }

    /**
     * Get the list of all paste models, returning an array of PasteModels on success, or null on failure
     * 
     * @param {number} [pageNumber=1]  
     * @param {number} [limit=10]  
     * @returns {Observable<PasteModel[]>} 
     * @memberof PasteService
     */
    getPastes(pageNumber: number = 1, limit: number = 10): Observable<PasteModel[]> {

        return this.http.get<PasteModel[]>(`/api/pastes?pageNumber=${pageNumber}&limit=${limit}`, { headers: this.authService.authHeader() })
            .do((pastes: PasteModel[]) => {
                pastes.forEach(paste => {
                    this.pasteMap.set(paste.quick_link, Observable.of(paste));
                });
            });

    }

    /**
     * Create a new paste and return the created PasteModel on success, or null on failure
     * 
     * @param {PasteCreateModel} pasteToCreate 
     * @returns {Observable<PasteModel?>}
     * @memberof PasteService
     */
    createPaste(pasteToCreate: PasteCreateModel): Observable<PasteModel> {
        let formData = new FormData();
        if (pasteToCreate.file_to_upload !== null) {
            formData.append('file_to_upload', pasteToCreate.file_to_upload[0]);
        }
        formData.append('is_public', pasteToCreate.is_public + "");
        formData.append('message', pasteToCreate.message);
        formData.append('quick_link', pasteToCreate.quick_link);

        return this.http.post<PasteModel>("api/pastes", formData, { headers: this.authService.authHeader() })
            .do((paste: PasteModel) => this.pasteMap.set(paste.quick_link, Observable.of(paste)))
            .publishReplay(1)
            .refCount()
            .catch((error: HttpErrorResponse) => {
                console.log(error.status);
                console.log(error.error);
                return Observable.of(null);
            });
    }

    /**
     * Generates a random alphanumeric string
     * 
     * @returns {string} 
     * @memberof PasteService
     */
    generateRandomPasteLink(): string {
        let link = "";
        let charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        let length = 32;

        for (let i = 0; i < length; i++) {
            link += charset.charAt(Math.floor(Math.random() * charset.length));
        }
        return link;
    }

    /**
     *  Create a link which can be used to view a blob for the paste that is normally private. After durationInMinutes has passed, the link is no longer valid
     * 
     * @param {string} pasteId 
     * @param {number} durationInMinutes 
     * @returns {Observable<SharedLinkModel>} 
     * @memberof PasteService
     */
    createSharedLink(pasteId: string, durationInMinutes: number): Observable<SharedLinkModel> {
        let sharedLinkInput: SharedLinkCreateModel = { duration_in_minutes: durationInMinutes };

        return this.http.post<SharedLinkModel>(`/api/pastes/${pasteId}/sharedlinks`, sharedLinkInput, { headers: this.authService.authHeader() })
            .catch((error: HttpErrorResponse) => {
                console.log(error.status);
                console.log(error.error);
                return Observable.of(null);
            });
    }

    /**
     * Create a paste anonymously using an upload link
     * 
     * @param {string} quickLink 
     * @param {AnonPasteCreateModel} pasteToCreate 
     * @returns {Observable<boolean>} 
     * @memberof PasteService
     */
    createPasteFromLink(quickLink: string, pasteToCreate: AnonPasteCreateModel): Observable<boolean> {
        let formData = new FormData();
        if (pasteToCreate.file_to_upload !== null) {
            formData.append('file_to_upload', pasteToCreate.file_to_upload[0]);
        }
        formData.append('message', pasteToCreate.message);

        return this.http.post(`api/pastes/${quickLink}`, formData, { headers: this.authService.authHeader() })
            .map((resp: any) => {
                return true;
            })
            .publishReplay(1)
            .refCount()
            .catch((error: HttpErrorResponse) => {
                console.log(error.status);
                console.log(error.error);
                return Observable.of(false);
            });
    }


    /**
     * Deletes a paste with the given id
     * 
     * @param {string} id 
     * @returns {Observable<boolean>} 
     * @memberof PasteService
     */
    deletePaste(id: string): Observable<boolean> {

        return this.http.delete(`/api/pastes/${id}`, { headers: this.authService.authHeader() })
            .map((deletedPaste: PasteModel) => {
                this.pasteMap.delete(deletedPaste.quick_link);
                return true;
            })
            .catch((error: HttpErrorResponse) => {
                return Observable.of(false);
            });

    }

}
