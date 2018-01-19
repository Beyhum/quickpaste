import { Directive, OnInit, Inject, HostListener, Input, ElementRef, EventEmitter, Output } from '@angular/core';
import Clipboard from 'clipboard';
import { ToastrService } from 'ngx-toastr';

/**
 * Wrapper directive for clipboardjs library. Copies the text passed to clipboard-copy to clipboard.
 * If clipboard-target is specified, will copy the textContent of the element passed to clipboard-target
 */
@Directive({
    selector: '[clipboard-copy]'
})
export class ClipboardDirective {
    clipboard: Clipboard;

    @Input('clipboard-copy') textToSendToClipboard: string;

    // Reference to an element whose textContent we want to copy to clipboard.
    // To use when copying text won't work. e.g. when a modal is open
    @Input('clipboard-target') elementToCopyFrom: ElementRef;

    @Output()
    clipboardSuccess: EventEmitter<any> = new EventEmitter();

    @Output()
    clipboardError: EventEmitter<any> = new EventEmitter();

    constructor(private hostElementRef: ElementRef, private toastr: ToastrService) {
    }

    ngOnInit() {

        // If a target element has been specified, set the text to null and force clipboardjs to copy through the target element
        let textToCopyFunction = this.elementToCopyFrom ? null : () => {
            return this.textToSendToClipboard;
        };

        this.clipboard = new Clipboard(this.hostElementRef.nativeElement, {
            text: textToCopyFunction,
            target: () => {
                return this.elementToCopyFrom;
            }
        });

        this.clipboard.on('success', (e) => {
            this.toastr.info("Copied to clipboard");
            this.clipboardSuccess.emit();
        });

        this.clipboard.on('error', (e) => {
            this.toastr.error("Failed to copy to clipboard");            
            this.clipboardError.emit();
        });
    }

    ngOnDestroy() {
        if (this.clipboard) {
            this.clipboard.destroy();
        }
    }
}