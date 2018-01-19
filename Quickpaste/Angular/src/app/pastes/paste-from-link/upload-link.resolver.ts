import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot } from "@angular/router";
import { Observable } from "rxjs/Observable";
import { PasteModel } from "../models/paste.model";
import { UploadLinkService } from "../../upload-links/upload-link/upload-link.service";
import { UploadLinkModel } from "../../upload-links/upload-link/upload-link.model";

@Injectable()
export class UploadLinkResolver implements Resolve<any> {

    constructor(private uploadLinkService: UploadLinkService){

    }

    resolve(route: ActivatedRouteSnapshot): Observable<UploadLinkModel> {
        return this.uploadLinkService.getUploadLink(route.params['quick_link']);
    }

}