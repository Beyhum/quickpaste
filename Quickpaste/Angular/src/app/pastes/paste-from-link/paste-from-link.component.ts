import { Component, OnInit } from '@angular/core';
import { UploadLinkService } from '../../upload-links/upload-link/upload-link.service';
import { ActivatedRoute } from '@angular/router';
import { UploadLinkModel } from '../../upload-links/upload-link/upload-link.model';
import { FormGroup, FormControl } from '@angular/forms';
import { SettingsService } from '../../shared/settings/settings.service';
import { Router } from '@angular/router';
import { AnonPasteCreateModel } from '../models/paste.model';
import { PasteService } from '../paste.service';
import { ToastrService } from 'ngx-toastr';
import { FileValidator } from '../file.validator';

/**
 * Component to create Pastes using an Upload Link that was created by the user
 * 
 * @export
 * @class PasteFromLinkComponent
 * @implements {OnInit}
 */
@Component({
  templateUrl: './paste-from-link.component.html',
  styleUrls: ['../../shared/styles/validation.css']
})
export class PasteFromLinkComponent implements OnInit {

  uploadLink: UploadLinkModel;

  allowFile: boolean = false;

  newPasteForm: FormGroup;
  message: FormControl;

  settingsService: SettingsService;

  constructor(private uploadLinkService: UploadLinkService, private pasteService: PasteService, private fileValidator: FileValidator, 
    private toastr: ToastrService, private route: ActivatedRoute, settingsService: SettingsService, private router: Router) {
    this.settingsService = settingsService;
    this.initializeForm();
  }

  cancel() {
    this.router.navigate(['/']);
  }

  createPaste(formValues) {
    let pasteToCreate: AnonPasteCreateModel = {
      message: formValues.message,
      file_to_upload: formValues.file_to_upload
    };
    this.pasteService.createPasteFromLink(this.uploadLink.quick_link, pasteToCreate).subscribe((success: boolean) => {
      if (success) {
        this.toastr.success('Paste created');
        this.router.navigate(['/']);
      } else {
        this.toastr.error('Failed to create paste');
      }
    });
  }

  fileChange(fileChangeEvent) {
    let files = fileChangeEvent.target.files;
    this.newPasteForm.controls['file_to_upload'].setValue(files);
  }

  private initializeForm() {
    this.message = new FormControl('');
    this.newPasteForm = new FormGroup({
      message: this.message,
      file_to_upload: new FormControl(null, [this.fileValidator.createValidateFile()])
    });
  }
  ngOnInit() {
    this.route.data.forEach((data) => {
      this.uploadLink = data['uploadLink'];
      if(this.uploadLink == null){
        this.toastr.error("This link has already been used/doesn't exist");
      }
      this.allowFile = this.uploadLink? this.uploadLink.allow_file : false;
    });
  }

}
