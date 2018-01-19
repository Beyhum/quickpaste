import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { UploadLinkService } from './upload-link.service';
import { UploadLinkModel } from './upload-link.model';

/**
 * Component to create and manage upload links (upload links can be used to anonymously create a paste without authentication)
 * 
 * @export
 * @class UploadLinkComponent
 * @implements {OnInit}
 */
@Component({
  templateUrl: './upload-link.component.html',
  styleUrls: ['./upload-link.component.css', '../../shared/clipboard/clipboard.css']
})
export class UploadLinkComponent implements OnInit {

  uploadLinks: UploadLinkModel[] = [];

  newUploadLinkForm: FormGroup;
  allow_file: FormControl;
  defaultAllowFile = false;
  @ViewChild('quickLinkInput') quickLinkInputEle: ElementRef;


  constructor(private uploadLinkService: UploadLinkService, private toastr: ToastrService, private router: Router, private location: Location) {
    this.initializeForm();
  }

  cancel() {
    this.location.back();
  }

  
  getFullUrl(quickLink: string): string {
    return location.href + "/" + quickLink;

  }

  createUploadLink(formValues) {
    let uploadLinkToCreate: UploadLinkModel = {
      quick_link: formValues.quick_link,
      allow_file: formValues.allow_file,
    };
    this.uploadLinkService.createUploadLink(uploadLinkToCreate).subscribe((result: [UploadLinkModel, string]) => {
      if (result[0]) {
        this.toastr.success('Upload Link created');
        this.uploadLinks.push(result[0]);     
      } else {
        this.toastr.error(result[1]);
      }
    });
  }

  deleteUploadLink(quickLink: string) {
    this.uploadLinkService.deleteUploadLink(quickLink).subscribe((success: boolean) => {
      if(success) {
        this.uploadLinks = this.uploadLinks.filter(link => link.quick_link.localeCompare(quickLink) !== 0);
      }
    });
  }


  private initializeForm() {
    this.allow_file = new FormControl(this.defaultAllowFile, Validators.required);
    this.newUploadLinkForm = new FormGroup({
      allow_file: this.allow_file
    });
  }

  ngOnInit(): void {
    this.uploadLinkService.getUploadLinks().subscribe((uploadLinks: UploadLinkModel[]) => {
      this.uploadLinks = uploadLinks;
    });
  }

}
