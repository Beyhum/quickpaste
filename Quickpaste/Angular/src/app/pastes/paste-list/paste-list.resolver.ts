import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot } from "@angular/router";
import { Observable } from "rxjs/Observable";
import { PasteService } from "../paste.service";
import { PasteModel } from "../models/paste.model";

@Injectable()
export class PasteListResolver implements Resolve<any> {

    constructor(private pasteService: PasteService){

    }

    resolve(): Observable<PasteModel[]> {
        return this.pasteService.getPastes(1, 10);
    }
}