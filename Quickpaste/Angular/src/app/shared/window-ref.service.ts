import { Injectable } from "@angular/core";

@Injectable()
export class WindowRefService {
    constructor(){}
    getNativeWindow() {
        return window;
    }
}