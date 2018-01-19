import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { PasteService } from '../../pastes/paste.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

/**
 * Component which wraps an input element and allows easy generation of quick links. 
 * A reference to the form which will use it as well as the formControlName to use must be passed as input
 * 
 * @export
 * @class QuicklinkInputComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'quicklink-input',
  templateUrl: './quicklink-input.component.html',
  styleUrls: ['../styles/validation.css']
})
export class QuicklinkInputComponent implements OnInit {

  quick_link: FormControl;
  // The formControlName attribute will be passed by the parent component
  @Input('quickLinkControlName') quickLinkControlName: string;
  
  // The parent component passes a form to which we add the quick_link control
  @Input('parentForm') parentForm: FormGroup;

  // Reference to the input tag that will be manipulated in clearQuickLink()
  @ViewChild('quickLinkInput') quickLinkInputEle: ElementRef;

  constructor(private pasteService: PasteService) { }

  generateRandomLink(): string {
    let randLink = this.pasteService.generateRandomPasteLink();
    this.parentForm.controls[this.quickLinkControlName].setValue(randLink);
    return randLink;
  }

  /**
   * Empties the form's input which corresponds to the quick link and focus the input box
   * 
   * @memberof QuicklinkInputComponent
   */
  clearQuickLink() {
    this.parentForm.controls[this.quickLinkControlName].setValue('');
    this.quickLinkInputEle.nativeElement.focus();
  }

  /**
   * Add a formControl named {{quickLinkControlName}} to the parent form 
   * 
   * @private
   * @memberof QuicklinkInputComponent
   */
  private initializeForm() {
    this.quick_link = new FormControl('', [Validators.required, Validators.minLength(1), this.ValidateQuickLink]);
    this.parentForm.addControl(this.quickLinkControlName, this.quick_link);
    
  }

  ngOnInit() {
    this.initializeForm();    
    this.generateRandomLink();
  }

  ValidateQuickLink(control: FormControl) {
    if (control.value.length > 0 && !/^[a-zA-Z0-9]+$/i.test(control.value)) {
      return { validateQuickLink: 'Link must use alphanumerical characters' };
    }
    return null;
  }

}
