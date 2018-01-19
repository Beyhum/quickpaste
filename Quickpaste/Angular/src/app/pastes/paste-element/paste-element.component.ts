import { Component, OnInit, Input, ViewChild, ElementRef, AfterViewInit, ViewChildren, QueryList } from '@angular/core';
import { PasteService } from '../paste.service';
import { WindowRefService } from '../../shared/window-ref.service';
import { ToastrService } from 'ngx-toastr';
import { SharedLinkModel } from '../models/shared-link.model';
import { PasteModel } from '../models/paste.model';
import { AfterContentInit, AfterViewChecked, OnChanges } from '@angular/core/src/metadata/lifecycle_hooks';

@Component({
  selector: 'paste-element',
  templateUrl: './paste-element.component.html',
  styleUrls: ['./paste-element.component.css', '../../shared/clipboard/clipboard.css']
})
export class PasteElementComponent implements AfterViewInit {

  // property to determine whether the message div's text is overflowing and should be cropped by default
  textIsOverflow: boolean;

  // property to crop/uncrop message divs
  messageIsCropped: boolean = true;

  @Input() paste: PasteModel;
  @ViewChildren('messageElement') messageElement: QueryList<ElementRef>;

  @ViewChild('sharedLinkModal') sharedLinkModal: ElementRef;
  sharedLink: string = null;
  sharedLinkDuration: number = 5;

  constructor(private pasteService: PasteService, private windowRef: WindowRefService, private toastr: ToastrService) {
  }

  /**
   * Creates a shared link with a short duration (3mins) for the authenticated user to gain access to a blob and download it
   * The new blob is downloaded by opening the temporary link in a new tab
   * 
   * @param {string} pasteId The ID of the paste whose file/blob to download
   * @memberof PasteListComponent
   */
  downloadPasteBlob(pasteId: string): void {
    this.pasteService.createSharedLink(pasteId, 3).subscribe((sharedLink: SharedLinkModel) => {
      if (sharedLink != null) {
        let newWin = this.windowRef.getNativeWindow().open(sharedLink.blob_url, "_blank");
        if (!newWin || newWin.closed || typeof newWin.closed == 'undefined') {
          this.toastr.error("Failed to download. Please check if popups are disabled and enable them");
        }

      }
    }
    );
  }

  onDurationChanged(keyUpEvent: any) {
    this.sharedLinkDuration = keyUpEvent.target.value;
  }

  initializeSharedLink() {
    this.sharedLink = null;
    this.sharedLinkDuration = 5;
  }

  /**
   * Generates a shared link with duration equal to sharedLinkDuration. 
   * On success, sets the component's sharedLink property to the new link value.
   * 
   * @memberof PasteListComponent 
   */
  generateSharedLink() {
    this.pasteService.createSharedLink(this.paste.id, this.sharedLinkDuration).subscribe((sharedLink: SharedLinkModel) => {
      if (sharedLink != null) {
        this.sharedLink = sharedLink.blob_url;
      } else {
        this.toastr.error("Failed to generate shared link");
      }
    });
  }

  /**
   * Expands/collapses a message div that's too long to fit in its container element
   * 
   * @memberof PasteElementComponent
   */
  toggleCrop() {
    this.messageIsCropped = !this.messageIsCropped;
  }

  /**
   * Determines whether the height of the message div is greater than the maximum allowed height by its container.
   * Used to display the "expand" arrow when a message should be cropped by default
   * 
   * @returns {boolean} true if the length of the message is longer than its container
   * @memberof PasteElementComponent
   */
  isOverflow(): boolean {
    let messageNativeElement = this.messageElement.first ? this.messageElement.first.nativeElement : null;
    return this.messageElement.first && messageNativeElement.scrollHeight > messageNativeElement.offsetHeight;
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.textIsOverflow = this.isOverflow();
    }, 0);
  }

}
