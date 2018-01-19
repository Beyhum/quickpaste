import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot } from "@angular/router";
import { Observable } from "rxjs/Observable";
import { PasteService } from "../paste.service";
import { PasteModel } from "../models/paste.model";

@Injectable()
export class PasteResolver implements Resolve<any> {

    constructor(private pasteService: PasteService){

    }

    resolve(route: ActivatedRouteSnapshot): Observable<[PasteModel, string]> {
        return this.pasteService.getPaste(route.params['quick_link']);
    }

}