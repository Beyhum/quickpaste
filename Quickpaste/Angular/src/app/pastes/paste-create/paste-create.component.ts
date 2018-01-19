import { Component, OnInit, ElementRef, ViewChild, Input } from '@angular/core';
import { FormControl } from "@angular/forms";
import { Validators } from "@angular/forms";
import { FormGroup } from "@angular/forms";
import { PasteCreateModel, PasteModel } from '../models/paste.model';
import { PasteService } from '../paste.service';
import { FileValidator } from '../file.validator';
import { SettingsService } from '../../shared/settings/settings.service';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'paste-create',
  templateUrl: './paste-create.component.html',
  styleUrls: ['../../shared/styles/validation.css']
})
export class PasteCreateComponent implements OnInit {

  newPasteForm: FormGroup;
  message: FormControl;
  is_public: FormControl;
  defaultPublic = false;

  settingsService: SettingsService;

  constructor(private pasteService: PasteService, _settingsService: SettingsService, private fileValidator: FileValidator, private toastr: ToastrService,
    private router: Router, private location: Location) {
    this.settingsService = _settingsService;
    this.initializeForm();
  }

  cancel() {
    this.location.back();
  }

  createPaste(formValues) {
    let pasteToCreate: PasteCreateModel = {
      quick_link: formValues.quick_link,
      message: formValues.message,
      is_public: formValues.is_public,
      file_to_upload: formValues.file_to_upload
    };
    this.pasteService.createPaste(pasteToCreate).subscribe((createdPaste: PasteModel) => {
      if (createdPaste) {
        this.toastr.success('Paste created');
        this.router.navigate(['/pastes']);
      } else {
        this.toastr.error('Failed to create paste');
      }
    });
  }

  /**
   * listens to the file input's (change) event and updates newPasteForm's file_to_upload value
   * 
   * @param {any} fileChangeEvent 
   * @memberof PasteCreateComponent
   */
  fileChange(fileChangeEvent) {
    let files = fileChangeEvent.target.files;
    this.newPasteForm.controls['file_to_upload'].setValue(files);
  }

  private initializeForm() {
    this.message = new FormControl('');
    this.is_public = new FormControl(this.defaultPublic, Validators.required);
    this.newPasteForm = new FormGroup({
      message: this.message,
      is_public: this.is_public,
      file_to_upload: new FormControl(null, [this.fileValidator.createValidateFile()])
    });
  }

  ngOnInit() { }

}
